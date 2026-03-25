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
    DataInicioLeitura DATETIME2 NULL,
    DataFimLeitura DATETIME2 NULL,
    CONSTRAINT FK_Avaliacao_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuario(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Avaliacao_Livro FOREIGN KEY (IdLivro) REFERENCES dbo.Livro(Id) ON DELETE CASCADE
);
GO

-- NotaMedia is calculated and updated by application code (AvaliacaoService)
-- No trigger needed — keeps logic explicit and testable