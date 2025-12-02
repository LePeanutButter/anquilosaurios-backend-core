FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

WORKDIR /app

COPY build/ .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "anquilosaurios-backend-core.dll"]