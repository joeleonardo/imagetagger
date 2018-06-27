
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/27/2018 20:14:11
-- Generated from EDMX file: C:\Users\conta\documents\visual studio 2017\Projects\ImageFileSorter\ImageReviewer\Schema.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ImagesImageTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTags] DROP CONSTRAINT [FK_ImagesImageTags];
GO
IF OBJECT_ID(N'[dbo].[FK_TagsImageTags]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ImageTags] DROP CONSTRAINT [FK_TagsImageTags];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Images]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Images];
GO
IF OBJECT_ID(N'[dbo].[Tags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tags];
GO
IF OBJECT_ID(N'[dbo].[ImageTags]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ImageTags];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Images'
CREATE TABLE [dbo].[Images] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FullPath] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TagName] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ImageTags'
CREATE TABLE [dbo].[ImageTags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Image_Id] int  NOT NULL,
    [Tag_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Images'
ALTER TABLE [dbo].[Images]
ADD CONSTRAINT [PK_Images]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ImageTags'
ALTER TABLE [dbo].[ImageTags]
ADD CONSTRAINT [PK_ImageTags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Image_Id] in table 'ImageTags'
ALTER TABLE [dbo].[ImageTags]
ADD CONSTRAINT [FK_ImagesImageTags]
    FOREIGN KEY ([Image_Id])
    REFERENCES [dbo].[Images]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ImagesImageTags'
CREATE INDEX [IX_FK_ImagesImageTags]
ON [dbo].[ImageTags]
    ([Image_Id]);
GO

-- Creating foreign key on [Tag_Id] in table 'ImageTags'
ALTER TABLE [dbo].[ImageTags]
ADD CONSTRAINT [FK_TagsImageTags]
    FOREIGN KEY ([Tag_Id])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TagsImageTags'
CREATE INDEX [IX_FK_TagsImageTags]
ON [dbo].[ImageTags]
    ([Tag_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------