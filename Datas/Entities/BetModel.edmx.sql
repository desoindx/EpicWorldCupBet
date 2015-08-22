
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/18/2015 18:51:32
-- Generated from EDMX file: D:\Git\EpicWorldCupBet\Datas\Entities\BetModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EpicSportExchange];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[CompetitionGames]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompetitionGames];
GO
IF OBJECT_ID(N'[dbo].[CompetitionPrizes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompetitionPrizes];
GO
IF OBJECT_ID(N'[dbo].[CompetitionResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CompetitionResults];
GO
IF OBJECT_ID(N'[dbo].[Competitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Competitions];
GO
IF OBJECT_ID(N'[dbo].[Moneys]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Moneys];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[ResultOverridedValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ResultOverridedValues];
GO
IF OBJECT_ID(N'[dbo].[Results]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Results];
GO
IF OBJECT_ID(N'[dbo].[Teams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Teams];
GO
IF OBJECT_ID(N'[dbo].[Trades]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Trades];
GO
IF OBJECT_ID(N'[dbo].[UniverseAvailables]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UniverseAvailables];
GO
IF OBJECT_ID(N'[dbo].[UniverseCompetitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UniverseCompetitions];
GO
IF OBJECT_ID(N'[dbo].[Universes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Universes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Moneys'
CREATE TABLE [dbo].[Moneys] (
    [User] varchar(50)  NOT NULL,
    [Money1] int  NOT NULL,
    [IdUniverseCompetition] int  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [User] nvarchar(50)  NOT NULL,
    [Date] datetime  NOT NULL,
    [Team] int  NOT NULL,
    [Quantity] int  NOT NULL,
    [Status] int  NOT NULL,
    [Price] int  NOT NULL,
    [Side] nchar(10)  NULL,
    [IdUniverseCompetition] int  NOT NULL
);
GO

-- Creating table 'Trades'
CREATE TABLE [dbo].[Trades] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Buyer] nvarchar(50)  NOT NULL,
    [Seller] nvarchar(50)  NOT NULL,
    [Date] datetime  NOT NULL,
    [Team] int  NOT NULL,
    [Quantity] int  NOT NULL,
    [Price] int  NOT NULL,
    [IdUniverseCompetition] int  NOT NULL
);
GO

-- Creating table 'Competitions'
CREATE TABLE [dbo].[Competitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Tie] float  NOT NULL,
    [Score] float  NOT NULL
);
GO

-- Creating table 'Teams'
CREATE TABLE [dbo].[Teams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [IdCompetition] int  NOT NULL,
    [Result] int  NULL,
    [RealTeam] bit  NULL,
    [Strength] float  NULL
);
GO

-- Creating table 'Universes'
CREATE TABLE [dbo].[Universes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [AdminUser] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UniverseCompetitions'
CREATE TABLE [dbo].[UniverseCompetitions] (
    [IdUniverse] int  NOT NULL,
    [IdCompetition] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'UniverseAvailables'
CREATE TABLE [dbo].[UniverseAvailables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdUniverse] int  NOT NULL,
    [UserName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Results'
CREATE TABLE [dbo].[Results] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [DefaultValue] int  NOT NULL
);
GO

-- Creating table 'ResultOverridedValues'
CREATE TABLE [dbo].[ResultOverridedValues] (
    [Id] int  NOT NULL,
    [Value] int  NOT NULL,
    [IdUniverseCompetition] int  NOT NULL
);
GO

-- Creating table 'CompetitionGames'
CREATE TABLE [dbo].[CompetitionGames] (
    [RoundKey] varchar(100)  NOT NULL,
    [TeamId] int  NOT NULL,
    [CompetitionId] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'CompetitionPrizes'
CREATE TABLE [dbo].[CompetitionPrizes] (
    [RoundKey] varchar(100)  NOT NULL,
    [PrizeName] varchar(100)  NOT NULL,
    [Value] float  NOT NULL,
    [CompetitionId] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'CompetitionResults'
CREATE TABLE [dbo].[CompetitionResults] (
    [RoundKey] varchar(100)  NOT NULL,
    [Result] varchar(max)  NOT NULL,
    [CompetitionId] int  NOT NULL,
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [User], [IdUniverseCompetition] in table 'Moneys'
ALTER TABLE [dbo].[Moneys]
ADD CONSTRAINT [PK_Moneys]
    PRIMARY KEY CLUSTERED ([User], [IdUniverseCompetition] ASC);
GO

-- Creating primary key on [Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Trades'
ALTER TABLE [dbo].[Trades]
ADD CONSTRAINT [PK_Trades]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Competitions'
ALTER TABLE [dbo].[Competitions]
ADD CONSTRAINT [PK_Competitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Teams'
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [PK_Teams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Universes'
ALTER TABLE [dbo].[Universes]
ADD CONSTRAINT [PK_Universes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UniverseCompetitions'
ALTER TABLE [dbo].[UniverseCompetitions]
ADD CONSTRAINT [PK_UniverseCompetitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UniverseAvailables'
ALTER TABLE [dbo].[UniverseAvailables]
ADD CONSTRAINT [PK_UniverseAvailables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Results'
ALTER TABLE [dbo].[Results]
ADD CONSTRAINT [PK_Results]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id], [IdUniverseCompetition] in table 'ResultOverridedValues'
ALTER TABLE [dbo].[ResultOverridedValues]
ADD CONSTRAINT [PK_ResultOverridedValues]
    PRIMARY KEY CLUSTERED ([Id], [IdUniverseCompetition] ASC);
GO

-- Creating primary key on [Id] in table 'CompetitionGames'
ALTER TABLE [dbo].[CompetitionGames]
ADD CONSTRAINT [PK_CompetitionGames]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompetitionPrizes'
ALTER TABLE [dbo].[CompetitionPrizes]
ADD CONSTRAINT [PK_CompetitionPrizes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CompetitionResults'
ALTER TABLE [dbo].[CompetitionResults]
ADD CONSTRAINT [PK_CompetitionResults]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------