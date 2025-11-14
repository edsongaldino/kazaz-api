# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1) Copie SOMENTE os .csproj (melhor cache)
COPY Kazaz.API/Kazaz.API.csproj Kazaz.API/
COPY Kazaz.Application/Kazaz.Application.csproj Kazaz.Application/
COPY Kazaz.Domain/Kazaz.Domain.csproj Kazaz.Domain/
COPY Kazaz.Infrastructure/Kazaz.Infrastructure.csproj Kazaz.Infrastructure/
COPY Kazaz.SharedKernel/Kazaz.SharedKernel.csproj Kazaz.SharedKernel/

# 2) Restore baseado no csproj da API (que referencia as demais)
RUN dotnet restore Kazaz.API/Kazaz.API.csproj

# 3) Agora copie o restante do código
COPY . .

# 4) Publish (Release)
RUN dotnet publish Kazaz.API/Kazaz.API.csproj -c Release -o /out

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out ./

# API escutando na porta 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# DLL exata (case-sensitive em Linux)
ENTRYPOINT ["dotnet", "Kazaz.API.dll"]
