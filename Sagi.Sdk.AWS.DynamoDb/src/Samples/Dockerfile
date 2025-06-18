FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./Sagi.Sdk.Results/src/Sagi.Sdk.Results/Sagi.Sdk.Results.csproj ./Sagi.Sdk.Results/src/Sagi.Sdk.Results/Sagi.Sdk.Results.csproj
COPY ./Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/Sagi.Sdk.AWS.DynamoDb.csproj ./Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/
COPY ./Sagi.Sdk.AWS.DynamoDb/src/Samples/Samples.csproj ./Sagi.Sdk.AWS.DynamoDb/src/Samples/
RUN dotnet restore ./Sagi.Sdk.AWS.DynamoDb/src/Samples/Samples.csproj


COPY ./Sagi.Sdk.Results/src/Sagi.Sdk.Results/ ./Sagi.Sdk.Results/src/Sagi.Sdk.Results/
COPY ./Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/ ./Sagi.Sdk.AWS.DynamoDb/src/Sagi.Sdk.AWS.DynamoDb/
COPY ./Sagi.Sdk.AWS.DynamoDb/src/Samples/ ./Sagi.Sdk.AWS.DynamoDb/src/Samples/


WORKDIR /app/Sagi.Sdk.AWS.DynamoDb/src/Samples/
RUN dotnet publish -c Release -o /app/out


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Samples.dll"]