using Microsoft.AspNetCore.Mvc;
using VotingApp.Models;
using VotingApp.Services;
using System.Linq;
using System.Text;

namespace VotingApp.Controllers
{
    public class VotingController : Controller
    {
        private readonly MongoDbService _mongoService;
        private static readonly List<string> candidates = new List<string>
        {
            "BJP", "Congress", "AAP"
        };

        // Inject MongoDbService via constructor
        public VotingController(MongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        public IActionResult Index()
        {
            ViewBag.Candidates = candidates;
            return View();
        }

        [HttpPost]
        public IActionResult SubmitVote(string voterName, string candidate)
        {
            voterName = voterName?.Trim();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(voterName) || string.IsNullOrWhiteSpace(candidate))
            {
                TempData["Error"] = "Please enter your name and select a candidate.";
                return RedirectToAction("Index");
            }

            // Restrict by voter name only (allow multiple votes from same IP)
            if (_mongoService.VoterExists(voterName))
            {
                TempData["Error"] = "You have already voted!";
                return RedirectToAction("Index");
            }

            _mongoService.AddVote(new Vote
            {
                VoterName = voterName,
                Candidate = candidate,
                IP = ipAddress ?? "Unknown",
                VoteTime = DateTime.Now
            });

            TempData["Success"] = "Your vote has been recorded successfully!";
            return RedirectToAction("Results");
        }

        public IActionResult Results()
        {
            var votes = _mongoService.GetVotes();

            var resultData = candidates.Select(c => new
            {
                Candidate = c,
                Votes = votes.Count(v => v.Candidate == c)
            }).ToList();

            ViewBag.Results = resultData;
            ViewBag.TotalVotes = votes.Count;
            ViewBag.LeadingParty = resultData
                .OrderByDescending(r => r.Votes)
                .FirstOrDefault()?.Candidate;

            return View();
        }

        [HttpGet]
        public JsonResult GetResults()
        {
            var votes = _mongoService.GetVotes();
            var resultData = candidates.Select(c => new
            {
                candidate = c,
                votes = votes.Count(v => v.Candidate == c)
            }).ToList();

            return Json(resultData);
        }

        // Download all votes as CSV
        [HttpGet]
        public IActionResult DownloadVotes()
        {
            var votes = _mongoService.GetVotes();

            var csv = new StringBuilder();
            csv.AppendLine("VoterName,Candidate,IP,VoteTime");

            foreach (var v in votes)
            {
                csv.AppendLine($"{v.VoterName},{v.Candidate},{v.IP},{v.VoteTime:yyyy-MM-dd HH:mm:ss}");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "votes.csv");
        }
    }
}
