
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 11/09/2011 06:52:44
-- Generated from EDMX file: D:\SIG\Obligatorio2\SIGTest\USARoadTrip.Api\EntityModels\USARoadTripDataModel.edmx
-- --------------------------------------------------

CREATE DATABASE [roadtrip];
GO

SET QUOTED_IDENTIFIER OFF;
GO
USE [roadtrip];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserTrips]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trips] DROP CONSTRAINT [FK_UserTrips];
GO
IF OBJECT_ID(N'[dbo].[FK_TripDestinations]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Addresses] DROP CONSTRAINT [FK_TripDestinations];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Trips]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Trips];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[Addresses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Addresses];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Trips'
CREATE TABLE [dbo].[Trips] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserNick] nvarchar(25)  NOT NULL,
    [Name] nvarchar(25)  NOT NULL,
    [Description] nvarchar(255)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Nick] nvarchar(25)  NOT NULL,
    [Password] nvarchar(25)  NOT NULL
);
GO

-- Creating table 'Addresses'
CREATE TABLE [dbo].[Addresses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Address] nvarchar(75)  NOT NULL,
    [City] nvarchar(50)  NOT NULL,
    [State] nchar(2)  NOT NULL,
    [Zip] nvarchar(10)  NOT NULL,
    [TripOrder] int  NULL,
    [TripDestinations_Location_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Trips'
ALTER TABLE [dbo].[Trips]
ADD CONSTRAINT [PK_Trips]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Nick] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Nick] ASC);
GO

-- Creating primary key on [Id] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [PK_Addresses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserNick] in table 'Trips'
ALTER TABLE [dbo].[Trips]
ADD CONSTRAINT [FK_UserTrips]
    FOREIGN KEY ([UserNick])
    REFERENCES [dbo].[Users]
        ([Nick])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTrips'
CREATE INDEX [IX_FK_UserTrips]
ON [dbo].[Trips]
    ([UserNick]);
GO

-- Creating foreign key on [TripDestinations_Location_Id] in table 'Addresses'
ALTER TABLE [dbo].[Addresses]
ADD CONSTRAINT [FK_TripDestinations]
    FOREIGN KEY ([TripDestinations_Location_Id])
    REFERENCES [dbo].[Trips]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TripDestinations'
CREATE INDEX [IX_FK_TripDestinations]
ON [dbo].[Addresses]
    ([TripDestinations_Location_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------