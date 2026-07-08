FROM 

WORKDIR /app

COPY publish/ .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TraineeApi.dll"]
