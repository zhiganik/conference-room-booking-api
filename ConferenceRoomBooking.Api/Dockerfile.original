FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConferenceRoomBooking.Api/ConferenceRoomBooking.Api.csproj", "ConferenceRoomBooking.Api/"]
COPY ["ConferenceRoomBooking.DataLayer/ConferenceRoomBooking.DataLayer.csproj", "ConferenceRoomBooking.DataLayer/"]
COPY ["ConferenceRoomBooking.Application/ConferenceRoomBooking.Application.csproj", "ConferenceRoomBooking.Application/"]
RUN dotnet restore "ConferenceRoomBooking.Api/ConferenceRoomBooking.Api.csproj"
COPY . .
WORKDIR "/src/ConferenceRoomBooking.Api"
RUN dotnet build "./ConferenceRoomBooking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConferenceRoomBooking.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConferenceRoomBooking.Api.dll"]
