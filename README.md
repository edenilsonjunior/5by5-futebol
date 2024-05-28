# 5by5 - Campeonato de Futebol

Projeto desenvolvido individualmente como parte do treinamento do estágio

## Funcionalidades do sistema:

- Cadastro e execução de campeonatos:
  - Possibilidade de inserção de equipes novas ou previamente cadastradas;
  - Multiplos jogos entre todas as equipes (feito de forma manual ou automatica, à escolha);
  - Exibição atualizada do placar do campeonato a qualquer momento;
  - Listagem de todos os jogos ocorridos, podendo navegar entre eles
- O campeonato pode possuir 3 status, sendo eles: Iniciado, Acontecendo, Finalizado.
- Um campeonato pode ser retomado à partir do menu inicial

## Logica do sistema

- O sistema utiliza conceitos de programação orientada à objetos e banco de dados, a linguagem utilizada foi C# e o SGBD utilizado foi o Sql Server;
- Para facilitar as operações que envolviam manipulação com banco de dados, foram criadas procedures que auxiliaram nesse quesito;
- As tratativas de possiveis exceções sao feitas e caso aconteçam, o usuario é informado do erro;
- Foram feitas diversas mudanças ao longo do projeto visando evitar repetir código, manter a legibilidade e simplicidade.

