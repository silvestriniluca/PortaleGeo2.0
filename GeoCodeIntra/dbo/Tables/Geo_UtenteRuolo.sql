CREATE TABLE [dbo].[Geo_UtenteRuolo] (
    [Id]     NVARCHAR (50)  NOT NULL,
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__Utent__3214EC0760D5CC4C] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK__Utente__RoleI__0D7A0286] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Geo_Ruolo] ([Id]),
    CONSTRAINT [FK__Utente__UserI__0E6E26BF] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Geo_Utente] ([Id])
);

