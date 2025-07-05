# DotNetCoreBaseAPI
This class library can be used as base api to import various basic configuration for consumer api.
Dot Net Version: 8.0

# Instructions for consumer APIs
# Healthchecks
  Provide heath check configurations in consumer's appSettings.json in below format.
  "HealthChecks":{
  "UriGroup":"<Custome uri group url for URi heal check>",
  "RemoteUris":[
    "List of remote uris for remote endpoints health checks"
    ]
  }
