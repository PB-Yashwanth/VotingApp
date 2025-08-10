# Use the official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY VotingApp/*.csproj ./VotingApp/
RUN dotnet restore VotingApp/VotingApp.csproj

# Copy everything and build
COPY . ./
RUN dotnet publish VotingApp/VotingApp.csproj -c Release -o /app

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Railway sets PORT dynamically
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VotingApp.dll"]
