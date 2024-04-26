Remove-Item -Recurse -Force .\x64\Release\** -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\x86\Release\** -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\TqkLibrary.AudioPlayer.Sdl2\bin\Release\** -ErrorAction SilentlyContinue

$env:PATH="$($env:PATH);C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE"
devenv .\TqkLibrary.AudioPlayer.Sdl2.sln /Rebuild 'Release|x64' /Project TqkLibrary.AudioPlayer.Sdl2.Native
devenv .\TqkLibrary.AudioPlayer.Sdl2.sln /Rebuild 'Release|x86' /Project TqkLibrary.AudioPlayer.Sdl2.Native

dotnet build --no-incremental .\TqkLibrary.AudioPlayer.Sdl2\TqkLibrary.AudioPlayer.Sdl2.csproj -c Release
nuget pack .\TqkLibrary.AudioPlayer.Sdl2\TqkLibrary.AudioPlayer.Sdl2.nuspec -Symbols -OutputDirectory .\TqkLibrary.AudioPlayer.Sdl2\bin\Release

$localNuget = $env:localNuget
if(![string]::IsNullOrWhiteSpace($localNuget))
{
    Copy-Item .\TqkLibrary.AudioPlayer.Sdl2\bin\Release\*.nupkg -Destination $localNuget -Force
}

$nugetKey = $env:nugetKey
if(![string]::IsNullOrWhiteSpace($nugetKey))
{
    Write-Host "Enter to push nuget"
    pause
    Write-Host "enter to confirm"
    pause
    $files = [System.IO.Directory]::GetFiles(".\TqkLibrary.AudioPlayer.Sdl2\bin\Release\")
    iex "nuget push $($files[0]) -ApiKey $($nugetKey) -Source https://api.nuget.org/v3/index.json"
}