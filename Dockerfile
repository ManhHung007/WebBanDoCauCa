# Giai doan build
FROM mcr.microsoft.comdotnetsdk8.0 AS build
WORKDIR src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o app

# Giai doan runtime
FROM mcr.microsoft.comdotnetaspnet8.0
WORKDIR app
COPY --from=build app .
ENTRYPOINT [dotnet, WebBanDoCauCa.dll]