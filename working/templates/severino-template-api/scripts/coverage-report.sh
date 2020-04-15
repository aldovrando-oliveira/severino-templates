#!/bin/sh
set -x
set -e

cd "$(dirname "$0")/.."

echo '---> Executing tests with coverage.' >&2
./scripts/coverage.sh

if ! [ -x "$(command -v reportgenerator)" ]; then
  echo '---> Installing ReportGenerator.' >&2
  dotnet tool install -g dotnet-reportgenerator-globaltool
fi

VERSION=$(grep '<Version>' src/Severino.Template.Api/Severino.Template.Api.csproj | sed 's/.*<Version>\(.*\)<\/Version>/\1/')

reportgenerator -reports:./coverage.opencover.xml -targetdir:./coverage-report -reportTypes:htmlInline -tag:$VERSION
