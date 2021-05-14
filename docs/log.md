What is this?

---

It is a platform that will store and analyse data related to arrival and departure of trains. 

# Architecture
The vision, which can change in future, is captured within this diagram

![vision](./imgs/vision.svg)

We will capture the events with an Event Hub, then the data will be extracted by the Ingress and stored within Tables. This path represents our first increment.

Then there is another one which will focus on detection anomalies within ingested data and a notification system that lets us know should any anomaly gets detected. 

The last planned increment is to create front end that will show stored data and explain the anomalies.

# Technology Stack

- Azure
- ARM
- .net

# Data Source 

An easy way how to get sensible data is to use [Open Transport Tycoon Deluxe](openttd.org). I forked the repository (see [here](https://github.com/jaroslavknotek/OpenTTD/tree/feature/train_station_logging)) and plugged in a trivial logging that stores all arrival and departure events. 

The data look like this:
```json
{
    "stationId": "46",
    "stationName": "Abertown Woods",
    "vehicleId": "3",
    "unitNumber": "80",
    "date": "813593",
    "orderNumberCurrent": "0",
    "orderNumberTotal": "2",
    "type": "left"
}
```

I don't plan to have any central register of station or trained that will be managed separately therefore the incoming data are bit denormalized. 

There are information about station (`stationId`, `stationName`), about vehicle (`vehicleId`,`unitNumber` used for identification within OTTD) about the order (`orderNumberCurrent`, `orderNumberTotal`) and some about the event itself (`type`, `date`).

The `orderNumber` is a number of a record (could be station or a waypoint) within the timetable. We assume that the record will never change. The element `orderNumberCurrent` and `orderNumberTotal` represents the order when the event was fired and out of how many respectively. So far we track only stations therefore some order numbers may be missing.

# Requirements

- Azure account
- C# basics
- .NET 5 SDK
- Tools:
  - [Azure Function Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local#v2)
  - [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)


# Implementation

The project is limited by its costs - 130 Euro per month.

Create a new resource group and deploy the arm template `resources.azrm.json`.

```ps1
$resourceGroup = "train-platform-rg"
az group create --location 'WestEurope' -n $resourceGroup
az deployment group create `
   --name "deploy-trains" `
   --resource-group $resourceGroup `
   --template-file "arm/resources.azrm.json" `
   --parameters "arm/resources.azrm.parameters.json"
```

Create an azure function project.

```ps1
func init "AzureFunctions" --worker-runtime "dotnetIsolated"
func new --name "IngressEventHub" --template "eventhub"
```


TODO created Console Application for debug purposes

TODO tag the commit