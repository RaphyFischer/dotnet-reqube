# dotnet-reqube

[![Build status](https://ci.appveyor.com/api/projects/status/kb0260n7o1alqyqv?svg=true)](https://ci.appveyor.com/project/olsh/reqube)
[![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=dotnet-reqube&metric=alert_status)](https://sonarcloud.io/dashboard?id=dotnet-reqube)
[![NuGet](https://img.shields.io/nuget/v/dotnet-reqube.svg)](https://www.nuget.org/packages/dotnet-reqube/)

.NET Core global tool that converts ReSharper inspect code report to SonarQube format

## Installation

`dotnet tool install --global dotnet-reqube`

## Usage

`dotnet-reqube -i ResharperReport.xml -o SonarQubeReportFileName.json -d Path\To\Output\Directory`

## Demo

Example project that shows how you can import ReSharper issues to SonarQube using the `dotnet-reqube`
https://github.com/olsh/resharper-to-sonarqube-example