# Serilog.Sinks.Exceptionless

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
Log.Logger = new LoggerConfiguration()
    .WriteTo.Exceptionless(b => b.AddTags("ASP.NET Core Example Logger"))
    .CreateLogger();

var log = Log.ForContext<HomeController>();
log.Information("Info Log that also contains HttpContext request info and default tags");
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
