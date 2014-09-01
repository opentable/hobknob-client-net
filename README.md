# hobknob-client-net

> A .net client library for Hobknob

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

var client = new HobknobClientFactory().Create(etcdHost, etcdPort, applicationName, cacheUpdateInterval);

var toggle1Status = client.Get("Toggle1"); // true
var toggle2Status = client.Get("ToggleThatDoesNotExist"); // throws exception
var toggle3Status = client.GetOrDefault("ToggleThatDoesNotExist", true); // true

```

## Etcd

Feature toggles are stored in Etcd using the following convention:
`http://host:port/v2/keys/v1/toggles/applicationName/toggleName`

## API

### HobknobClientFactory().Create(string etcdHost, int etcdPort, string applicationName, TimeSpan cacheUpdateInterval)

Creates a new feature toggle client.

- `etcdHost` the host of the Etcd instance
- `etcdPort` the port of the Etcd instance
- `applicationName` the name of the application used to find feature toggles
- `cacheUpdateInterval` interval for the cache update, which loads all the applications toggles into memory

### client.Get(string toggleName)

Gets the boolean value of a feature toggle if it exists, otherwise throw exception

- `toggleName` the name of the toggle, used with the application name to get the feature toggle value


### client.GetOrDefault(string toggleName, bool defaultValue)

Gets the boolean value of a feature toggle if it exists, otherwise return the default value

- `toggleName` the name of the toggle, used with the application name to get the feature toggle value
- `defaultValue` the value to return if the toggle value is not found


### client.CacheUpdated

An event which is raised on each cache update

```c#

client.CacheUpdated += (sender, eventArgs) => { console.Write("Updated"); }

```


### client.CacheUpdateFailed

An event which is raised when there is an error updating the cache.

```c#

client.CacheUpdateFailed += (sender, eventArgs) => { console.Write(eventArgs.Exception.ToString()); }

```