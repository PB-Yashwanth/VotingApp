using VotingApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Voting}/{action=Index}/{id?}");

// Get port from environment (Railway will set this automatically)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5024";
app.Run($"http://0.0.0.0:{port}");
