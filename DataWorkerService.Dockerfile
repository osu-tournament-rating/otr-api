FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ENV HUSKY=0
WORKDIR /src
COPY ["DataWorkerService/DataWorkerService.csproj", "DataWorkerService/"]
RUN dotnet restore "DataWorkerService/DataWorkerService.csproj"
COPY . .
WORKDIR "/src/DataWorkerService"
RUN dotnet build "DataWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataWorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataWorkerService.dll"]