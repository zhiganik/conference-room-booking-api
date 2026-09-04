# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ConferenceRoomBooking.Web/ConferenceRoomBooking.Web.csproj", "ConferenceRoomBooking.Web/"]
COPY ["ConferenceRoomBooking.Bll/ConferenceRoomBooking.Bll.csproj", "ConferenceRoomBooking.Bll/"]
COPY ["ConferenceRoomBooking.Bll.Common/ConferenceRoomBooking.Bll.Common.csproj", "ConferenceRoomBooking.Bll.Common/"]
COPY ["ConferenceRoomBooking.Dal.SqlRepositories/ConferenceRoomBooking.Dal.SqlRepositories.csproj", "ConferenceRoomBooking.Dal.SqlRepositories/"]
RUN dotnet restore "./ConferenceRoomBooking.Web/ConferenceRoomBooking.Web.csproj"
COPY . .
WORKDIR "/src/ConferenceRoomBooking.Web"
RUN dotnet build "./ConferenceRoomBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConferenceRoomBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConferenceRoomBooking.Web.dll"]
