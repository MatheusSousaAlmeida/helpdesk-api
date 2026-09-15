FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HelpDesk.Api/HelpDesk.Api.csproj", "HelpDesk.Api/"]
RUN dotnet restore "HelpDesk.Api/HelpDesk.Api.csproj"
COPY . .
WORKDIR "/src/HelpDesk.Api"
RUN dotnet build "HelpDesk.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HelpDesk.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelpDesk.Api.dll"]
