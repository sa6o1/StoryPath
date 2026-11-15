/*
Платформа за интерактивни истории — начална база за Microsoft SQL Server.
Изпълнение: DBeaver > SQL Editor > Open SQL Script > Execute SQL Script.
Необходим е работещ SQL Server и право CREATE DATABASE.
Скриптът НЕ изтрива съществуващи данни. Спира, ако има таблици в целевата база.
Включва Identity таблици, шест таблици за истории/напредък и три жанра.
Няма демонстрационни пароли. Потребители се създават чрез Identity UserManager.

ВАЖНО ЗА EF CORE: това е самостоятелен, ръчно подготвен SQL скрипт,
а не генерирана миграция. Не създава __EFMigrationsHistory.
За Code First използвайте миграциите върху ДРУГА празна база или предварително
съгласувайте baseline. Не прилагайте InitialCreate върху тези таблици.
Не е изпълнен срещу жив SQL Server в средата на подготовка.
*/
IF DB_ID(N'InteractiveStoriesDb') IS NULL
    EXEC(N'CREATE DATABASE [InteractiveStoriesDb]');

-- Динамичният batch задава контекста без разделител GO (удобно за DBeaver).
EXEC(N'
USE [InteractiveStoriesDb];
SET XACT_ABORT ON;
IF EXISTS (SELECT 1 FROM sys.tables WHERE is_ms_shipped = 0)
    THROW 50001, N''Базата вече съдържа таблици. Скриптът е предназначен за празна база.'', 1;
BEGIN TRY
BEGIN TRANSACTION;

CREATE TABLE dbo.AspNetUsers (
    Id nvarchar(450) NOT NULL PRIMARY KEY,
    DisplayName nvarchar(100) NOT NULL,
    Bio nvarchar(1000) NULL,
    UserName nvarchar(256) NULL,
    NormalizedUserName nvarchar(256) NULL,
    Email nvarchar(256) NULL,
    NormalizedEmail nvarchar(256) NULL,
    EmailConfirmed bit NOT NULL,
    PasswordHash nvarchar(max) NULL,
    SecurityStamp nvarchar(max) NULL,
    ConcurrencyStamp nvarchar(max) NULL,
    PhoneNumber nvarchar(max) NULL,
    PhoneNumberConfirmed bit NOT NULL,
    TwoFactorEnabled bit NOT NULL,
    LockoutEnd datetimeoffset NULL,
    LockoutEnabled bit NOT NULL,
    AccessFailedCount int NOT NULL
);

CREATE TABLE dbo.AspNetRoles (
    Id nvarchar(450) NOT NULL PRIMARY KEY,
    Name nvarchar(256) NULL,
    NormalizedName nvarchar(256) NULL,
    ConcurrencyStamp nvarchar(max) NULL
);

CREATE TABLE dbo.AspNetUserRoles (
    UserId nvarchar(450) NOT NULL,
    RoleId nvarchar(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.AspNetUserClaims (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId nvarchar(450) NOT NULL,
    ClaimType nvarchar(max) NULL,
    ClaimValue nvarchar(max) NULL,
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.AspNetRoleClaims (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    RoleId nvarchar(450) NOT NULL,
    ClaimType nvarchar(max) NULL,
    ClaimValue nvarchar(max) NULL,
    FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.AspNetUserLogins (
    LoginProvider nvarchar(128) NOT NULL,
    ProviderKey nvarchar(128) NOT NULL,
    ProviderDisplayName nvarchar(max) NULL,
    UserId nvarchar(450) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.AspNetUserTokens (
    UserId nvarchar(450) NOT NULL,
    LoginProvider nvarchar(128) NOT NULL,
    Name nvarchar(128) NOT NULL,
    Value nvarchar(max) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE dbo.Genres (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name nvarchar(50) NOT NULL
);

CREATE TABLE dbo.Stories (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title nvarchar(150) NOT NULL,
    Description nvarchar(2000) NOT NULL,
    CreatedAt datetime2 NOT NULL,
    IsPublished bit NOT NULL,
    AuthorId nvarchar(450) NOT NULL,
    GenreId int NOT NULL,
    StartSceneId int NULL,
    FOREIGN KEY (AuthorId) REFERENCES dbo.AspNetUsers(Id),
    FOREIGN KEY (GenreId) REFERENCES dbo.Genres(Id)
);

CREATE TABLE dbo.Chapters (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title nvarchar(150) NOT NULL,
    OrderNumber int NOT NULL,
    StoryId int NOT NULL,
    FOREIGN KEY (StoryId) REFERENCES dbo.Stories(Id)
);

CREATE TABLE dbo.Scenes (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title nvarchar(150) NOT NULL,
    Content nvarchar(max) NOT NULL,
    IsEnding bit NOT NULL,
    ChapterId int NOT NULL,
    FOREIGN KEY (ChapterId) REFERENCES dbo.Chapters(Id)
);

ALTER TABLE dbo.Stories ADD CONSTRAINT FK_Stories_StartScene
    FOREIGN KEY (StartSceneId) REFERENCES dbo.Scenes(Id);

CREATE TABLE dbo.Choices (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Text nvarchar(300) NOT NULL,
    OrderNumber int NOT NULL,
    FromSceneId int NOT NULL,
    ToSceneId int NOT NULL,
    FOREIGN KEY (FromSceneId) REFERENCES dbo.Scenes(Id),
    FOREIGN KEY (ToSceneId) REFERENCES dbo.Scenes(Id)
);

CREATE TABLE dbo.ReadingProgresses (
    Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId nvarchar(450) NOT NULL,
    StoryId int NOT NULL,
    CurrentSceneId int NOT NULL,
    IsCompleted bit NOT NULL,
    UpdatedAt datetime2 NOT NULL,
    FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    FOREIGN KEY (StoryId) REFERENCES dbo.Stories(Id),
    FOREIGN KEY (CurrentSceneId) REFERENCES dbo.Scenes(Id)
);

CREATE UNIQUE INDEX UserNameIndex ON dbo.AspNetUsers (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
CREATE INDEX EmailIndex ON dbo.AspNetUsers (NormalizedEmail);
CREATE UNIQUE INDEX RoleNameIndex ON dbo.AspNetRoles (NormalizedName) WHERE NormalizedName IS NOT NULL;
CREATE UNIQUE INDEX IX_Genres_Name ON dbo.Genres (Name);
CREATE UNIQUE INDEX IX_Chapters_StoryId_OrderNumber ON dbo.Chapters (StoryId, OrderNumber);
CREATE UNIQUE INDEX IX_Choices_FromSceneId_OrderNumber ON dbo.Choices (FromSceneId, OrderNumber);
CREATE UNIQUE INDEX IX_ReadingProgresses_UserId_StoryId ON dbo.ReadingProgresses (UserId, StoryId);
CREATE INDEX IX_Stories_AuthorId ON dbo.Stories (AuthorId);
CREATE INDEX IX_Stories_GenreId ON dbo.Stories (GenreId);
CREATE INDEX IX_Stories_StartSceneId ON dbo.Stories (StartSceneId);
CREATE INDEX IX_Scenes_ChapterId ON dbo.Scenes (ChapterId);
CREATE INDEX IX_Choices_ToSceneId ON dbo.Choices (ToSceneId);
CREATE INDEX IX_ReadingProgresses_StoryId ON dbo.ReadingProgresses (StoryId);
CREATE INDEX IX_ReadingProgresses_CurrentSceneId ON dbo.ReadingProgresses (CurrentSceneId);
CREATE INDEX IX_AspNetUserRoles_RoleId ON dbo.AspNetUserRoles (RoleId);
CREATE INDEX IX_AspNetUserClaims_UserId ON dbo.AspNetUserClaims (UserId);
CREATE INDEX IX_AspNetRoleClaims_RoleId ON dbo.AspNetRoleClaims (RoleId);
CREATE INDEX IX_AspNetUserLogins_UserId ON dbo.AspNetUserLogins (UserId);
INSERT INTO dbo.Genres (Name) VALUES (N''Приключенски''), (N''Фентъзи''), (N''Мистерия'');
COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;

-- Проверки: очакват се 13 таблици и 3 жанра.
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = N''dbo'' AND TABLE_TYPE = N''BASE TABLE''
ORDER BY TABLE_NAME;
SELECT Id, Name FROM dbo.Genres ORDER BY Id;

-- Бизнес проверки за бъдещите услуги:
-- Началната и текущата сцена трябва да принадлежат на съответната история.
-- Изходната и целевата сцена на избор трябва да са от една история.
-- Преди публикуване проверявайте достижимостта на сцените и наличието на финал.

');
