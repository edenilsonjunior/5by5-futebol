USE DBFutebol;

-- Procedure que vai inserir uma equipe ao contexto geral
GO
CREATE OR ALTER PROC InserirEquipe

    @nome VARCHAR(30),
    @apelido VARCHAR(30),
    @data_criacao_str VARCHAR(10)
    AS
    BEGIN

        DECLARE @data_criacao DATE = CONVERT(DATE, @data_criacao_str, 103);

        IF NOT EXISTS (SELECT * FROM Equipe WHERE nome = @nome)
        BEGIN
            INSERT INTO Equipe
            VALUES (@nome, @apelido, @data_criacao);
        END;
END;

-- Procedure que vai criar uma estatistica para determinado time, em determinado campeonato
GO
CREATE OR ALTER PROC AdicionarTimeAoCampeonato
    @nome_camp VARCHAR(30), 
    @temp_camp VARCHAR(10), 
    @nome_equipe VARCHAR(30)
    AS
    BEGIN

        IF NOT EXISTS (SELECT * FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp AND nome_equipe = @nome_equipe)
        BEGIN
            INSERT INTO Estatistica VALUES
            (@nome_camp, @temp_camp, @nome_equipe, 0, 0, 0);
        END;
END;

-- Procedure que vai atualizar uma estatistica para determinado time, em determinado campeonato
GO
CREATE OR ALTER PROC AtualizarEstatistica
    @nome_camp VARCHAR(30), 
    @temp_camp VARCHAR(10), 
    @nome_equipe VARCHAR(30),
    @pontos INT,
    @gols_marcados INT,
    @gols_sofridos INT
    AS
    BEGIN
        IF EXISTS (SELECT * FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp AND nome_equipe = @nome_equipe)
        BEGIN
            UPDATE Estatistica
            SET 
                pontos = pontos + @pontos,
                gols_marcados = gols_marcados + @gols_marcados,
                gols_sofridos = gols_sofridos + @gols_sofridos
            WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp AND nome_equipe = @nome_equipe;
        END;
        ELSE
        BEGIN
            INSERT INTO Estatistica VALUES
            (@nome_camp, @temp_camp, @nome_equipe, @pontos, @gols_marcados, @gols_sofridos);
        END;
END;

-- Procedure que vai inserir um jogo de terminados times, em um campeonato, manipulando tambem a tabela Estatistica
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

        -- Inserindo as estatisticas do time da casa        
        EXEC AtualizarEstatistica @nome_camp, @temp_camp, @time_casa, @pontos_time_casa, @gols_time_casa, @gols_time_visitante;

        -- Inserindo as estatisticas do time visitante        
        EXEC AtualizarEstatistica @nome_camp, @temp_camp, @time_visitante, @pontos_time_visitante, @gols_time_visitante, @gols_time_casa;

        -- Inserindo o jogo atual em sua respectiva tabela
        IF NOT EXISTS (SELECT * FROM Jogo WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp AND time_casa = @time_casa AND time_visitante = @time_visitante)
        BEGIN
            INSERT INTO Jogo
            VALUES (@nome_camp, @temp_camp, @time_casa, @time_visitante, @gols_time_casa, @gols_time_visitante);
        END;
END;

GO
CREATE OR ALTER PROC InserirCampeonato
    @nome VARCHAR(30), 
    @temporada VARCHAR(10), 
    @status VARCHAR(30),
    @resultado INT OUTPUT
    AS
    BEGIN
        IF NOT EXISTS (SELECT * FROM Campeonato WHERE nome = @nome AND temporada = @temporada)
        BEGIN
            INSERT INTO Campeonato VALUES (@nome, @temporada, @status);
            SET @resultado = 1;
        END;
        ELSE
            SET @resultado = 0;
END;

GO
CREATE OR ALTER PROC AtualizarCampeonato
    @nome VARCHAR(30), 
    @temporada VARCHAR(10), 
    @status VARCHAR(30)
    AS
    BEGIN

        IF NOT EXISTS (SELECT * FROM Campeonato WHERE nome = @nome AND temporada = @temporada)
        BEGIN
            INSERT INTO Campeonato VALUES (@nome, @temporada, @status);
        END;
        ELSE
        BEGIN
            UPDATE Campeonato
            SET status = @status
            WHERE nome = @nome AND temporada = @temporada;
        END;

END;


GO
CREATE OR ALTER PROC RecuperarEquipesPorCampeonato
    @nome_camp VARCHAR(30),
    @temp_camp VARCHAR(10)
    AS
    BEGIN
    SELECT e.*
    FROM Equipe e
    INNER JOIN Estatistica es ON e.nome = es.nome_equipe
    WHERE es.nome_camp = @nome_camp AND es.temp_camp = @temp_camp;
END;
