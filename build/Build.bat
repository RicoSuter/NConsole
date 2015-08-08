nuget restore ../src/NConsole.sln
msbuild ../src/NConsole.sln /p:Configuration=Release /t:rebuild
nuget pack ../src/NConsole/NConsole.csproj -OutputDirectory "Packages" -Prop Configuration=Release
