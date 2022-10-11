#!/bin/bash

case "$1" in
    ""|"build")
        docker-compose up --build &
        exit 1;;
    "up")
        docker-compose up -d &
        exit 1;;
    "down")
        docker-compose down --remove-orphans &
        exit 1;;
    "strt")
        docker-compose start &
        exit 1;;
    "stop")
        docker-compose stop &
        exit 1;;
    "dev")
        docker-compose -f docker-compose-dev.yml down --remove-orphans
        docker-compose -f docker-compose-dev.yml up --build &
        exit 1;;
    "devtest")
        docker-compose -f docker-compose-integration-test.yml down --remove-orphans
        docker-compose -f docker-compose-integration-test.yml up --build &
        exit 1;;
    *)
        exit 1;;
esac
    