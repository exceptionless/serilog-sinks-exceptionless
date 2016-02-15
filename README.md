Serilog.Sinks.Exceptionless
===========================

[![Build status](https://ci.appveyor.com/api/projects/status/bvmfe8muijhgkb9j?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-exceptionless)

Serilog sink to publish to Exceptionless

```
PM> Install-Package Serilog.Sinks.Exceptionless
``` 

Read the Exceptionless configuration
```csharp
using Exceptionless;
ExceptionlessClient.Default.Register();
```

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.With(new HttpRequestIdEnricher())
    .WriteTo.Exceptionless(b => b.AddTags("Cart").AddObject(order).AddRequestInfo())
    .CreateLogger();
```
