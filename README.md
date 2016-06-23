#Serilog.Sinks.Exceptionless

[![Build status](https://ci.appveyor.com/api/projects/status/bvmfe8muijhgkb9j?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-exceptionless)

Serilog sink to publish to Exceptionless

```
PM> Install-Package Serilog.Sinks.Exceptionless
``` 

Read the Exceptionless configuration
```csharp
using Exceptionless;
Exceptionless.ExceptionlessClient.Default.Startup();
```

```csharp
var log = new LoggerConfiguration()
    .Enrich.With(new HttpRequestIdEnricher())
    .WriteTo.Exceptionless(b => b.AddTags("Cart").AddObject(order).AddRequestInfo())
    .CreateLogger();
```

* [Documentation](https://github.com/serilog/serilog/wiki)

Copyright &copy; 2016 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html).
