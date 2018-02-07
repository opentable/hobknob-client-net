FROM mono:latest

LABEL maintainer="James Hay"
LABEL version="1.0"

LABEL description="Builds and publishes the hobknob c# client"

#This will be set by the command line
ARG version
ENV version=$version

COPY . /app
WORKDIR /app

#Restore test dependencies
RUN msbuild /t:restore hobknob-client-net.sln

#Build for test release
RUN msbuild /p:Configuration=Release /p:version=$version hobknob-client-net.sln

#Test and create nuget package
CMD mono packages/nunit.consolerunner/3.7.0/tools/nunit3-console.exe test/bin/Release/net451/HobknobClientNet.Tests.dll && \
 msbuild /t:pack /p:Configuration=Release /p:PackageOutputPath=/opt/nuget_package /p:Version=$version /p:ApplicationVersion=$version src/HobknobClientNet.csproj
