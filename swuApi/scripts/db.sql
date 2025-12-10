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
IF OBJECT_ID('swuDB.dbo.Reviews', 'U') IS NOT NULL DROP TABLE Reviews;
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

INSERT INTO Collections (CollectionName, Color, NumCards, EstimatedValue, CreationDate, IsComplete)
VALUES
('Spark of Rebellion', '#e10600', 252, 500.00, GETDATE(), 0),
('Shadows of the Galaxy', '#3b3fb6', 250, 750.50, GETDATE(), 0);

SELECT * FROM Collections;

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

INSERT INTO Users (Username, Email, PasswordHash, RegistrationDate, IsActive, TotalCollectionValue)
VALUES
('HanShotFirst', 'han@falcon.com', 'hashed_pwd_123', GETDATE(), 1, 550.75),
('LukeJedi', 'luke@jedi.net', 'hashed_pwd_456', GETDATE(), 1, 1200.00);

SELECT * FROM Users;

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

INSERT INTO Packs (PackName, NumberOfCards, ShowcaseRarityOdds, Price, ReleaseDate, CollectionId)
VALUES
('Booster Pack SoR', 16, 288, 4.99, GETDATE(), 1),
('Booster Pack SoG', 16, 288, 4.99, GETDATE(), 2);

SELECT * FROM Packs;

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

INSERT INTO Cards (CardName, Subtitle, Model, Aspect, Rarity, CardNumber, CollectionId, Price, DateAcquired, IsPromo) VALUES
('Luke Skywalker', 'Jedi Knight', 'Showcase', 'Vigilance', 'Uncommon', 5, 1, 15.00, GETDATE(), 0),
('Jedi Guardian', NULL, 'Standard', 'Heroism', 'Common', 1, 1, 0.00, GETDATE(), 0),
('Rebel Scout', NULL, 'Standard', 'Vigilance', 'Common', 2, 1, 0.00, GETDATE(), 0),
('Alliance Sharpshooter', NULL, 'Foil', 'Aggression', 'Uncommon', 3, 1, 1.50, GETDATE(), 0),
('Smuggler’s Gambit', NULL, 'Standard', 'Cunning', 'Common', 4, 1, 0.00, GETDATE(), 0),
('Imperial Defector', NULL, 'Hyperspace', 'Command', 'Rare', 5, 1, 5.00, GETDATE(), 0),
('Blaster Rifle Trooper', NULL, 'Standard', 'Aggression', 'Common', 6, 1, 0.00, GETDATE(), 0),
('Rebel Saboteur', NULL, 'Showcase', 'Cunning', 'Uncommon', 7, 1, 2.50, GETDATE(), 0),
('Jedi Apprentice', NULL, 'Foil', 'Heroism', 'Rare', 8, 1, 4.00, GETDATE(), 0),
('Corellian Pilot', NULL, 'Standard', 'Vigilance', 'Common', 9, 1, 0.00, GETDATE(), 0),
('Alliance Commander', NULL, 'Standard', 'Command', 'Common', 10, 1, 0.00, GETDATE(), 0),
('Senate Envoy', NULL, 'Foil', 'Cunning', 'Uncommon', 11, 1, 1.75, GETDATE(), 0),
('Secret Rebel Weapon', 'Prototype Blaster', 'Standard', 'Aggression', 'Legendary', 12, 1, 0.00, GETDATE(), 1),
('Shadow Assassin', NULL, 'Standard', 'Villainy', 'Common', 1, 2, 0.00, GETDATE(), 0),
('Underworld Informant', NULL, 'Foil', 'Cunning', 'Uncommon', 2, 2, 2.00, GETDATE(), 0),
('Bounty Hunter Recruit', NULL, 'Standard', 'Aggression', 'Common', 3, 2, 0.00, GETDATE(), 0),
('Smuggler Kingpin', NULL, 'Hyperspace', 'Cunning', 'Rare', 4, 2, 6.00, GETDATE(), 0),
('Galactic Spy', NULL, 'Standard', 'Vigilance', 'Common', 5, 2, 0.00, GETDATE(), 0),
('Dark Force Acolyte', NULL, 'Foil', 'Villainy', 'Uncommon', 6, 2, 3.50, GETDATE(), 0),
('Rogue Droid Unit', NULL, 'Standard', 'Command', 'Common', 7, 2, 0.00, GETDATE(), 0),
('Corrupted Admiral', NULL, 'Standard', 'Villainy', 'Rare', 8, 2, 8.00, GETDATE(), 0),
('Smuggler’s Stash', 'Hidden Cargo', 'Standard', 'Cunning', 'Common', 9, 2, 0.00, GETDATE(), 0),
('Galactic Bounty', NULL, 'Standard', 'Aggression', 'Common', 10, 2, 0.00, GETDATE(), 0),
('Shadow Commander', NULL, 'Hyperspace', 'Villainy', 'Legendary', 11, 2, 0.00, GETDATE(), 1),
('Encrypted Holocron', NULL, 'Foil', 'Cunning', 'Uncommon', 12, 2, 4.00, GETDATE(), 0);

SELECT * FROM Cards;

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

INSERT INTO UserCards (UserId, CardId, Copies, DateAdded)
VALUES (1, 1, 2, GETDATE());

INSERT INTO UserCards (UserId, CardId, Copies, DateAdded)
VALUES (2, 2, 1, GETDATE());

INSERT INTO UserCards (UserId, CardId, Copies, DateAdded)
VALUES (1, 3, 5, GETDATE());

SELECT * FROM UserCards;

/*------------- Reviews -------------*/
CREATE TABLE Reviews (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CreationDate DATETIME NOT NULL,
    MessageReview NVARCHAR(100) NULL,
    Stars DECIMAL NOT NULL CHECK (Stars > 1 AND Stars < 5),
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

INSERT INTO Reviews (CreationDate, MessageReview, Stars, UserId)
VALUES
(GETDATE(), 'Buena carta, mucho ataque y compensada blaaaa', 5.0, 1),
(GETDATE(), 'Carta pochilla, ataque flojo y defensa mejorable', 2.5, 2),
(GETDATE(), 'Carta pésima, ni la toquen', 1.0, 1);

SELECT * FROM Reviews;