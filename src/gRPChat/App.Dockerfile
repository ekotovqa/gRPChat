FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
#EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["Backend/gRPChat.Backend/gRPChat.Backend.csproj", "Backend/gRPChat.Backend/"]
COPY ["Frontend/Web/gRPChat.Web/gRPChat.Web.csproj", "Frontend/Web/gRPChat.Web/"]
COPY ["Shared/gRPChat.Protos/gRPChat.Protos/gRPChat.Protos.csproj", "Shared/gRPChat.Protos/gRPChat.Protos/"]
COPY ["Frontend/Web/Components/Blazor.CssGrid/Blazor.CssGrid.csproj", "Frontend/Web/Components/Blazor.CssGrid/"]
COPY ["Backend/gRPChat.Database/gRPChat.Database.csproj", "Backend/gRPChat.Database/"]
RUN dotnet restore "Backend/gRPChat.Backend/gRPChat.Backend.csproj"
COPY . .
WORKDIR "/src/Backend/gRPChat.Backend"
RUN dotnet build "gRPChat.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gRPChat.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS https://*:5003
ENTRYPOINT ["dotnet", "gRPChat.Backend.dll"]

# docker build -t grpcchat -f App.Dockerfile .
# docker run -d -p 5003:5003 --name grpcchat  grpcchat
# docker run -d -p 443:5003 -e ASPNETCORE_Kestrel__Certificates__Default__Password="KotofeyADMN@vbkFyf" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/localhost.pfx -v $env:USERPROFILE\.aspnet\https:/https/ --name grpcchat grpcchat