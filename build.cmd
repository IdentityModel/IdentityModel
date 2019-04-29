@echo off
dotnet tool install --tool-path tools SignClient

dotnet run --project build -- %*