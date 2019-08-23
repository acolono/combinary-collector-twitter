FROM microsoft/dotnet:2.1-sdk-alpine AS build-env
WORKDIR /src
ADD . ./
RUN dotnet restore
RUN cd TwitterStream && dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build-env /src/TwitterStream/out ./
ENTRYPOINT ["dotnet", "TwitterStream.dll"]
