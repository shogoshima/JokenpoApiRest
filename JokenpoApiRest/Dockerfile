FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app

# Restore só uma vez (vai para o cache de build da imagem)
COPY *.csproj ./
RUN dotnet restore

# Copia o resto das fontes (mas isso será escondido pelo volume em dev)
COPY . ./

CMD ["dotnet", "watch", "run", "--urls", "http://+:80"]