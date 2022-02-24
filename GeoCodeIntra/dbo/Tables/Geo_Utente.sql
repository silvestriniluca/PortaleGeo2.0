CREATE TABLE [dbo].[Geo_Utente] (
    [Id]                   NVARCHAR (128) NOT NULL,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME       NULL,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR (256) NOT NULL,
    [Cognome]              VARCHAR (50)   NULL,
    [Nome]                 VARCHAR (50)   NULL,
    [CodiceFiscale]        VARCHAR (50)   NULL,
    [IdRuolo]              NVARCHAR (50)  NULL,
    CONSTRAINT [PK_dbo.Utenti] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Geo_Utente_Geo_Ruolo] FOREIGN KEY ([IdRuolo]) REFERENCES [dbo].[Geo_Ruolo] ([Id])
);


