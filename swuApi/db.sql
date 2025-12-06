-- Creo la Base de Datos (solo si no existe)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'swuDB')
    CREATE DATABASE swuDB;

USE swuDB;


SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'swuDB';


-- Función Helper para eliminar tablas en orden si existen, respetando las FK
IF OBJECT_ID('swuDB.dbo.Cards', 'U') IS NOT NULL DROP TABLE Cards;
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

INSERT INTO Collections (CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete)
VALUES
('Spark of Rebellion', '#e10600', 252, 500.00, GETDATE(), 0),
('Shadows of the Galaxy', '#3b3fb6', 250, 750.50, GETDATE(), 0);

SELECT * FROM Collections;


/*------------- Cartas -------------*/

CREATE TABLE Cards (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CardName NVARCHAR(100) NOT NULL,
    Subtitle NVARCHAR(100) NULL,
    Model NVARCHAR(50) NOT NULL,
    Aspect NVARCHAR(50) NULL,
    CardNumber INT NOT NULL,
    Copies INT NOT NULL DEFAULT 0,
    CollectionId INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL CHECK (Price >= 0),
    DateAcquired DATETIME NOT NULL,
    IsPromo BIT NOT NULL DEFAULT 0,
    
    FOREIGN KEY (CollectionId) REFERENCES Collections(Id)
);

INSERT INTO Cards (CardName, Subtitle, Model, Aspect, CardNumber, Copies, CollectionId, Price, DateAcquired, IsPromo)
VALUES
('Luke Skywalker', 'Jedi Knight', 'Unit', 'Vigilance', 5, 1, 1, 15.00, GETDATE(), 0),
('Darth Vader', 'Dark Lord', 'Unit', 'Command', 1, 1, 1, 30.50, GETDATE(), 0),
('Fighter Wing', NULL, 'Starship', 'Aggression', 150, 2, 2, 5.00, GETDATE(), 0),
('Moff Gideon', 'Imperial Commander', 'Leader', 'Command', 10, 1, 2, 20.00, GETDATE(), 1);

SELECT * FROM Cards;
