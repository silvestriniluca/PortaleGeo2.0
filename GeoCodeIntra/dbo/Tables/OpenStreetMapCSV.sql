CREATE TABLE [dbo].[OpenStreetMapCSV] (
    [Id]             INT           NOT NULL,
    [Indirizzo]      VARCHAR (250) NULL,
    [N_Civico]       VARCHAR (250) NULL,
    [Comune]         VARCHAR (250) NULL,
    [IstatComune]    VARCHAR (250) NULL,
    [Provincia]      VARCHAR (250) NULL,
    [IstatProvincia] VARCHAR (250) NULL,
    [Regione]        VARCHAR (250) NULL,
    [IstatRegione]   VARCHAR (250) NULL,
    [Note1]          VARCHAR (250) NULL,
    [Note2]          VARCHAR (250) NULL,
    [Note3]          VARCHAR (250) NULL,
    [Lat]            VARCHAR (250) NULL,
    [Lon]            VARCHAR (250) NULL,
    [Approx01]       VARCHAR (100) NULL,
    [Approx02]       VARCHAR (100) NULL,
    [Cap]            VARCHAR (5)   NULL,
    [AltroIndirizzo] VARCHAR (250) NULL,
    [APIGoogle]      VARCHAR (1)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

