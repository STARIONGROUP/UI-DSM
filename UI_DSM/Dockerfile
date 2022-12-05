#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["UI_DSM.Server/UI_DSM.Server.csproj", "UI_DSM.Server/"]
COPY ["UI_DSM.Client/UI_DSM.Client.csproj", "UI_DSM.Client/"]
COPY ["UI_DSM.Serializer.Json/UI_DSM.Serializer.Json.csproj", "UI_DSM.Serializer.Json/"]
COPY ["UI_DSM.Shared/UI_DSM.Shared.csproj", "UI_DSM.Shared/"]

RUN --mount=type=secret,id=DEVEXPRESS_NUGET_KEY export DEVEXPRESS_NUGET_KEY=$(cat /run/secrets/DEVEXPRESS_NUGET_KEY) \
 && dotnet nuget add source https://nuget.devexpress.com/api -n DXFeed -u DevExpress -p ${DEVEXPRESS_NUGET_KEY} --store-password-in-clear-text \
 && dotnet restore "UI_DSM.Server/UI_DSM.Server.csproj"

COPY . .
WORKDIR "/src/UI_DSM.Server"
RUN dotnet build "UI_DSM.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UI_DSM.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UI_DSM.Server.dll"]