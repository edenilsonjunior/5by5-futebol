USE DBFutebol;

INSERT INTO Campeonato(nome, temporada) VALUES 
('Brasileirao', '2024/1'),
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


select * from Estatistica WHERE temp_camp = '2024/1' order by pontos DESC, gols_marcados DESC;
select * from Estatistica WHERE temp_camp = '2024/2' order by pontos DESC, gols_marcados DESC;