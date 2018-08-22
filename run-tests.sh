dotnet build src
dotnet test src/Core.Abstractions.Tests --no-build
dotnet test src/Core.Tests --no-build
dotnet test src/Http.Tests --no-build
dotnet test src/Session.Abstractions.Tests --no-build
dotnet test src/Session.Tests --no-build
dotnet test src/Transmission.Abstractions.Tests --no-build
dotnet test src/Transmission.Tests --no-build
dotnet test src/Transmission.BlobStorage.Tests --no-build
dotnet test src/Transmission.EventHub.Tests --no-build
