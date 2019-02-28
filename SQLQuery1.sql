CREATE TABLE [dbo].[Task] (
    [Id]                       INT           IDENTITY (1, 1) NOT NULL,
	[OrgId]	INT NOT NULL,
	[StatusUrl]                      VARCHAR (250) NULL,
	[Name] VARCHAR (250) NOT NULL,
    [Status] VARCHAR (250) NULL,
    [Notes] VARCHAR (800) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

