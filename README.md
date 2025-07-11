# JOKENPO API REST

## Introdução

Este é um projeto para o desafio técnico do processo seletivo da vaga de estágio na **BTG Pactual**. O projeto consiste em um sistema backend que disponibiliza APIs REST para realizar as atividades de um jogo de jokenpo.

A documentação completa da API está [**nesse link**](https://documenter.getpostman.com/view/39631274/2sB2xBDpsi).

O projeto está em produção em https://shogoshima.duckdns.org.

### Funcionalidades

O sistema terá as seguintes funcionalidades:

- **Cadastrar um jogador**: O jogador pode se cadastrar no sistema.
- **Cadastrar um jogador na rodada**: O jogador pode se cadastrar na rodada atual (com ou sem jogada).
- **Cadastrar a jogada na rodada**: O jogador pode atualizar a sua jogada.
- **Remover um jogador da rodada**: O jogador pode se remover da rodada.
- **Consultar a rodada atual**: O jogador pode consultar na rodada:
  - A situação da rodada (em andamento ou finalizada)
  - Os jogadores participantes
  - Quem ainda falta jogar, ou quem ganhou
- **Finalização da rodada**: Ao finalizar a rodada, é possível ver os ganhadores. Após finalizada, não é possível mais atualizar dados relacionados àquela rodada.

### Como Rodar?

Para rodar, é preciso ter instalado no seu sistema:

- Docker
- .NET 8.0 SDK

Após essa etapa, siga os seguintes passos:

1. Vá ao diretório do projeto:

    ```bash
    cd JokenpoApiRest
    ```

2. Renomeie `.env.template` para `.env`. Isso carregará as variáveis de ambiente no container do Docker.
3. Para rodar em modo de desenvolvimento (com `dotnet watch` rodando):

    ```bash
    docker compose up     # para rodar
    docker compose down   # para terminar
    ```

4. Para rodar em modo de produção:

    ```bash
    docker compose -f docker-compose.prod.yaml up -d    # para rodar
    docker compose -f docker-compose.prod.yaml down     # para terminar
    ```

5. Para rodar as migrations (no modo de desenvolvimento), execute os seguintes comandos:

    ```bash
    export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres" && dotnet ef database update
    ```

    > Altere `Database`, `Username` e `Password` conforme o necessário (caso rode em produção).

## Decisões de Arquitetura

### Tecnologias Escolhidas

- **ASP.NET Core:** O ASP.NET Core foi escolhido tanto pela vaga de estágio, quanto para aprendizado do autor.
- **Docker:** O Docker foi escolhido para isolar o projeto, facilitar a execução do projeto e otimizar o fluxo de desenvolvimento.
- **PostgreSQL:** O banco de dados escolhido foi o PostgreSQL, devido à maior experiência do autor com ele, incluindo a familiaridade com o PgAdmin. Apesar da utilização do banco de dados não ser obrigatória, decidiu-se por utilizar considerando um sistema escalável.
- **Postman:** O Postman foi escolhido para documentar a API de forma mais adequada, já que permite a visualização de requisições exemplo, e a explicação mais detalhada delas.
- **AWS EC2**: O projeto está em produção pode meio do serviço EC2 da AWS. Isso foi feito tanto para poder visualizar o resultado de forma mais fácil, quanto para certificar-se de que o projeto roda facilmente em outras máquinas.

### Modelagem do Banco de dados

A modelagem feita é mostrada a seguir:

![Modelagem no dbdiagram](/jokenpo.png)

Ao analisar, é possível identificar algumas entidades:

- **rounds:** A rodada. Ele possui um `status` para saber se o usuário pode ou não atualizar suas informações, e uma data de criação para se poder ordená-lo.
- **users:** O usuário com `id` e `nome`, conforme a especificação.
- **hands:** As jogadas que o usuário pode escolher quando vai participar da rodada.
- **participations:** A participação do jogador. É uma relação ternária, em que a chave primária composta são os `id`'s da rodada e do usuário que está se cadastrando para a rodada. O `hand_id` é opcional, pois o usuário pode apenas se cadastrar na rodada, sem necessidade de cadastrar a jogada. 
- **hands_relations:** As relações entre as jogadas. É possível registrar qual jogada ganha de qual. Será utilizada na hora de calcular o(s) vencedor(es).

Caso se queira consultar as cardinalidades de cada relação, [este é o link de visualização no dbdiagram](https://dbdiagram.io/d/jokenpo-6852ce8af039ec6d36d18ba4).

#### Considerações

- A rodada terá um status que será um `enum('open', 'closed')`, para indicar a situação da rodada. Isso permitirá o sistema julgar se é possível criar novas rodadas.
- A rodada guardará o `timestamp` da criação dela. Isso possibilitará ordenar as rodadas pelo tempo de criação, com o intuito de pegar sempre últimas rodadas.
- O cadastro do usuário será no sistema, significando que ele poderá participar de múltiplas rodadas.
- Como o sistema não exige autenticação, a identificação será feita tanto pelo id quanto pelo nome. Caso um usuário utilize um nome que já está em uso, o sistema o tratará como o "mesmo jogador" (mesmo ID).
- O cadastro do jogador na rodada será feita a partir do `participation`. Assim, a remoção de uma instância deste resultará na remoção da participação do usuário naquela rodada específica.
- O jogador tentará cadastrar sempre na rodada mais atual, contanto que esteja aberta. Caso já esteja finalizada, então será criada uma nova rodada, e o jogador imediatamente registrará sua participação naquela rodada.
- As mãos poderão ser registradas, com `id` e `name` únicos. As relações entre essas mãos serão mapeadas pela tabela `hands_relations`. Isso foi feito para que o sistema possa ser escalado, caso se queira criar mais mãos no jokenpo.

## Rotas Criadas

Abaixo seguem as rotas criadas para a API REST. Essas informações poderão ser observadas também no swagger do projeto, e na documentação do Postman.

**Rotas para usuários:**

- `POST /api/users`: Cadastrar (caso não exista) o usuário no sistema.
- `GET /api/users/{userId}/rounds`: Listar todas as rodadas em que o usuário participou.

**Rotas para jogadas:**

- `GET /api/hands`: Listar todas as jogadas possíveis.

**Rotas para rodadas:**

- `GET /api/rounds/current`: Consultar a situação da rodada aberta atual.
- `GET /api/rounds/{roundId}`: Consultar a situação de uma rodada específica.
- `POST /api/rounds`: Criar uma nova rodada (retorna erro se já houver uma rodada aberta).
- `POST /api/rounds/{roundId}/participations`: Cadastrar o usuário na rodada (com ou sem jogada).
- `PUT /api/rounds/{roundId}/participations`: Atualizar a jogada do usuário na rodada.
- `DELETE /api/rounds/{roundId}/participations/{userId}`: Remover a participação do usuário na rodada (se ela ainda estiver aberta).
- `POST /api/rounds/{roundId}/finalize`: Finalizar a rodada atual. Muda status para "closed", calcula vencedor e retorna resultado.

## Exemplos de Chamadas à API

Caso se queira consultar exemplos de chamadas e respostas da API, consulte a [**documentação completa criada no Postman**](https://documenter.getpostman.com/view/39631274/2sB2xBDpsi):

![Print de parte da documentação no Postman](postman.png)

A documentação da API, com a estrutura esperada das respostas, também pode ser vista no Swagger (em `http://localhost:5176/swagger` ao executar o projeto):

![Documentação no Swagger](swagger.png)

### Exemplos de Possíveis Cenários

Abaixo seguem exemplos de fluxo de utilização da API que constavam na especificação.

#### Cenário 1 (Sucesso)

1. Rota `POST /api/users` para cadastrar usuários no sistema:
    - Usuário 1: Carlos
      ```json
      {
        "name": "Carlos"
      }
      ```
      retorna
      ```json
      {
        "id": 1,
        "name": "Carlos"
      }
      ```
    - Usuário 2: João
      ```json
      {
        "name": "João"
      }
      ```
      retorna
      ```json
      {
        "id": 2,
        "name": "João"
      }
      ```
    - Usuário 3: Matheus
      ```json
      {
        "name": "Matheus"
      }
      ```
      retorna
      ```json
      {
        "id": 3,
        "name": "Matheus"
      }
      ```
2. Rota `POST /api/rounds` para criar uma rodada nova. Retorna: 
    ```json
    {
      "id": 1,
      "status": 0,
      "createdAt": "2025-06-21T05:07:15.9537005Z"
    }
    ```
3. Rota `POST /api/rounds/1/participations` para cadastrar usuário na rodada 1:
    - Usuário 1: Carlos joga pedra (handId = 1)
      ```json
      {
        "userId": 1,
        "handId": 1
      }
      ```
    - Usuário 2: João joga tesoura (handId = 3)
      ```json
      {
        "userId": 2,
        "handId": 3
      }
      ```
    - Usuário 3: Matheus apenas se cadastra na rodada
      ```json
      {
        "userId": 3
      }
      ```
4. Rota `GET /api/rounds/current` para consultar a situação da rodada atual. Retorna:
    ```json
    {
      "data": {
        "id": 1,
        "status": 0,
        "createdAt": "2025-06-21T05:07:15.9537005Z"
      },
      "playedUsers": [
        "Carlos",
        "João"
      ],
      "pendingUsers": [
        "Matheus"
      ]
    }
    ```
5. Rota `PUT /api/rounds/1/participations` para atualizar a jogada na rodada 1:
    - Usuário 3: Matheus joga tesoura (handId = 3)
      ```json
      {
        "userId": 3,
        "handId": 3
      }
      ```
6. Rota `POST /api/rounds/1/finalize` para finalizar a rodada 1. Retorna:
    ```json
    {
      "message": "Temos um vencedor!",
      "winners": ["Carlos"],
      "participations": [
        {
          "name": "Carlos",
          "handName": "Pedra"
        },
        {
          "name": "João",
          "handName": "Tesoura"
        },
        {
          "name": "Matheus",
          "handName": "Tesoura"
        }
      ]
    }
    ```

#### Cenário 2 (Falha)

1. Rota `POST /api/users` para cadastrar usuários no sistema:
    - Usuário 1: Carlos
      ```json
      {
        "name": "Carlos"
      }
      ```
      retorna
      ```json
      {
        "id": 1,
        "name": "Carlos"
      }
      ```
    - Usuário 2: João
      ```json
      {
        "name": "João"
      }
      ```
      retorna
      ```json
      {
        "id": 2,
        "name": "João"
      }
      ```
    - Usuário 3: Matheus
      ```json
      {
        "name": "Matheus"
      }
      ```
      retorna
      ```json
      {
        "id": 3,
        "name": "Matheus"
      }
      ```
2. Rota `POST /api/rounds` para criar uma rodada nova. Retorna: 
    ```json
    {
      "id": 1,
      "status": 0,
      "createdAt": "2025-06-21T05:07:15.9537005Z"
    }
    ```
3. Rota `POST /api/rounds/1/participations` para cadastrar usuário na rodada 1:
    - Usuário 1: Carlos joga pedra (handId = 1)
      ```json
      {
        "userId": 1,
        "handId": 1
      }
      ```
    - Usuário 2: João joga tesoura (handId = 3)
      ```json
      {
        "userId": 2,
        "handId": 3
      }
      ```
    - Usuário 3: Matheus apenas se cadastra na rodada
      ```json
      {
        "userId": 3
      }
      ```
4. Rota `POST /api/rounds/1/finalize` para finalizar a rodada 1. Retorna erro:
    ```json
    {
      "message": "Ainda faltam jogadas de alguns participantes.",
      "pending": [
        "Matheus"
      ]
    }
    ```

#### Cenário 3 (Falha)

1. Rota `POST /api/rounds` para criar uma rodada nova. Retorna: 
    ```json
    {
      "id": 1,
      "status": 0,
      "createdAt": "2025-06-21T05:07:15.9537005Z"
    }
    ```
2. Rota `POST /api/rounds/1/participations` para cadastrar usuário na rodada 1:
    - Jogada jogador 1
      ```json
      {
        "userId": 1
      }
      ```
      retorna
      ```json
      {
        "message": "usuário não cadastrado."
      }
      ```