# build application using dotnet sdk
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# using aspnet runtime for a more lightweight image
# copy and use PlatformService dll into folder
# dlls are linked and used in aspnet runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "PlatformService.dll", "--environment=Production" ]
