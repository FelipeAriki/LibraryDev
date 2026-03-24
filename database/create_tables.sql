-- SQL Server script to create tables for Usuario, Livro and Avaliacao
-- Run on a SQL Server database

SET NOCOUNT ON;
GO

-- Create Usuario
IF OBJECT_ID('dbo.Usuario', 'U') IS NOT NULL
    DROP TABLE dbo.Usuario;
GO

CREATE TABLE dbo.Usuario
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(320) NOT NULL,
    Nome NVARCHAR(200) NOT NULL,
    Senha NVARCHAR(255) NOT NULL,
    RefreshToken NVARCHAR(500) NULL,
    RefreshTokenExpiracao DATETIME2 NULL,
    TokenRecuperacaoSenha NVARCHAR(500) NULL,
    TokenRecuperacaoExpiracao DATETIME2 NULL
);
GO

-- Create Livro
IF OBJECT_ID('dbo.Livro', 'U') IS NOT NULL
    DROP TABLE dbo.Livro;
GO

CREATE TABLE dbo.Livro
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(500) NOT NULL,
    Descricao NVARCHAR(MAX) NULL,
    ISBN NVARCHAR(50) NULL,
    Autor NVARCHAR(200) NULL,
    Editora NVARCHAR(200) NULL,
    Genero TINYINT NOT NULL,
    AnoDePublicacao INT NULL,
    QuantidadePaginas INT NULL,
    DataCriacao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    NotaMedia DECIMAL(5,2) NOT NULL DEFAULT (0),
    CapaLivro VARBINARY(MAX) NULL
);
GO

CREATE INDEX IX_Livro_Titulo ON dbo.Livro(Titulo);
GO

-- Create Avaliacao
IF OBJECT_ID('dbo.Avaliacao', 'U') IS NOT NULL
    DROP TABLE dbo.Avaliacao;
GO

CREATE TABLE dbo.Avaliacao
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nota TINYINT NOT NULL CHECK (Nota BETWEEN 1 AND 5),
    Descricao NVARCHAR(MAX) NULL,
    IdUsuario INT NOT NULL,
    IdLivro INT NOT NULL,
    DataCriacao DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Avaliacao_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Avaliacao_Livro FOREIGN KEY (IdLivro) REFERENCES dbo.Livro(Id) ON DELETE CASCADE
);
GO

-- Trigger to update Livro.NotaMedia automatically when Avaliacao is inserted/updated/deleted
IF OBJECT_ID('dbo.TR_Avaliacao_NotaMedia_Update', 'TR') IS NOT NULL
    DROP TRIGGER dbo.TR_Avaliacao_NotaMedia_Update;
GO

CREATE TRIGGER dbo.TR_Avaliacao_NotaMedia_Update
ON dbo.Avaliacao
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH AffectedBooks AS
    (
        SELECT IdLivro FROM inserted
        UNION
        SELECT IdLivro FROM deleted
    )
    UPDATE L
    SET NotaMedia = ISNULL(S.AvgNota, 0)
    FROM dbo.Livro L
    INNER JOIN AffectedBooks AB ON L.Id = AB.IdLivro
    LEFT JOIN
    (
        SELECT IdLivro, CAST(AVG(CAST(Nota AS DECIMAL(5,2))) AS DECIMAL(5,2)) AS AvgNota
        FROM dbo.Avaliacao
        WHERE IdLivro IN (SELECT IdLivro FROM AffectedBooks)
        GROUP BY IdLivro
    ) S ON S.IdLivro = L.Id;
END;
GO

-- Optional: stored procedure to recalculate all averages (useful for maintenance)
IF OBJECT_ID('dbo.sp_RecalculaNotas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RecalculaNotas;
GO

CREATE PROCEDURE dbo.sp_RecalculaNotas
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE L
    SET NotaMedia = ISNULL(S.AvgNota, 0)
    FROM dbo.Livro L
    LEFT JOIN (
        SELECT IdLivro, CAST(AVG(CAST(Nota AS DECIMAL(5,2))) AS DECIMAL(5,2)) AS AvgNota
        FROM dbo.Avaliacao
        GROUP BY IdLivro
    ) S ON S.IdLivro = L.Id;
END;
GO
