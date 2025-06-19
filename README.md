# JOKENPO API REST

## Introdução

Este é um projeto voltado para desenvolver a solução para o teste técnico do processo seletivo da **BTG Pactual**.

O projeto consiste em um sistema que disponibiliza APIs REST para realizar as atividades de um jogo de jokenpo.

### Funcionalidades

O sistema terá algumas funcionalidades:

- Cadastrar um jogador
  
  O jogador pode se cadastrar no sistema

- Cadastrar a jogada

  O jogador pode entrar em uma rodada por meio de uma jogada

- Remover um jogador

  O jogador pode remover a sua jogada da rodada

- Consultar a rodada atual

  O jogador pode consultar na rodada:
    - A situação da rodada (em andamento ou finalizada)
    - Os jogadores participantes
    - Quem ainda falta jogar, ou quem ganhou

- Finalização da rodada

  A rodada poderá ser finalizada após todos os jogadores ganharem

### Como Rodar?

1. Antes de tudo, renomeie `.env.template` para `.env`. Isso carregará as variáveis de ambiente no container do Docker.
2. Para rodar em modo de desenvolvimento (com `dotnet watch` rodando):

    ```bash
    docker compose up     # para rodar
    docker compose down   # para terminar
    ```

3. Para rodar em modo de produção:

    ```bash
    docker compose -f docker-compose.prod.yaml up -d    # para rodar
    docker compose -f docker-compose.prod.yaml down     # para terminar
    ```

4. Para rodar as migrations (no modo de desenvolvimento):

    ```bash
    export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres" & dotnet ef database update
    ```

## Decisões de Arquitetura

### Modelagem do Banco de dados

A modelagem feita no dbdiagram é mostrada a seguir:

![Modelagem no db diagram](/jokenpo.png)

Caso se queira consultar as cardinalidades de cada relação, [este é o link de visualização no dbdiagram](https://dbdiagram.io/d/jokenpo-6852ce8af039ec6d36d18ba4).

#### Algumas Considerações

- A rodada terá um status que será um `enum('open', 'closed')`, para indicar a situação da rodada. Isso permitirá o sistema julgar se é possível criar novas rodadas.
- A rodada guardará o `timestamp` da criação dela. Isso possibilitará ordenar as rodadas pelo tempo de criação, com o intuito de pegar sempre últimas rodadas.
- O cadastro do usuário será no sistema, significando que ele poderá participar de múltiplas rodadas.
- Como o sistema não exige autenticação, a identificação será feita tanto pelo id quanto pelo nome. Caso um usuário utilize um nome que já está em uso, o sistema o tratará como o "mesmo jogador" (mesmo ID).
- O cadastro do jogador na rodada será feita a partir do `participation`. Assim, a remoção de uma instância deste resultará na remoção da participação do usuário naquela rodada específica.
- O jogador tentará cadastrar sempre na rodada mais atual, contanto que esteja aberta. Caso já esteja finalizada, então será criada uma nova rodada, e o jogador imediatamente registrará sua participação naquela rodada.
- As mãos poderão ser registradas, com `id` e `name` únicos. As relações entre essas mãos serão mapeadas pela tabela `hands_relations`. Isso foi feito para que o sistema possa ser escalado, caso se queira criar mais mãos no jokenpo.

### Rotas Criadas

Abaixo seguem as rotas criadas para a API REST. Essas informações poderão ser observadas também no swagger do projeto.

**Rotas para usuários:**

- `POST /users`: Cadastrar (caso não exista) o usuário no sistema.
- `GET /users/{userId}/rounds`: Listar todas as rodadas em que o usuário participou

**Rotas para rodadas:**

- `GET /rounds/current`: Consultar a situação da rodada aberta atual.
- `GET /rounds/{roundId}`: Consultar a situação de uma rodada específica.
- `POST /rounds`: Criar uma nova rodada (retorna erro se já houver uma rodada aberta).
- `POST /rounds/{roundId}/participations`: Cadastrar o usuário na rodada (sem jogada).
- `PUT /rounds/{roundId}/participations/{userId}`: Atualizar a jogada do usuário na rodada.
- `DELETE /rounds/{roundId}/participations/{userId}`: Remover a participação do usuário na rodada (se ela ainda estiver aberta).
- `POST /rounds/{roundId}/finalize`: Finalizar a rodada atual. Muda status para "closed", calcula vencedor e retorna resultado.

### Exemplos de Chamadas à API

TBD