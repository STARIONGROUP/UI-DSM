#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["UI_DSM.Server", "UI_DSM.Server/"]
COPY ["UI_DSM.Client", "UI_DSM.Client/"]
COPY ["UI_DSM.Serializer.Json", "UI_DSM.Serializer.Json/"]
COPY ["UI_DSM.Shared", "UI_DSM.Shared/"]

RUN --mount=type=secret,id=envConfig . /run/secrets/envConfig \
 && dotnet nuget add source https://nuget.devexpress.com/api -n DXFeed -u DevExpress -p ${DEVEXPRESS_NUGET_KEY} --store-password-in-clear-text \ 
 && dotnet nuget add source https://gitlab.esa.int/api/v4/projects/7524/packages/nuget/index.json -n GPFeed -u ${GP_NUGET_USER} -p ${GP_NUGET_TOKEN} --store-password-in-clear-text \ 
 && dotnet restore "UI_DSM.Server/UI_DSM.Server.csproj"
 
WORKDIR "/src/UI_DSM.Server"
RUN dotnet build --no-restore "UI_DSM.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UI_DSM.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["UI_DSM.Server/Reports", "storage/"]

RUN apt update
RUN apt install -y libc6 -f -o APT::Immediate-Configure=0 
RUN apt install -y libicu-dev libharfbuzz0b libfontconfig1 libfreetype6 libosmesa6 libglu1-mesa

ENTRYPOINT ["dotnet", "UI_DSM.Server.dll"]