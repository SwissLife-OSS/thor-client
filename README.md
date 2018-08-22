![GreenDonut](https://cdn.rawgit.com/ChilliCream/thor-logo/master/img/thor-banner-light.svg)

[![GitHub release](https://img.shields.io/github/release/ChilliCream/thor-core.svg)](https://github.com/ChilliCream/thor-core/releases) [![NuGet Package](https://img.shields.io/nuget/v/Thor.Core.svg)](https://www.nuget.org/packages/Thor.Core/) [![License](https://img.shields.io/github/license/ChilliCream/thor-core.svg)](https://github.com/ChilliCream/thor-core/releases) [![Build](https://ci.appveyor.com/api/projects/status/u7uk31dyjp399m59/branch/master?svg=true)](https://ci.appveyor.com/project/rstaib/thor-core/branch/master) [![Tests](https://img.shields.io/appveyor/tests/rstaib/thor-core/master.svg)](https://ci.appveyor.com/project/rstaib/thor-core) [![Coverage Status](https://sonarcloud.io/api/project_badges/measure?project=ThorCore&metric=coverage)](https://sonarcloud.io/dashboard?id=ThorCore) [![Quality](https://sonarcloud.io/api/project_badges/measure?project=ThorCore&metric=alert_status)](https://sonarcloud.io/dashboard?id=ThorCore) [![better code](https://bettercodehub.com/edge/badge/ChilliCream/thor-core)](https://bettercodehub.com/results/ChilliCream/thor-core)

---

**The _Thor_ tracing core for _.net_**

The tracing core contains everything to enable _ETW_ tracing in your _.net_ application.

## Getting Started

### Install Package

Here is how the `Thor.Core` has to be installed via _NuGet_ on a powershell console.

```powershell
Install-Package Thor.Core
```

### Code Example

Okay, after installing our dependencies we could start writing a bit code.

#### How to scope (group) events together

The following code is not production ready. This code should give only an idea of how to group _ETW_
events into an activity.

```csharp
using System;
using System.Threading;
using Thor.Core;
using static Thor.Core.DefaultEventSource;

public class UserRepository
{
    ...

    public async Task<bool> RemoveAsync(User user)
    {
        using (Activity.Create("Remove User"))
        {
            try
            {
                Log.Info($"Removing user with id {user.Id}");

                if (await _collection.RemoveAsync(user.Id).ConfigureAwait(false))
                {
                    Log.Info($"Removed user with id {user.Id} successfully");

                    return true;
                }
                else
                {
                    Log.Info($"User with id {user.Id} not found");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Info(ex, $"Removing user with id {user.Id} failed");

                return false;
            }
        }
    }

    ...
}
```

## Documentation

Click [here](https://github.com/ChilliCream/thor-core-docs) to get to the documentation home of _Thor Core_.

## Checkout the Thor Generator

We strongly recommend to use the _Thor Generator_ to generate your own event sources automatically
and avoid using the `DefaultEventSource`, because _Semantic Tracing_ is much more powerful for you
in the end. Click [here](https://github.com/ChilliCream/thor-generator) to get more information
about the _Thor Generator_.
