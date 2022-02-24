CREATE TABLE [dbo].[Geo_Attività] (
    [IdGeo]           INT            IDENTITY (1, 1) NOT NULL,
    [Utente]          NVARCHAR (256) NULL,
    [DescrizioneFile] NVARCHAR (256) NULL,
    [PathFile]        NVARCHAR (256) NULL,
    [Here]            BIT            NULL,
    [OpenStreetMap]   BIT            NULL,
    [Id_Utente]       NVARCHAR (128) NULL,
    CONSTRAINT [PK__Geo_Attività__0C9F839F17C3A3EB] PRIMARY KEY CLUSTERED ([IdGeo] ASC),
    CONSTRAINT [FK_Geo_Attività_Geo_Utente] FOREIGN KEY ([Id_Utente]) REFERENCES [dbo].[Geo_Utente] ([Id])
);

