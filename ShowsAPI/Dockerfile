FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ShowsAPI/ShowsAPI.csproj", "ShowsAPI/"]
RUN dotnet restore "ShowsAPI/ShowsAPI.csproj"
COPY . .
WORKDIR "/src/ShowsAPI"
RUN dotnet build "ShowsAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShowsAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShowsAPI.dll"]