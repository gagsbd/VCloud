/****** Object: Table [dbo].[Org] Script Date: 3/6/2019 8:33:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Org] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [CustomerPaymentProfileId] NVARCHAR (MAX) NULL,
    [CustomerProfileId]        NVARCHAR (MAX) NULL,
    [CompanyShortName]         NVARCHAR (MAX) NULL,
    [CompanyFullName]          NVARCHAR (MAX) NULL,
    [EmailAddress]             NVARCHAR (MAX) NULL,
    [Address_Zip]              NVARCHAR (MAX) NULL,
    [Address_Address1]         NVARCHAR (MAX) NULL,
    [Address_Address2]         NVARCHAR (MAX) NULL,
    [Address_City]             NVARCHAR (MAX) NULL,
    [Address_State]            NVARCHAR (MAX) NULL,
    [Address_Country]          NVARCHAR (MAX) NULL,
    [TenantId]                 NVARCHAR (MAX) NULL,
    [AdminName]                NVARCHAR (MAX) NULL,
    [AdminPassword]            NVARCHAR (MAX) NULL,
    [Cloud_TenantId]           NVARCHAR (MAX) NULL,
    [UpdateTime]               DATETIME       NOT NULL
);


/****** Object: Table [dbo].[Task] Script Date: 3/6/2019 8:34:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Task] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [OrgId]       INT            NOT NULL,
    [Name]        NVARCHAR (MAX) NULL,
    [StatusUrl]   NVARCHAR (MAX) NULL,
    [Status]      NVARCHAR (MAX) NULL,
    [Notes]       NVARCHAR (MAX) NULL,
    [TaskType]    INT            NOT NULL,
    [IsLRP]       BIT            NOT NULL,
    [TaskCode]    NVARCHAR (MAX) NULL,
    [Predecessor] NVARCHAR (MAX) NULL,
    [UpdateTime]  DATETIME       NOT NULL
);