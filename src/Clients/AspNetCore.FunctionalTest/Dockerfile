#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Clients/AspNetCore.FunctionalTest/AspNetCore.FunctionalTest.csproj", "Clients/AspNetCore.FunctionalTest/"]
COPY ["Clients/AspNetCore/AspNetCore.csproj", "Clients/AspNetCore/"]
COPY ["Core/Transmission.EventHub/Transmission.EventHub.csproj", "Core/Transmission.EventHub/"]
COPY ["Core/Transmission.Abstractions/Transmission.Abstractions.csproj", "Core/Transmission.Abstractions/"]
COPY ["Core/Core.Abstractions/Core.Abstractions.csproj", "Core/Core.Abstractions/"]
COPY ["Core/Core/Core.csproj", "Core/Core/"]
COPY ["Clients/Http/Http.csproj", "Clients/Http/"]
COPY ["Core/Session.Abstractions/Session.Abstractions.csproj", "Core/Session.Abstractions/"]
COPY ["Core/Transmission.BlobStorage/Transmission.BlobStorage.csproj", "Core/Transmission.BlobStorage/"]
COPY ["Core/Session/Session.csproj", "Core/Session/"]
RUN dotnet restore "Clients/AspNetCore.FunctionalTest/AspNetCore.FunctionalTest.csproj"
COPY . .
WORKDIR "/src/Clients/AspNetCore.FunctionalTest"
RUN dotnet build "AspNetCore.FunctionalTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspNetCore.FunctionalTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Thor.Hosting.AspNetCore.FunctionalTest.dll"]