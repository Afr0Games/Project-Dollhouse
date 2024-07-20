# First stage: build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj files and restore as distinct layers
COPY server/Dollhouse.Server.csproj ./server/
COPY libs/TSOProtocol/TSOProtocol.csproj ./libs/TSOProtocol/
COPY libs/Files/Files.csproj ./libs/Files/
COPY libs/Vitaboy/Vitaboy.csproj ./libs/Vitaboy/
RUN dotnet restore server/Dollhouse.Server.csproj

# Copy everything else and build
COPY server/ ./server/Dollhouse.Server/
COPY libs/TSOProtocol/ ./libs/TSOProtocol/
COPY libs/Files/ ./libs/Files/
COPY libs/Vitaboy/ ./libs/Vitaboy/
RUN dotnet publish server/Dollhouse.Server.csproj -c Release -o out

# Second stage: setup the runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build-env /app/out .

# Copy the register.html file to the root of the server directory
COPY server/register.html /app/register.html

# Expose port 3077 and 8080
EXPOSE 3077
EXPOSE 8080

# Specify the entry point of your app
ENTRYPOINT ["dotnet", "Dollhouse.Server.dll"]