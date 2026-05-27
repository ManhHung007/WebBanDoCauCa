# Giai doan build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
# Chỉ định rõ project cần publish
RUN dotnet publish "WebBanDoCauCa/WebBanDoCauCa.csproj" -c Release -o /app

# Giai doan runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
# Copy kết quả đã publish từ folder /app của bước build
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebBanDoCauCa.dll"]