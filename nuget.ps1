cd ChatBot
# ..\..\..\NuGet-Signed.exe spec
..\..\..\NuGet-Signed.exe pack ChatBot.csproj

# UPLOAD
# ..\..\..\nuget setApiKey Your-API-Key
# ..\..\..\nuget push ChatBot.1.0.0.0.nupkg