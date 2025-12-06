CREATE DATABASE swuDB;

SELECT name, database_id, create_date 
FROM sys.databases 
WHERE name = 'swuDB';

USE swuDB;

/*------------- Colecciones -------------*/
CREATE TABLE Collections (
    Id INT PRIMARY KEY,
    CollectionName NVARCHAR(100) NOT NULL,
    Color NVARCHAR(100),
    NumCards INT NOT NULL
);

SELECT * FROM Collections;

/*------------- Cartas -------------*/
CREATE TABLE Cards (
    Id INT PRIMARY KEY,
    CardName NVARCHAR(100) NOT NULL,
    Subtitle NVARCHAR(100),
    Model NVARCHAR(50) NOT NULL,
    Aspect NVARCHAR(50),
    CardNumber INT NOT NULL,
    Copies INT NOT NULL DEFAULT 0,
    CollectionId INT NOT NULL,
    FOREIGN KEY (CollectionId) REFERENCES Collections(Id)
);

SELECT * FROM Cards;