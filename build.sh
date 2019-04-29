#!/usr/bin/env bash
set -euo pipefail
dotnet tool install --tool-path tools SignClient

dotnet run --project build -- "$@"