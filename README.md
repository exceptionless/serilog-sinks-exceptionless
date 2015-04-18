Serilog.Sinks.ExceptionLess
===========================

[![Build status](https://ci.appveyor.com/api/projects/status/bvmfe8muijhgkb9j?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-exceptionless)

Serilog sink to publish to ExceptionLess

```
PM> Install-Package Serilog.Sinks.ExceptionLess
``` 

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.With(new HttpRequestIdEnricher())
    .WriteTo.ExceptionLess(b => b.AddTags("Cart").AddObject(order).AddRequestInfo())
    .CreateLogger();
```
