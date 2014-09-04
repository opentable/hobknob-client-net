# hobknob-client-net

> A .net client library for Hobknob

## Installation

Either use the Nuget user interface in Visual Studio, or type the following in to a compatible prompt:
```
Install-package hobknob-client-net
```

## Building from source

This project uses Grunt to build and create a Nuget package. Run the following in the command line:

```
npm install
grunt build
```

There is a file called HobknobClientNet.nuspec which is used to configure the Nuget package. See http://docs.nuget.org/ for more information.

### Testing

There are end-to-end tests which can be run via grunt.

> These tests require Etcd to be installed locally on port 4001 (this can be done via Vagrant by executing `vagrant up` in the root directory).

```
grunt test
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

An event which is raised when there is an error updating the cache

```c#

client.CacheUpdateFailed += (sender, eventArgs) => { console.Write(eventArgs.Exception.ToString()); }

```