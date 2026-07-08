# FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:9.0

# WORKDIR /app

# COPY publish/ .

# ENV ASPNETCORE_URLS=http://+:8080

# EXPOSE 8080

# ENTRYPOINT ["dotnet", "TraineeApi.dll"]


FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build
 
WORKDIR /src

ARG CODEARTIFACT_TOKEN
 
COPY . .

RUN ls
 
RUN dotnet nuget add source https://zeuslearning-824100177805.d.codeartifact.ap-south-1.amazonaws.com/nuget/training-nuget-store/v3/index.json \
    --name CodeArtifact \
    --username aws \
    --password ${CODEARTIFACT_TOKEN} \
    --store-password-in-clear-text 
   
RUN dotnet nuget disable source nuget

RUN dotnet restore --configfile ./nuget.config
 
RUN dotnet publish TraineeApi.csproj \
    -c Release \
    -o /app/publish
 
FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:10.0-alpine
 
WORKDIR /app
 
COPY --from=build /app/publish .
 
EXPOSE 8080
 
ENTRYPOINT ["dotnet","TraineeApi.dll"]