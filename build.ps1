dotnet tool install --tool-path tools SignClient

$ErrorActionPreference = "Stop";
dotnet run --project build -- $args