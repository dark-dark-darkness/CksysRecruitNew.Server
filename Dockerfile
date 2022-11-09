#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CksysRecruitNew.Server.csproj", "."]
RUN dotnet restore "./CksysRecruitNew.Server.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CksysRecruitNew.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CksysRecruitNew.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "Asia/Shanghai" > /etc/timezone && ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime 
ENTRYPOINT ["dotnet", "CksysRecruitNew.Server.dll"]