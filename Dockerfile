FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/OrderFlow.Api/OrderFlow.Api.csproj", "src/OrderFlow.Api/"]
COPY ["src/Orderflow.Application/Orderflow.Application.csproj", "src/Orderflow.Application/"]
COPY ["src/OrderFlow.Domain/OrderFlow.Domain.csproj", "src/OrderFlow.Domain/"]
COPY ["src/OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj", "src/OrderFlow.Infrastructure/"]
RUN dotnet restore "src/OrderFlow.Api/OrderFlow.Api.csproj"

COPY . .
WORKDIR /src/src/OrderFlow.Api
RUN dotnet publish "OrderFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .
RUN mkdir -p /app/Logs && chown -R $APP_UID:$APP_UID /app
USER $APP_UID

ENTRYPOINT ["dotnet", "OrderFlow.Api.dll"]
