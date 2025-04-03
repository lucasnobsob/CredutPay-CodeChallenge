# Usa uma imagem oficial do Node.js como base
FROM node:20-alpine

# Define o diretório de trabalho dentro do container
WORKDIR /app

# Copia os arquivos de dependências e instala os pacotes
COPY package.json package-lock.json ./
RUN npm install

# Copia o restante do código para dentro do container
COPY . .

# Compila o projeto (se necessário)
RUN npm run build

# Expõe a porta em que a aplicação será servida
EXPOSE 3000

# Comando para iniciar o servidor Next.js
CMD ["npm", "run", "start"]
