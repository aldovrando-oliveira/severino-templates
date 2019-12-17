# Template Api

Template com recursos pré configurados para facilitar o inicio de novos desenvolvimentos

* [Desenvolvimento](#desenvolvimento)
    - [Requisitos](#requisitos)
    - [Instalação](#instalação)
        - [Docker](#docker-compose)
        - [.Net Core 3.1](#net-core-3.1)
        - [VS Code](#visual-studio-code)
    - [Configuração](#configuração)

## Desenvolvimento

### Requisitos

``` 

* .Net Core 3.1
* Docker
* Docker Compose
* Mysql
* InfluxDb
* Grafana

``` 

### Instalação

#### Docker compose:
Acessar a pasta raiz do projeto e executar:
```

https://docs.docker.com/compose/install/
docker-compose up -d

``` 

#### .Net Core 3.1

```

wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get update

sudo apt-get install apt-transport-https

sudo apt-get install dotnet-sdk-3.1

``` 

#### Visual Studio Code

```

https://code.visualstudio.com/docs/?dv=linux64_deb

https://github.com/OmniSharp/omnisharp-vscode
```

## Configuração

|         Variável        |                   Descrição                  |   Tipo   | Obrigatório | Valor Padrão |
| ----------------------- | -------------------------------------------- |:--------:|:-----------:|:------------:|
| DB_TEMPLATE_SERVER      | Ip ou endereço do servidor de banco de dados |   Texto  |     Sim     |              |
| DB_TEMPLATE_DATABASE    | Nome do banco de dados                       |   Texto  |     Sim     |              |
| DB_TEMPLATE_USER        | Usuário para conexão de dados                |   Texto  |     Sim     |              |
| DE_TEMPLATE_PASSWORD    | Senha do usuário para acesso ao banco        |   Texto  |     Sim     |              |
| DB_TEMPLATE_POOLING     | Indica se deve ser usado pooling de conexão  |  Boolean |     Não     |     false    |
| DB_TEMPLATE_POOLING_MIN | Quantidade mínima de conexões no pool        | Numérico |     Não     |       1      |
| DB_TEMPLATE_POOLING_MAX | Quantidade máxima de conexões no pool        | Numérico |     Não     |       3      |
| INFLUXDB_URL            | Url para conexão com banco Influx            |   Texto  |     Não     |              |
| INFLUXDB_DATABASE       | Nome do banco de dados influx                |   Texto  |     Não     |              |
| INFLUXDB_USERNAME       | Usuário de conexão com o banco influx        |   Texto  |     Não     |              |
| INFLUXDB_PASSWORD       | Senha de usuário do banco influx             |   Texto  |     Não     |              |
| INFLUXDB_FLUSH_INTERVAL | Intervalo para envio das métricas            |   Texto  |     Não     |              |
| METRICS_CONTEXT         | Nome contexto das métricas                   |   Texto  |     Não     |              |

