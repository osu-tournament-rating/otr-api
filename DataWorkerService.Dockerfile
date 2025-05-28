# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

# Stage 2: Build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ENV HUSKY=0
WORKDIR /src

# Copy project files and dependencies
COPY ["otrAPI.sln", "./"]

COPY ["API/API.csproj", "./API/"]
COPY ["API.Tests/API.Tests.csproj", "./API.Tests/"]
COPY ["API.Utils.Jwt/API.Utils.Jwt.csproj", "./API.Utils.Jwt/"]
COPY ["Common/Common.csproj", "./Common/"]
COPY ["Database/Database.csproj", "./Database/"]
COPY ["DataWorkerService/DataWorkerService.csproj", "./DataWorkerService/"]
COPY ["DataWorkerService.Tests/DataWorkerService.Tests.csproj", "./DataWorkerService.Tests/"]
COPY ["OsuApiClient/OsuApiClient.csproj", "./OsuApiClient/"]
COPY ["OsuApiClient.Tests/OsuApiClient.Tests.csproj", "./OsuApiClient.Tests/"]
COPY ["TestingUtils/TestingUtils.csproj", "./TestingUtils/"]

RUN dotnet restore "otrAPI.sln"

COPY . .

RUN dotnet build "DataWorkerService/DataWorkerService.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "DataWorkerService/DataWorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image
FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataWorkerService.dll"]