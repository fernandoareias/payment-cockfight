# Payment Cockfight

Sistema de pagamentos distribuído com arquitetura de microserviços usando RedPanda (Kafka), MongoDB e Nginx como proxy reverso.

## Pré-requisitos

- Docker
- Docker Compose
- Pelo menos 1.5 CPU cores disponíveis
- Pelo menos 350MB de RAM disponível

## Arquitetura

O projeto utiliza os seguintes serviços:

- **RedPanda**: Message broker compatível com Kafka (1 nó)
- **RedPanda Console**: Interface web para monitoramento do Kafka
- **Schema Registry**: Gerenciamento de esquemas de mensagens
- **MongoDB**: Banco de dados NoSQL
- **Nginx**: Proxy reverso para load balancing
- **App-1 e App-2**: Placeholders para suas aplicações futuras

## Distribuição de Recursos

O sistema foi otimizado para funcionar com recursos controlados:

- **CPU Total**: 1.0 cores utilizados explicitamente
- **RAM Total**: 420MB alocados

### Distribuição atual por serviço:

**CPU (1.0 cores alocados):**
- MongoDB: 0.4 CPU (40%) - Banco de dados principal
- RedPanda (1 nó): 0.25 CPU (25%) - Message broker
- RedPanda Console: 0.15 CPU (15%) - Interface web
- Schema Registry: 0.15 CPU (15%) - Gerenciador de esquemas  
- App 1: 0.15 CPU (15%) - Aplicação placeholder
- App 2: 0.15 CPU (15%) - Aplicação placeholder
- Nginx: 0.1 CPU (10%) - Proxy reverso

**Memória (420MB alocados):**
- MongoDB: 280MB (67%) - Cache WiredTiger 0.25GB
- RedPanda: 200MB (48%) - Message broker otimizado
- Schema Registry: 50MB (12%) - Gerenciador de esquemas
- RedPanda Console: 40MB (10%) - Interface web
- App 1: 40MB (10%) - Aplicação placeholder  
- App 2: 40MB (10%) - Aplicação placeholder
- Nginx: 10MB (2%) - Proxy leve

### Características importantes:

- **MongoDB**: Configurado com cache WiredTiger de 256MB mínimo
- **RedPanda**: Modo single-node otimizado para recursos limitados
- **Aplicações**: Placeholders prontos para suas aplicações reais

## Como executar

### 1. Primeira execução (limpeza completa)

Para uma execução limpa, remova todos os dados persistidos:

```bash
# Parar todos os serviços e remover volumes
docker compose down -v

# Limpar dados persistidos (requer sudo)
sudo rm -rf ./infrastructure/kafka/redpanda-0/*
sudo rm -rf ./infrastructure/mongodb/data/*

# Recriar estrutura de pastas
mkdir -p ./infrastructure/kafka/redpanda-0
mkdir -p ./infrastructure/mongodb/data
```

**Importante:** O MongoDB será inicializado automaticamente na primeira execução:
- Usuário admin: `admin/password123` (criado pelo MONGO_INITDB)
- Usuário app: `app_user/app_password123` (criado pelo script de inicialização)
- Database `payment_db` com coleções básicas e dados de exemplo

### 2. Iniciar os serviços

```bash
docker compose up -d
```

**Na primeira execução**, aguarde alguns minutos para:
- RedPanda formar o cluster single-node
- MongoDB executar scripts de inicialização
- Todos os serviços estabilizarem

### 3. Verificar status

```bash
# Ver status de todos os serviços
docker-compose ps

# Ver logs em tempo real
docker-compose logs -f
```

## Serviços disponíveis

Após a inicialização, os seguintes serviços estarão disponíveis:

### RedPanda Console
- **URL**: http://localhost:8083
- **Descrição**: Interface web para monitoramento e gerenciamento do Kafka

### Schema Registry
- **URL**: http://localhost:8081
- **Descrição**: API para gerenciamento de esquemas de mensagens

### MongoDB
- **Host**: localhost:27017
- **Usuário**: admin
- **Senha**: password123
- **Database**: payment_db
- **String de conexão**: `mongodb://admin:password123@localhost:27017/payment_db?authSource=admin`
- **Cache WiredTiger**: 256MB configurado automaticamente
- **Usuário admin**: admin/password123 
- **Database padrão**: payment_db

### Kafka (RedPanda)
- **Interno**: redpanda-0:9092
- **Externo**: localhost:19092

### Admin API (RedPanda)
- **URL**: http://localhost:9644
- **Descrição**: API administrativa do RedPanda

### Nginx (Load Balancer)
- **URL**: http://localhost:9999
- **Descrição**: Proxy reverso para suas aplicações

## Desenvolvendo suas aplicações

### Substituindo os placeholders

As aplicações app-1 e app-2 são atualmente placeholders (Alpine Linux). Para substituí-las por suas aplicações reais:

1. **Edite o docker-compose.yaml**:

```yaml
app-1:
  image: sua-aplicacao-1:latest
  container_name: app-1
  ports:
    - "3001:3000"
  environment:
    - DATABASE_URL=mongodb://admin:password123@mongodb:27017/payment_db
    - KAFKA_BROKERS=redpanda-0:9092
  networks:
    - backend
    - payment-processor
  deploy:
    resources:
      limits:
        cpus: "0.2"
        memory: "65MB"
  depends_on:
    - mongodb
    - redpanda-0
```

2. **Reconstrua os serviços**:

```bash
docker-compose up -d --build
```

### Conectando ao MongoDB

Use a seguinte string de conexão em suas aplicações:

```
mongodb://admin:password123@mongodb:27017/payment_db
```

O banco já vem com:
- Usuário aplicação: `app_user` / `app_password123`
- Coleções básicas: `payments`, `transactions`, `users`

### Conectando ao Kafka

Configure seus produtores e consumidores com:

```
KAFKA_BROKERS=redpanda-0:9092
```

## Comandos úteis

### Monitoramento

```bash
# Ver logs de um serviço específico
docker-compose logs -f redpanda-0
docker-compose logs -f mongodb

# Ver uso de recursos
docker stats

# Verificar saúde dos serviços
docker-compose ps
```

### Limpeza

```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes (perda de dados)
docker-compose down -v

# Remover imagens não utilizadas
docker image prune
```

### Debugging

```bash
# Acessar shell de um container
docker-compose exec redpanda-0 /bin/bash
docker-compose exec mongodb /bin/bash

# Ver configuração do RedPanda
docker-compose exec redpanda-0 rpk cluster info
```

## Resolução de problemas

### RedPanda Console não inicia (code 137)

Isso indica falta de memória. O sistema já foi otimizado, mas se ainda ocorrer:

```bash
# Reiniciar apenas o console
docker-compose up -d redpanda-console
```

### Nginx falha na inicialização

Verifique se as aplicações de destino estão rodando:

```bash
# Ver logs do nginx
docker-compose logs nginx

# Verificar conectividade
docker-compose exec nginx ping app-1-placeholder
```

### RedPanda não forma cluster

Se houver problemas de eleição de leader:

```bash
# Fazer limpeza completa (seção "Primeira execução")
docker-compose down -v
sudo rm -rf ./infrastructure/kafka/redpanda-0/*
docker-compose up -d
```

### MongoDB não aceita conexões

```bash
# Verificar se o MongoDB iniciou corretamente (deve mostrar logs de inicialização)
docker-compose logs mongodb

# Verificar se está usando recursos adequados (280MB RAM)
docker-compose ps

# Testar conexão (usar authSource=admin)
docker-compose exec mongodb mongosh --username admin --password password123 --authenticationDatabase admin

# Ou via string de conexão completa
mongosh "mongodb://admin:password123@localhost:27017/payment_db?authSource=admin"

# Se falhar com erro de cache, verifique se há memória suficiente
# MongoDB requer mínimo 280MB RAM (256MB para cache WiredTiger)
```

### MongoDB erro de cache WiredTiger

Se aparecer erro "cacheSizeGB must be greater than or equal to 0.25":

```bash
# O problema é falta de recursos - MongoDB precisa de 280MB+ de RAM
# Configuração atual: --wiredTigerCacheSizeGB 0.25 (256MB mínimo)
docker-compose restart mongodb
```

## Estrutura do projeto

```
payment-cockfight/
├── docker-compose.yaml          # Configuração dos serviços
├── infrastructure/
│   ├── kafka/                   # Dados persistidos do RedPanda
│   ├── mongodb/
│   │   ├── data/               # Dados do MongoDB
│   │   └── init/               # Scripts de inicialização
│   └── ngnix/
│       └── nginx.conf          # Configuração do Nginx
├── payment-processor/           # Seus processadores de pagamento
└── README.md                    # Este arquivo
```

## Próximos passos

1. Desenvolva suas aplicações de pagamento
2. Substitua os placeholders app-1 e app-2
3. Configure tópicos Kafka apropriados
4. Implemente esquemas no Schema Registry
5. Configure monitoramento adicional

## Suporte

Para problemas ou dúvidas:

1. Verifique os logs: `docker-compose logs`
2. Confirme recursos disponíveis: `docker stats`
3. Verifique conectividade de rede entre containers
4. Consulte a documentação do RedPanda em https://docs.redpanda.com