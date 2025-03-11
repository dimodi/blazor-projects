#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build command:
#
# docker build --secret id=telerik-license-key,src=/Users/dimov/.telerik/telerik-license.txt --secret id=telerik-nuget-key,src=/Users/dimov/.telerik/telerik-nuget.txt -t dockerblazorapp:v1 .
#
# Run command:
#
# docker run -p 8080:8080 dockerblazorapp:v1
#
# Browse at:
#
# http://localhost:8080
#

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY DockerBlazorApp/DockerBlazorApp.csproj DockerBlazorApp/
COPY NuGet.Config .
RUN --mount=type=secret,id=telerik-nuget-key \
    dotnet nuget update source TelerikOnlineFeed --username="api-key" --password="$(cat /run/secrets/telerik-nuget-key)" --store-password-in-clear-text --configfile ./NuGet.Config
RUN dotnet restore DockerBlazorApp/DockerBlazorApp.csproj
COPY . .
WORKDIR /src/DockerBlazorApp
RUN --mount=type=secret,id=telerik-license-key,env=TELERIK_LICENSE \
    dotnet build DockerBlazorApp.csproj -c Release -o /app/build

FROM build AS publish
RUN --mount=type=secret,id=telerik-license-key,env=TELERIK_LICENSE \
    dotnet publish DockerBlazorApp.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerBlazorApp.dll"]
