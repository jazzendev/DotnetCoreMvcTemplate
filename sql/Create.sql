SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--DROP Existing table and recreate
DROP TABLE IF EXISTS [dbo].[Logs]
DROP TABLE IF EXISTS [dbo].[PortalLogins]
DROP TABLE IF EXISTS [dbo].[PortalRoles]
DROP TABLE IF EXISTS [dbo].[PortalLoginRoles]
GO


CREATE TABLE [dbo].[Logs] (
   [ID] [int] IDENTITY(1,1) NOT NULL,
   [Logged] [datetime] NOT NULL,
   [Level] [varchar](5) NOT NULL,
   [UserName] [nvarchar](200) NULL,
   [Message] [nvarchar](max) NOT NULL,
   [Properties] [nvarchar](max) NULL,
   [ServerName] [nvarchar](200) NULL,
   [Port] [nvarchar](100) NULL,
   [Url] [nvarchar](2000) NULL,
   [Https] [bit] NULL,
   [ServerAddress] [nvarchar](100) NULL,
   [RemoteAddress] [nvarchar](100) NULL,
   [Callsite] [nvarchar](300) NULL,
   [Exception] [nvarchar](max) NULL,
   [UserIdentity] [nvarchar](200) NULL,
   [Controller] [nvarchar](200) NULL,
   [Action] [nvarchar](200) NULL,
   [Operation] [nvarchar](200) NULL,
   [OperationCode] [nvarchar](100) NULL,
   [OperatorId] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Logs] PRIMARY KEY CLUSTERED ([ID] ASC) 
   WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

-------------------- Portal Logins --------------------

CREATE TABLE [dbo].[PortalLogins] (
    [Id]                NVARCHAR (50)   NOT NULL,
    
    [Username]          NVARCHAR (50)   NULL,
	[Domain]	        NVARCHAR (50)   NULL,
    [PasswordHash]      NVARCHAR (MAX)  NULL,
    [IsPasswordToChange]   BIT             NOT NULL,

	[IsLocked]			BIT				NOT NULL,
	[MerchantId]        NVARCHAR (50)   NULL,

    [CreatorId]         NVARCHAR (50)   NOT NULL,
    [CreationTime]      DATETIME        NOT NULL,
    [EditorId]          NVARCHAR (50)   NULL,
    [LastEditTime]      DATETIME        NULL,
    [IsValid]           BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE [dbo].[PortalRoles] (
    [Id]                NVARCHAR (20)   NOT NULL,
    [Name]              NVARCHAR (50)   NOT NULL,
    [IsDefault]         BIT             NOT NULL,
	[IsBackend]			BIT				NOT NULL,
    [IsValid]           BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE [dbo].[PortalLoginRoles] (
    [LoginId]           NVARCHAR (50)   NOT NULL,
    [RoleId]            NVARCHAR (20)   NOT NULL,
    PRIMARY KEY CLUSTERED ([LoginId] ASC, [RoleId] ASC)
)
GO

-------------------- data tables --------------------


GO

-------------------- seed data --------------------

-- Roles
INSERT INTO [PortalRoles] VALUES ('1', 'SuperAdmin', 1, 1, 1)
INSERT INTO [PortalRoles] VALUES ('10', 'Admin', 1, 1, 1)

-- Default Super Admin Account (password: Passw0rd01!)
INSERT INTO [PortalLogins] VALUES ('10000000000000000001', '<default admin>', NULL, '<default password>', 0, 0, NULL, '', GETDATE(), NULL,NULL,1)
-- Asign Super Admin Role
INSERT INTO [PortalLoginRoles] VALUES ('10000000000000000001', '1')
INSERT INTO [PortalLoginRoles] VALUES ('10000000000000000001', '10')

GO