FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["NoteApp_UserManagement_Api.csproj", "src/"]
RUN dotnet restore "src/NoteApp_UserManagement_Api.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "NoteApp_UserManagement_Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NoteApp_UserManagement_Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NoteApp_UserManagement_Api.dll"]
