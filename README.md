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

### Como rodar?

Para rodar com auto-reload:

```bash
dotnet watch run
```

Para rodar normalmente:

```bash
dotnet run
```