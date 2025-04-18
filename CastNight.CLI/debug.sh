rm -rf HelloWorld
rm -rf ~/.castnight/
dotnet run install
castnight create HelloWorld desktop
cd HelloWorld
dotnet add reference ../../CastNight
