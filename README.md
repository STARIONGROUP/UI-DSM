# UI-DSM

The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=coverage)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_UI-DSM&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_UI-DSM)


## Installation

The web application is distributed using docker containers and docker-compose.

In the first stage the application is built using the private DevExpress nuget feed. In order to access this nuget feed, it is required to EXPORT the API-KEY to an environment variable.

A script is available to run docker commands

### Linux
```
$ ./solutionFolder# export DEVEXPRESS_NUGET_KEY=<YOUR-API-KEY>
$ ./solutionFolder# ./compose.sh
```

### Windows
```
$ ./solutionFolder# set DEVEXPRESS_NUGET_KEY=<YOUR-API-KEY>
$ ./solutionFolder# ./compose.bat <option>
```

Available options : 

- `build` - (default) builds the solution, creates the images and runs the containers.
- `strt` - starts the containers if they already have been run and stopped.
- `stp` - stops the running containers without removing them.
- `up` - runs containers without rebuilding them.
- `down` - stops and removes the containers. Volume information is not lost.
- `dev` - creates database containers. Does not required to set the DEVEXPRESS_NUGET_KEY.
- `devtest` - creates database and server containers. 

## Build Status

GitHub actions are used to build and test the libraries

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/RHEAGROUP/UI-DSM/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/RHEAGROUP/UI-DSM/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

# License

The UI-DSM application is provided to the community under the Apache License 2.0.
