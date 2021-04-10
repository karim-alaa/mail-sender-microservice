#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RabbitMQ.Producer/RabbitMQ.Producer.csproj", "RabbitMQ.Producer/"]
RUN dotnet restore "RabbitMQ.Producer/RabbitMQ.Producer.csproj"
COPY . .
WORKDIR "/src/RabbitMQ.Producer"
RUN dotnet build "RabbitMQ.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitMQ.Producer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitMQ.Producer.dll"]