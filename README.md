# Introduction

<a href="https://www.nuget.org/packages/Hic.Json.MSBuild/"><img alt="NuGet version" src="https://img.shields.io/nuget/v/Hic.Json.MSBuild.svg?svg=true"></img>|<img alt="NuGet version" src="https://img.shields.io/nuget/dt/Hic.Json.MSBuild.svg?svg=true"></img></a>

This repository contains a set of MSBuild tasks for handling JSON in the build process. These tasks are not intended to replace existing tasks such as JsonPeek and JsonPoke, but at the time of writing no alternatives could be found.

Due to the support of the JsonSchema.Net package, only recent drafts of json schema are supported.

## Build and Test

`dotnet restore --interactive`

`dotnet build`

`dotnet test`

## Adding new projects

`dotnet new`
