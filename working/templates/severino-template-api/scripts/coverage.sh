#!/bin/sh
set -x
set -e

cd "$(dirname "$0")/.."

if ! [ -x "$(command -v coverlet)" ]; then
  echo 'Installing Coverlet.' >&2
  dotnet tool install --global coverlet.console
  
  export PATH="$PATH:/root/.dotnet/tools"
fi

dotnet build

echo "--------------- COVERLET ----------------"

coverlet ./test/Severino.Template.Api.Tests/bin/Debug/netcoreapp3.1/Severino.Template.Api.Tests.dll \
    --target "dotnet" \
    --targetargs "test ./test/Severino.Template.Api.Tests/Severino.Template.Api.Tests.csproj --no-build" \
    --output "./coverage" \
    --exclude-by-file "**/Infra/**" \
    --exclude-by-file "**/Migrations/*" \
    --exclude-by-file "**/Models/**" \
    --exclude-by-file "**/ViewModels/**" \
    --exclude-by-file "**/Maps/**" \
    --exclude-by-file "**/Dto/**" \
    --exclude-by-file "**/Extensions/*" \
    --exclude-by-file "**/Exceptions/*" \
    --exclude-by-file "**/Repositories/**" \
    --exclude-by-file "**/Queue/*" \
    --exclude-by-file "**/*Extensions*" \
    --exclude-by-file "**/*Extension*" \
    --exclude-by-file "**/*Startup*" \
    --exclude-by-file "**/Program.cs" \
    --format "opencover"