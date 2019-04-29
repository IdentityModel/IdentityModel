$ErrorActionPreference = "Stop";
dotnet tool install --tool-path tools SignClient

dotnet run --project build -- $args