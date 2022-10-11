@echo off

IF %1.==. GOTO Build
IF %1==build GOTO Build
IF %1==up GOTO Up
IF %1==down GOTO Down
IF %1==strt GOTO Strt
IF %1==stp GOTO Stp
IF %1==dev GOTO Dev
IF %1==devtest GOTO DevTest

GOTO End

:Build
START /B docker-compose up --build
GOTO End

:Strt
START /B docker-compose start
GOTO End

:Stp
START /B docker-compose stop
GOTO End

:Up
START /B docker-compose up -d
GOTO End

:Down
START /B docker-compose down --remove-orphans
GOTO End

:Dev
START /B docker-compose -f docker-compose-dev.yml down --remove-orphans
START /B docker-compose -f docker-compose-dev.yml up --build
GOTO End

:DevTest
START /B docker-compose -f docker-compose-integration-test.yml down --remove-orphans
START /B docker-compose -f docker-compose-integration-test.yml up --build
GOTO End

:End