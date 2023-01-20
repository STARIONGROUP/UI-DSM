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


## Build and deploy

The web application is distributed using docker containers and docker-compose.

In the first stage the application is built using the private DevExpress nuget feed and the private Generic Platform feed. In order to access this nuget feed, it is required to EXPORT the API-KEYs to an environment variable through secret file.

### Build

#### Secret File
Create a file `.env` with the following content
```
DEVEXPRESS_NUGET_KEY=<YOUR-DEVEXPRESS-KEY>
GP_NUGET_USER=<YOUR-GP-USER>
GP_NUGET_TOKEN=<YOUR-GP-TOKEN>
```

#### Linux
```
$ ./solutionFolder# export BUILDKIT=1
$ ./solutionFolder# docker build --secret id=envConfig,src=.env -f Dockerfile -t rheagroup/ui-dsm:latest -t rheagroup/ui-dsm:%1 .
```

#### Windows
```
$ ./solutionFolder# set DEVEXPRESS_NUGET_KEY=<YOUR-API-KEY>
$ ./solutionFolder# set BUILDKIT=1
$ ./solutionFolder# docker build --secret id=envConfig,src=.env -f Dockerfile -t rheagroup/ui-dsm:latest -t rheagroup/ui-dsm:%1 .
```

### Deploy

```
$ ./solutionFolder# docker push rheagroup/ui-dsm:lastest
$ ./solutionFolder# docker push rheagroup/ui-dsm:<version>
```

### Development Environment
Under a development environment, running the 'docker-compose-dev.yml' compose file is enough

## Build Status

GitHub actions are used to build and test the libraries

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/RHEAGROUP/UI-DSM/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/RHEAGROUP/UI-DSM/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

# License

The UI-DSM application is provided to the community under the Apache License 2.0.
