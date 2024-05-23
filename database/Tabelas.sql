USE DBFutebol;

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
    pontos INT,
    gols_marcados INT,
    gols_sofridos INT,

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

GO
CREATE OR ALTER PROC InserirJogo
    @nome_camp VARCHAR(30), 
    @temp_camp VARCHAR(10), 
    @time_casa VARCHAR(30),
    @time_visitante VARCHAR(30),
    @gols_time_casa INT,
    @gols_time_visitante INT
    AS
    BEGIN

        DECLARE @pontos_time_casa int, @pontos_time_visitante int;

        -- atribuindo os pontos para cada equipe
        if(@gols_time_casa > @gols_time_visitante)
        BEGIN
            SET @pontos_time_casa = 3;
            SET @pontos_time_visitante = 0;
        END;
        ELSE IF(@gols_time_casa < @gols_time_visitante)
        BEGIN
            SET @pontos_time_visitante = 5;
            SET @pontos_time_casa = 0;
        END;
        ELSE
            BEGIN
            SET @pontos_time_visitante = 1;
            SET @pontos_time_casa = 1;
        END;

        -- Colocando os pontos, gols marcados e sofridos de cada equipe
        UPDATE Equipe
        SET 
            pontos = pontos + @pontos_time_casa,
            gols_marcados = gols_marcados + @gols_time_casa,
            gols_sofridos = gols_sofridos + @gols_time_visitante
        WHERE nome = @time_casa;

        UPDATE Equipe
        SET 
            pontos = pontos + @pontos_time_visitante,
            gols_marcados = gols_marcados + @gols_time_visitante,
            gols_sofridos = gols_sofridos + @gols_time_casa
        WHERE nome = @time_visitante;


        INSERT INTO Jogo
        VALUES (@nome_camp, @temp_camp, @time_casa, @time_visitante, @gols_time_casa, @gols_time_visitante);
END;

GO
CREATE OR ALTER PROC InserirEquipe

    @nome VARCHAR(30),
    @apelido VARCHAR(30),
    @data_criacao_str VARCHAR(10)
    AS
    BEGIN

    DECLARE 
    @data_criacao DATE = CONVERT(DATE, @data_criacao_str, 103),
    @pontos int = 0,
    @gols_sofridos int = 0,
    @gols_marcados int = 0

    INSERT INTO Equipe
    VALUES (@nome, @apelido, @data_criacao, @pontos, @gols_marcados, @gols_sofridos);
END;

INSERT INTO Campeonato(nome, temporada)
VALUES ('Brasileirao', '2024/1'),
 ('Brasileirao', '2024/2');


exec InserirEquipe 'Leões do Sul', 'Leões', '15/03/1998';
exec InserirEquipe 'Águias Aladas', 'Águias', '05/09/2005';
exec InserirEquipe 'Fúria Vermelha', 'Fúria', '20/04/1976';
exec InserirEquipe 'Dragões Azuis', 'Dragões', '10/01/1983';
exec InserirEquipe 'Tigres do Norte', 'Tigres', '08/07/2000';
exec InserirEquipe 'Lobos da Montanha', 'Lobos', '30/05/1995';
exec InserirEquipe 'Panteras Negras', 'Panteras', '12/10/1989';
exec InserirEquipe 'Corvos Noturnos', 'Corvos', '03/02/1979';
exec InserirEquipe 'Centauros da Cidade', 'Centauros', '21/08/2007';
exec InserirEquipe 'Serpentes Venenosas', 'Serpentes', '17/11/1992';


exec InserirJogo 'Brasileirao', '2024/1', 'Leões do Sul', 'Águias Aladas', 3, 2;
exec InserirJogo 'Brasileirao', '2024/1', 'Serpentes Venenosas', 'Corvos Noturnos', 10, 2;
exec InserirJogo 'Brasileirao', '2024/1', 'Serpentes Venenosas', 'Águias Aladas', 3, 5;


exec InserirJogo 'Brasileirao', '2024/2', 'Serpentes Venenosas', 'Corvos Noturnos', 10, 2;


select * from Equipe ORDER BY pontos DESC, gols_marcados DESC;
