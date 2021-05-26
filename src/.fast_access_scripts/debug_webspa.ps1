# Run all projects using dotnet and open the WebSPA in VS Code

start powershell {cd ../Services/Identity/Identity.API; dotnet run}
start powershell {cd ../Services/ConferenceManagement/Strive; dotnet run}
start powershell {cd ../Services/SFU; yarn dev}
start powershell {cd ../Web/WebSPA; dotnet run}
start code ../Web/WebSPA/Client