#!/bin/sh
set -x
set -e

cd "$(dirname "$0")/.."

echo "Installing Dependencies"

apk add --no-cache --update \
    openjdk8-jre\
    nss

echo "Installing SonarQube"

dotnet tool install --global dotnet-sonarscanner

export PATH="$PATH:/root/.dotnet/tools"

VERSION=$(grep '<Version>' src/Severino.Template.Api/Severino.Template.Api.csproj | sed 's/.*<Version>\(.*\)<\/Version>/\1/')

dotnet sonarscanner begin \
    /d:sonar.login=$SONAR_LOGIN \
    /k:"severino-template-api" \
    /v:"$VERSION-$CI_COMMIT_SHORT_SHA" \
    /d:sonar.host.url=$SONAR_URL \
    /d:sonar.cs.opencover.reportsPaths="$(pwd)/coverage.opencover.xml" \
    /d:sonar.coverage.exclusions="\
        **Test*.cs,\
        **/Migrations/**,\
        **/Models/**,\
        **/Maps/**,\
        **/ViewModels/**,\
        **/Repositories/**,\
        **/*.Core/Jobs/**,\
        **/Extensions/**,\
        **/Exceptions/**,\
        **/Infra/**,\
        **/App_Start/**,\
        **/*Startup*,\
        **/Program.cs"  

dotnet build

dotnet sonarscanner end /d:sonar.login=$SONAR_LOGIN
