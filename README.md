# Serilog.Sinks.Exceptionless

[![Build Windows](https://github.com/exceptionless/serilog-sinks-exceptionless/workflows/Build%20Windows/badge.svg?branch=master)](https://github.com/Exceptionless/serilog-sinks-exceptionless/actions)
[![Build OSX](https://github.com/exceptionless/serilog-sinks-exceptionless/workflows/Build%20OSX/badge.svg)](https://github.com/Exceptionless/serilog-sinks-exceptionless/actions)
[![Build Linux](https://github.com/exceptionless/serilog-sinks-exceptionless/workflows/Build%20Linux/badge.svg)](https://github.com/Exceptionless/serilog-sinks-exceptionless/actions)
[![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Exceptionless.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Exceptionless/)

## Getting started

To use the Exceptionless sink, first install the [NuGet package](https://www.nuget.org/packages/Serilog.Sinks.Exceptionless/):

```powershell
Install-Package Serilog.Sinks.Exceptionless
```

Next, we need to ensure that Exceptionless is configured with an API Key. If you are
already using Exceptionless you can skip this step.

The Exceptionless sink will use the default `ExceptionlessClient` client instance. This ensures
that all of your Exceptionless configuration is shared with the sink and also enables logging
of unhandled exceptions to Exceptionless.

> For advanced users who wish to configure the sink to use custom `ExceptionlessClient` instance
> you can provide an API Key or `ExceptionlessClient` instance to `WriteTo.Exceptionless()`.

```csharp
using Exceptionless;
ExceptionlessClient.Default.Startup("API_KEY");
```

Next, enable the sink using `WriteTo.Exceptionless()`

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Exceptionless(b => b.AddTags("Serilog Example"))
    .CreateLogger();
```

To get tags to populate on the exceptionless UI, add a `Tags` string enumerable to any log.

```Example with Serilog: 
Serilog: Log.ForContext("Tags", new List() { "Tag1", "Tag2"}).Information("Seri info");
```

```Example with ILogger
using (var scope = _logger.BeginScope(new Dictionary<string, object> { ["Tags"] = new string[] { "Tag1", "Tag2" }}))
{
_logger.Log(logLevel, eventId, state, exception, formatter);
}
```

* [Documentation](https://github.com/serilog/serilog/wiki)

Copyright &copy; 2023 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html).
