-- Creo la Base de Datos (solo si no existe)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'swuDB')
    CREATE DATABASE swuDB;
GO

USE swuDB;
GO
SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'swuDB';

/* Función Helper para eliminar tablas en orden si existen, respetando las FK */
IF OBJECT_ID('swuDB.dbo.UserCards', 'U') IS NOT NULL DROP TABLE UserCards;
IF OBJECT_ID('swuDB.dbo.Cards', 'U') IS NOT NULL DROP TABLE Cards;
IF OBJECT_ID('swuDB.dbo.Packs', 'U') IS NOT NULL DROP TABLE Packs;
IF OBJECT_ID('swuDB.dbo.Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('swuDB.dbo.Collections', 'U') IS NOT NULL DROP TABLE Collections;

/*------------- Colecciones -------------*/
CREATE TABLE Collections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CollectionName NVARCHAR(100) NOT NULL,
    Color NVARCHAR(100),
    NumCards INT NOT NULL DEFAULT 0,
    EstimatedValue DECIMAL(10, 2) NOT NULL CHECK (EstimatedValue >= 0),
    CreationDate DATETIME NOT NULL,
    IsComplete BIT NOT NULL DEFAULT 0
);

/*------------- Usuarios -------------*/
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE, 
    Email NVARCHAR(100) NOT NULL UNIQUE,  
    PasswordHash NVARCHAR(256) NOT NULL, 
    RegistrationDate DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    TotalCollectionValue DECIMAL(10, 2) NOT NULL CHECK (TotalCollectionValue >= 0) DEFAULT 0
);

/*------------- Sobres -------------*/
CREATE TABLE Packs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PackName NVARCHAR(100) NOT NULL,
    NumberOfCards INT NOT NULL CHECK (NumberOfCards = 16) DEFAULT 16,
    ShowcaseRarityOdds INT NOT NULL CHECK (ShowcaseRarityOdds >= 1),
    GuaranteesRare BIT NOT NULL DEFAULT 1,
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    ReleaseDate DATETIME NOT NULL,
    CollectionId INT NOT NULL,
    FOREIGN KEY (CollectionId) REFERENCES Collections(Id)
);

/*------------- Cartas -------------*/
CREATE TABLE Cards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CardName NVARCHAR(100) NOT NULL,
    Subtitle NVARCHAR(100) NULL,
    Model NVARCHAR(50) NOT NULL DEFAULT 'Standard',
    Aspect NVARCHAR(50) NULL,
    Rarity NVARCHAR(50) NOT NULL DEFAULT 'Common',
    CardNumber INT NOT NULL,
    CollectionId INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    DateAcquired DATETIME NOT NULL,
    IsPromo BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CollectionId) REFERENCES Collections(Id)
);

/*------------- Inventario Personal -------------*/
CREATE TABLE UserCards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    CardId INT NOT NULL,
    Copies INT NOT NULL CHECK (Copies >= 1) DEFAULT 1,
    DateAdded DATETIME NOT NULL, 
    IsFavorite BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT UQ_UserCard UNIQUE (UserId, CardId),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CardId) REFERENCES Cards(Id)
);