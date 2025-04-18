# Base stage with .NET dependencies
FROM defradigital/dotnetcore-development:dotnet8.0 AS base
 
USER root
ARG PORT=8080
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}
 
RUN apk update && apk --no-cache add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
 
# Build stage with .NET and Node.js/Gulp installation
FROM defradigital/dotnetcore-development:dotnet8.0 AS build
USER root
WORKDIR /src
COPY ["EPR.Payment.Portal/EPR.Payment.Portal.csproj", "EPR.Payment.Portal/"]
COPY ["EPR.Payment.Portal.Common/EPR.Payment.Portal.Common.csproj", "EPR.Payment.Portal.Common/"]
COPY ["NuGet.Config",""]

# Use Build Argument to insert PAT into Nuget file
ARG PAT=localhost
RUN sed -i "s|</configuration>|<packageSourceCredentials><epr-packaging-common><add key=\"Username\" value=\"PAT\" /><add key=\"ClearTextPassword\" value=\"${PAT}\" /></epr-packaging-common></packageSourceCredentials></configuration>|" NuGet.Config
 
# Restore as distinct layers
USER root
COPY Directory.Build.props ./
RUN dotnet restore "EPR.Payment.Portal/EPR.Payment.Portal.csproj" --configfile "./NuGet.Config"
 
# Install Node.js
RUN apk add --no-cache nodejs npm
 
# Install dotnet-ef tool
RUN dotnet tool install dotnet-ef --version 6.* --tool-path "/root/.dotnet/tools"
 
# Add the tools folder to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"
 
COPY EPR.Payment.Portal/. ./EPR.Payment.Portal/.
COPY EPR.Payment.Portal.Common/. ./EPR.Payment.Portal.Common/.
 
# Install Gulp CLI globally and project dependencies
WORKDIR "/src/EPR.Payment.Portal"
RUN npm install --ignore-scripts -g gulp-cli && npm install --ignore-scripts
 
# Run Gulp build task
RUN gulp build-frontend
 
RUN dotnet build "EPR.Payment.Portal.csproj" -c Release /p:AzureBuild=true -o /app/build
 
FROM build AS publish
RUN dotnet publish "EPR.Payment.Portal.csproj" -c Release /p:AzureBuild=false -o /app/publish
 
# Final stage
FROM base AS final
 
# Switch to the non-root user
USER dotnet
 
WORKDIR /app
COPY --from=publish /app/publish .
USER dotnet
ENTRYPOINT ["dotnet", "EPR.Payment.Portal.dll"]