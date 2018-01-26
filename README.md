# hobknob-client-net

> A .net client library for [Hobknob](https://github.com/opentable/hobknob)

## Installation

Either use the Nuget user interface in Visual Studio, or type the following in to a compatible prompt:
```
Install-package hobknob-client-net
```

## Usage

```c#

var etcdHost = "localhost";
var etcdPort = 4001;
var applicationName = "radApp";
var cacheUpdateInterval = TimeSpan.FromSeconds(180);
EventHandler<CacheUpdateFailedArgs> cacheUpdateFailed = (o, args) => { throw args.Exception; }; 

var client = new HobknobClientFactory().Create(etcdHost, etcdPort, applicationName, cacheUpdateInterval, cacheUpdateFailed);

var toggleValue1 = client.GetOrDefault("Feature1", true); // Feature1 is false => false
var toggleValue2 = client.GetOrDefault("Feature2", true); // Feature2 does not exist => true

var toggleValue3 = client.GetOrDefault("DomainFeature1", "com", true); // Feature1/com exists => false
var toggleValue4 = client.GetOrDefault("DomainFeature1", "couk", true); // Feature1/couk does not exist => true

var allToggles = client.GetAll();
```

## API

### HobknobClientFactory().Create(string etcdHost, int etcdPort, string applicationName, TimeSpan cacheUpdateInterval)

Creates a new feature toggle client.

- `etcdHost` the host of the Etcd instance
- `etcdPort` the port of the Etcd instance
- `applicationName` the name of the application used to find feature toggles
- `cacheUpdateInterval` interval for the cache update, which loads all the applications toggles into memory
- `cacheUpdateFailed` delegate called whenever there is issue updating local hobknob cache. Must not be null for your own good.


### client.GetOrDefault(string featureName, bool defaultValue)

Gets the boolean value of a simple feature toggle if it exists, otherwise return the default value

- `featureName` the name of the feature, used with the application name to get the feature toggle value
- `defaultValue` the value to return if the feature does not exist

```c#
var featureToggleValue = client.GetOrDefault("feature1", false);
```

### client.GetOrDefault(string featureName, string toggleName, bool defaultValue)

Gets the boolean value of a multi feature toggle if it exists, otherwise return the default value

- `featureName` the name of the feature
- `toggleName` the name of the toggle (used when dealing with multi-features, e.g. `Feature1/com`)
- `defaultValue` the value to return if the feature toggle does not exist

```c#
var featureToggleValueForCom = client.GetOrDefault("DomainFeature1", "com", false);
```

### client.GetAll()

Gets the values for all features for the application

```c#
var allToggles = client.GetAll();
```

### client.CacheUpdated

An event which is raised on each cache update

```c#

client.CacheUpdated += (sender, eventArgs) => { console.Write("Updated"); }

```


### client.CacheUpdateFailed

An event which is raised when there is an error updating the cache. This is the same event as the one that is registered in Create method.

```c#

client.CacheUpdateFailed += (sender, eventArgs) => { console.Write(eventArgs.Exception.ToString()); }

```

## Building, testing and packaging from source

This project uses Docker Compose to build, test and create a Nuget package. Run the following in the command line:

```
docker-compose up --build --abort-on-container-exit
```

### Publishing package to Nuget

The grunt plug-in grunt-nuget is used to package and publish nuget packages. To publish this package, run the following:

```
grunt build
grunt nugetpush:dist
```

Notes:
1. The version number in the HobknobClientNet.nuspec file must be incremented on each publish.
2. You must specify a user auth key to publish to Nuget (see https://github.com/spatools/grunt-nuget/wiki/Key-Options)


## Etcd

Feature toggles are stored in Etcd using the following convention:
`http://host:port/v2/keys/v1/toggles/applicationName/featureName[/toggleName]`
