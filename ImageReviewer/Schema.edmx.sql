
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/27/2018 21:18:30
-- Generated from EDMX file: C:\Users\conta\Documents\Visual Studio 2017\Projects\ImageFileSorter\ImageReviewer\Schema.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
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
IF OBJECT_ID(N'[dbo].[FK_CollectionCollectionPage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CollectionPages] DROP CONSTRAINT [FK_CollectionCollectionPage];
GO
IF OBJECT_ID(N'[dbo].[FK_CollectionPageImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Images] DROP CONSTRAINT [FK_CollectionPageImage];
GO
IF OBJECT_ID(N'[dbo].[FK_ArtistImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Images] DROP CONSTRAINT [FK_ArtistImage];
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
IF OBJECT_ID(N'[dbo].[Collections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Collections];
GO
IF OBJECT_ID(N'[dbo].[CollectionPages]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CollectionPages];
GO
IF OBJECT_ID(N'[dbo].[Artists]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Artists];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Images'
CREATE TABLE [dbo].[Images] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [File_Name] nvarchar(max)  NOT NULL,
    [Full_Path] nvarchar(max)  NOT NULL,
    [CollectionPage_Id] int  NULL,
    [Artist_Id] int  NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ImageTags'
CREATE TABLE [dbo].[ImageTags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Image_Id] int  NOT NULL,
    [Tag_Id] int  NOT NULL
);
GO

-- Creating table 'Collections'
CREATE TABLE [dbo].[Collections] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CollectionPages'
CREATE TABLE [dbo].[CollectionPages] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Order] nvarchar(max)  NOT NULL,
    [Collection_Id] int  NOT NULL
);
GO

-- Creating table 'Artists'
CREATE TABLE [dbo].[Artists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
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

-- Creating primary key on [Id] in table 'Collections'
ALTER TABLE [dbo].[Collections]
ADD CONSTRAINT [PK_Collections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CollectionPages'
ALTER TABLE [dbo].[CollectionPages]
ADD CONSTRAINT [PK_CollectionPages]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Artists'
ALTER TABLE [dbo].[Artists]
ADD CONSTRAINT [PK_Artists]
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

-- Creating foreign key on [Collection_Id] in table 'CollectionPages'
ALTER TABLE [dbo].[CollectionPages]
ADD CONSTRAINT [FK_CollectionCollectionPage]
    FOREIGN KEY ([Collection_Id])
    REFERENCES [dbo].[Collections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CollectionCollectionPage'
CREATE INDEX [IX_FK_CollectionCollectionPage]
ON [dbo].[CollectionPages]
    ([Collection_Id]);
GO

-- Creating foreign key on [CollectionPage_Id] in table 'Images'
ALTER TABLE [dbo].[Images]
ADD CONSTRAINT [FK_CollectionPageImage]
    FOREIGN KEY ([CollectionPage_Id])
    REFERENCES [dbo].[CollectionPages]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CollectionPageImage'
CREATE INDEX [IX_FK_CollectionPageImage]
ON [dbo].[Images]
    ([CollectionPage_Id]);
GO

-- Creating foreign key on [Artist_Id] in table 'Images'
ALTER TABLE [dbo].[Images]
ADD CONSTRAINT [FK_ArtistImage]
    FOREIGN KEY ([Artist_Id])
    REFERENCES [dbo].[Artists]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ArtistImage'
CREATE INDEX [IX_FK_ArtistImage]
ON [dbo].[Images]
    ([Artist_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------