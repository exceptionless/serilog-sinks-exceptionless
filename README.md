Serilog.Sinks.ExceptionLess
===========================

Serilog sink to publish to ExceptionLess

Version 1.1
-----------
Added the ability to add additional ExceptionLess configuration when configuring Serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.With(new HttpRequestIdEnricher())
    .WriteTo.Glimpse()
    .WriteTo.ExceptionLess(builder => builder.AddRequestInfo())
    .CreateLogger();
```