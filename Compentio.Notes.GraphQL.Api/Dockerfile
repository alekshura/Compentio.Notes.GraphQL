#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Compentio.Notes.GraphQL.Api/Compentio.Notes.GraphQL.Api.csproj", "Compentio.Notes.GraphQL.Reports/"]
RUN dotnet restore "Compentio.Notes.GraphQL.Api/Compentio.Notes.GraphQL.Api.csproj"
COPY . .
WORKDIR "/src/Compentio.Notes.GraphQL.Api"
RUN dotnet build "Compentio.Notes.GraphQL.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Compentio.Notes.GraphQL.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Compentio.Notes.GraphQL.Api.dll"]