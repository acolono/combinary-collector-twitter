# Combinary Twitter Collector

## Prerequisites

- [docker](http://get.docker.com)
- [docker-compose](https://docs.docker.com/compose/install/)

## Setup

- Create a [twitter app](https://apps.twitter.com).
- Generate access tokens.
- Set your credentials in `.env` - all `TWITTER_*` variables

You can copy the [`.env` example file](src/.env-example) and fill in the missing values

## Start
```sh
docker-compose -f src/docker-compose.yml up -d
```

## Usage
The example setup also contains kibana which can be used to analyze the collected data.  
Which will be accessible via `localhost:8789`  
To get started there is also a example [kibana configuration](src/kibana-twitter-example.json) which can be imported.  

A complete list of services can be found in the [docker-compose.yml file](src/docker-compose.yml).

[![Build Status](https://dev.azure.com/volatile-void/pipes/_apis/build/status/acolono.combinary-collector-twitter)](https://dev.azure.com/volatile-void/pipes/_build/latest?definitionId=5)
