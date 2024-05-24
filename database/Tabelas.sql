USE DBFutebol;

if exists (SELECT * FROM dbo.SYSOBJECTS WHERE XTYPE = 'U' AND NAME = 'Estatistica')
	drop table Estatistica;
GO

if exists (SELECT * FROM dbo.SYSOBJECTS WHERE XTYPE = 'U' AND NAME = 'Jogo')
	drop table Jogo;
GO

if exists (SELECT * FROM dbo.SYSOBJECTS WHERE XTYPE = 'U' AND NAME = 'Campeonato')
	drop table Campeonato;
GO

if exists (SELECT * FROM dbo.SYSOBJECTS WHERE XTYPE = 'U' AND NAME = 'Equipe')
	drop table Equipe;
GO


CREATE TABLE Campeonato(

    nome varchar(30) NOT NULL,
    temporada varchar(10) NOT NULL,
    status varchar(30),

    CONSTRAINT pkcampeonato PRIMARY KEY (nome, temporada)
);


CREATE TABLE Equipe(

    nome VARCHAR(30) NOT NULL,
    apelido VARCHAR(30) NOT NULL,
    data_criacao DATE NOT NULL,


    CONSTRAINT pkequipe PRIMARY KEY (nome)
);


CREATE TABLE Jogo(

    nome_camp VARCHAR(30) NOT NULL, 
    temp_camp VARCHAR(10) NOT NULL, 
    time_casa VARCHAR(30) NOT NULL,
    time_visitante VARCHAR(30) NOT NULL,
    gols_time_casa INT NOT NULL,
    gols_time_visitante INT NOT NULL,

    CONSTRAINT pkjogo PRIMARY KEY (nome_camp, temp_camp, time_casa, time_visitante),
    CONSTRAINT fkcampeonato FOREIGN KEY (nome_camp, temp_camp) REFERENCES Campeonato(nome, temporada),

    CONSTRAINT fktimecasa FOREIGN KEY (time_casa) REFERENCES Equipe(nome),
    CONSTRAINT fktimevisitante FOREIGN KEY (time_visitante) REFERENCES Equipe(nome)
);


CREATE TABLE Estatistica(
    nome_camp VARCHAR(30) NOT NULL, 
    temp_camp VARCHAR(10) NOT NULL, 
    nome_equipe VARCHAR(30) NOT NULL,
    pontos INT,
    gols_marcados INT,
    gols_sofridos INT,

    CONSTRAINT pkestatistica PRIMARY KEY (nome_camp, temp_camp, nome_equipe),

    CONSTRAINT fkcampeonatoestatistica FOREIGN KEY (nome_camp, temp_camp) REFERENCES Campeonato(nome, temporada),
    CONSTRAINT fkequipe FOREIGN KEY (nome_equipe) REFERENCES Equipe(nome),
);
