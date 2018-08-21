#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 1862
EXPOSE 44323

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["AWSS3WebApi/AWSS3WebApi.csproj", "AWSS3WebApi/"]
RUN dotnet restore "AWSS3WebApi/AWSS3WebApi.csproj"
COPY . .
WORKDIR "/src/AWSS3WebApi"
RUN dotnet build "AWSS3WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AWSS3WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AWSS3WebApi.dll"]