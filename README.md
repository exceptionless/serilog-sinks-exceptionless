# Serilog.Sinks.Exceptionless

[![Build status](https://ci.appveyor.com/api/projects/status/bvmfe8muijhgkb9j?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-exceptionless)


### Getting started

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

SetUserIdentity
```csharp
 public class LogUserNameMiddleware
    {
        private readonly RequestDelegate next;

        public LogUserNameMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var identity = context.User.Identity;

            if (identity.IsAuthenticated)
            {
                LogContext.PushProperty(Exceptionless.Models.Event.KnownDataKeys.UserInfo, new Exceptionless.Models.Data.UserInfo(identity.GetClaimValue("email"), identity.GetClaimValue("username")), true);
                //or
                //LogContext.PushProperty(Exceptionless.Models.Event.KnownDataKeys.UserDescription, new Exceptionless.Models.Data.UserDescription(identity.GetClaimValue("email"), identity.GetClaimValue("username")), true);
            }

            return next(context);
        }
    }

    app.UseMiddleware<LogUserNameMiddleware>();

```

* [Documentation](https://github.com/serilog/serilog/wiki)

Copyright &copy; 2017 Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html).
