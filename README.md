## JOKENPO API REST

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

Para rodar com auto-reload:

```bash
dotnet watch run
```

Para rodar normalmente:

```bash
dotnet run
```

### Modelagem do Banco de dados

A modelagem feita no db diagram é mostrada a seguir:

![Modelagem no db diagram](/jokenpo.png)

Caso se queira consultar as cardinalidades de cada relação, [este é o link de visualização no dbdiagram](https://dbdiagram.io/d/jokenpo-6852ce8af039ec6d36d18ba4).

#### Algumas Considerações de Arquitetura

- A rodada terá um status que será um `enum('open', 'closed')`, para indicar a situação da rodada. Isso permitirá o sistema julgar se é possível criar novas rodadas.
- A rodada guardará o `timestamp` da criação dela. Isso possibilitará ordenar as rodadas pelo tempo de criação, com o intuito de pegar sempre últimas rodadas.
- O cadastro do usuário será no sistema, significando que ele poderá participar de múltiplas rodadas.
- Como o sistema não exige autenticação, a identificação será feita tanto pelo id quanto pelo nome. Caso um usuário utilize um nome que já está em uso, o sistema o tratará como o "mesmo jogador" (mesmo ID).
- O cadastro do jogador na rodada será feita a partir do `participation`. Assim, a remoção de uma instância deste resultará na remoção da participação do usuário naquela rodada específica.
- O jogador tentará cadastrar sempre na rodada mais atual, contanto que esteja aberta. Caso já esteja finalizada, então será criada uma nova rodada, e o jogador imediatamente registrará sua participação naquela rodada.
- As mãos poderão ser registradas, com `id` e `name` únicos. As relações entre essas mãos serão mapeadas pela tabela `hands_relations`. Isso foi feito para que o sistema possa ser escalado, caso se queira criar mais mãos no jokenpo.

### Rotas Criadas

- `POST /user`: Cadastrar (caso não exista) o usuário, e retornar todas as rodadas que aquele usuário jogou.
- `POST /round/participation`: Cadastrar o usuário na rodada mais atual, ou criar uma rodada nova caso não exista.
- `DELETE /round/participation`: Remover a participação do usuário na rodada.
- `GET /round`: Consultar a situação da rodada atual.
- `GET /round/{id}`: Consultar a situação de uma rodada.
- `POST /round/finalize`: Finalizar a rodada atual.