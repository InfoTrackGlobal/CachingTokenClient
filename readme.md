# OAuth 2.0 TokenClient & CachingTokenClient

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

- OAuth 2.0 Client Library for .NET
- Supports >= .NET Core 1.1 and .NET Framework 4.5.2
- Provides implementions which automatically cache authentication server responses
- Supports the following grant types
  - password
  - client_credentials
  - refresh_token

This repository contains the source code for the `TokenClient` and `CachingTokenClient` which is an OAuth2.0 client for .NET. The caching version automatically caches the authentication server response in memory on the server.

## Contents

- [Cache Expiry](#cache-expiry)
- [.NET Core](#net-core)
- [.NET Framework](#net-framework)
- [Options](#options)
- [Non-Caching Version](#non-caching-version)

## Cache Expiry
If the [server response](https://www.oauth.com/oauth2-servers/access-tokens/access-token-response/) includes a value for `expires_in`, the response is cached up until this value. If the server does not include this value, it is cached for 24 hours, though this can be overridden using the `DefaultCacheExpiry` option.

> Note: If you are using a refresh token, you should store this separately, and use it to get a new token once your access token expires.


## .NET Core

- Requires >= .NET Core 1.1

Please use the .NET Standard package for .NET Core applications.

### Usage

1. Install the `InfoTrack.OAuth.Caching.DotNetCore` NuGet package.
```
PM> Install-Package InfoTrack.OAuth.Caching.DotNetCore
```

2. Register the .NET Core Memory Cache in your application during startup when configuring your services. You can also optionally register the `CachingTokenClient` for dependency injection.

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddMemoryCache();

    services.AddSingleton<ITokenClient, CachingTokenClient>();
}
```

3. Whenever you need a token, inject an ITokenClient, and call a token method. It will return the cached token unless it has expired, at which point it will grab a new one.

```C#
var tokenResponse = await tokenClient.ClientCredentialsGrantAsync(
    new Uri("https://authenticate.me/connect/token"),
    "client_id",
    "client_secret"
    );
```
You may register the `CachingTokenClient` with any dependency lifecycle as it is fully thread safe.

5. Profit 🤑


## .NET Framework

- Requires >= .NET Framework 4.5.2

This package contains a .NET Framework implementation of the `CachingTokenClient`. Internally this uses `System.Runtime.Caching.MemoryCache`.

1. Install the `InfoTrack.OAuth.Caching.DotNetFramework` NuGet package.

```
PM> Install-Package InfoTrack.OAuth.Caching.DotNetFramework
```

2. Optionally register the `CachingTokenClient` for dependency injection.

```C#
// Ninject example
_kernel.Bind<ITokenClient>().To<CachingTokenClient>();
```

3. Whenever you need a token, inject an ITokenClient, and call a token method. It will return the cached token unless it has expired, at which point it will grab a new one.

```C#
var tokenResponse = await tokenClient.ClientCredentialsGrantAsync(
    new Uri("https://authenticate.me/connect/token"),
    "client_id",
    "client_secret"
    );
```
You may register the `CachingTokenClient` with any dependency lifecycle as it is fully thread safe.

## .NET Standard

- Requires >= .NET Standard 2.0

This package contains a .NET Standard  implementation of the `CachingTokenClient`. Internally this uses `System.Runtime.Caching.MemoryCache`.

1. Install the `InfoTrack.OAuth.Caching.DotNetStandard` NuGet package.

```
PM> Install-Package InfoTrack.OAuth.Caching.DotNetStandard
```

2. Optionally register the `CachingTokenClient` for dependency injection.

```C#
// Ninject example
_kernel.Bind<ITokenClient>().To<CachingTokenClient>();
```

3. Whenever you need a token, inject an ITokenClient, and call a token method. It will return the cached token unless it has expired, at which point it will grab a new one.

```C#
var tokenResponse = await tokenClient.ClientCredentialsGrantAsync(
    new Uri("https://authenticate.me/connect/token"),
    "client_id",
    "client_secret"
    );
```
You may register the `CachingTokenClient` with any dependency lifecycle as it is fully thread safe.

## Non-Caching Version
If all you need is a basic token client with very few dependencies, you can use the base package on its own.

```
PM> Install-Package InfoTrack.OAuth
```

## Options
There are constructors for the `CachingTokenClient` which allow you to pass a `ClientOptions` object.

| Property | Default | Remarks |
| --- | --- | --- |
| DefaultCacheExpiry | 86400 | Time in seconds to cache the response if it does not contain a value for `expires_in`