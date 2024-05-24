USE DBFutebol;

GO
CREATE OR ALTER PROC InserirEstatistica
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
        EXEC InserirEstatistica @nome_camp, @temp_camp, @time_casa, @pontos_time_casa, @gols_time_casa, @gols_time_visitante;

        -- Inserindo as estatisticas do time visitante        
        EXEC InserirEstatistica @nome_camp, @temp_camp, @time_visitante, @pontos_time_visitante, @gols_time_visitante, @gols_time_casa;

        -- Inserindo o jogo atual em sua respectiva tabela
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

        DECLARE @data_criacao DATE = CONVERT(DATE, @data_criacao_str, 103);

        INSERT INTO Equipe
        VALUES (@nome, @apelido, @data_criacao);
END;