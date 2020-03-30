USE [master]
GO

/****** Object:  Database [eMemberRegApp]    Script Date: 2/27/2017 6:27:32 PM ******/
CREATE DATABASE [eMemberRegApp]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'eMemberRegApp', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\eMemberRegApp.mdf' , SIZE = 70656KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'eMemberRegApp_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\eMemberRegApp_log.ldf' , SIZE = 120448KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [eMemberRegApp] SET COMPATIBILITY_LEVEL = 110
GO


USE [eMemberRegApp]
GO
/****** Object:  User [eadmin]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE USER [smadmin] FOR LOGIN [smadmin] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_accessadmin] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_securityadmin] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_backupoperator] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_datareader] ADD MEMBER [smadmin]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [smadmin]
GO

/****** Object:  Table [dbo].[Checkin]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EMemberRegistration](
	[Id] [nvarchar](128) NOT NULL,
	[First_Name] [nvarchar](100) NULL,
	[Last_Name] [nvarchar](100) NULL,
	[Middle_Name] [nvarchar](50) NULL,
	[Dob] [date] NULL,
	[Address] [nvarchar](100) NULL,
	[Address_2] [nvarchar](100) NULL,
	[City] [nvarchar](100) NULL,
	[State] [nvarchar](10) NULL,
	[Postal_Code] [nvarchar](20) NULL,
	[Deduction_Opt_Out_Flag] [bit] NOT NULL,
	[Home_Phone] [nvarchar](100) NULL,
	[Mobile_Phone] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Employer_Union_Id] [nvarchar](20) NULL,
	[Position] [nvarchar](100) NULL,
	[Date_Of_Hire] [date] NULL,
	[Ssn] [nvarchar](12) NULL,
	[Sms_Opt_In_Flag] [bit] NOT NULL,
	[Race] [nvarchar](20) NULL,
	[Gender] [nvarchar](20) NULL,
	[Other_Gender] [nvarchar](100) NULL,
	[Ethnicity] [nvarchar](20) NULL,
	[Country] [nvarchar](20) NULL,
	[First_Language] [nvarchar](100) NULL,
	[Second_Language] [nvarchar](100) NULL,
	[Tip_Opt_In_Flag] [bit] NOT NULL,
	[Tip_Frequency] [nvarchar](20) NULL,
	[Tip_Contribution] [numeric](10, 2) NULL,
	[LocalNumber] [nvarchar](20) NULL,
	[Validated_Flag] [bit] NOT NULL,
	[Validation_Status] [nvarchar](256) NULL,
	[Validation_Date] [nvarchar](256) NULL,
	[Processed_Flag] [bit] NOT NULL,
	[Processed_Status] [nvarchar](256) NULL,
	[Processed_Date] [date] NULL,
	[Success_Flag] [bit] NOT NULL,
	[Member_Union_Id] [nvarchar](20) NULL,
	[CreatedBy] [nvarchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](20) NULL,
	[ModifiedOn] [datetime] NULL,
	[RowVersion] [int] NOT NULL,
	[Dues_Card_File_Name] [nvarchar](100) NULL,
	[Dues_Card_Image] [varbinary](max) NULL,
	[Mbr_Level1] [nvarchar](20) NULL,
	[Mbr_Level2] [nvarchar](20) NULL,
	[Mbr_Level3] [nvarchar](20) NULL,
	[Mbr_Type] [nvarchar](20) NULL,
	[Employer_Name] [nvarchar](100) NULL,
	[UpdateName] [bit] NOT NULL,
	[UpdateAddress] [bit] NOT NULL,
	[UpdateHouse] [bit] NOT NULL,
	[UpdatePhone] [bit] NOT NULL,
	[UpdateEmail] [bit] NOT NULL,
	[Department] [nvarchar](20) NULL,
	[Section] [nvarchar](20) NULL,
	[Craft] [nvarchar](20) NULL,
	[FullPartTime] [nvarchar](20) NULL,
	[Notes] [nvarchar](2000) NULL,
 CONSTRAINT [PK_dbo.EMemberRegistration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD  DEFAULT ((0)) FOR [UpdateName]
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD  DEFAULT ((0)) FOR [UpdateAddress]
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD  DEFAULT ((0)) FOR [UpdateHouse]
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD  DEFAULT ((0)) FOR [UpdatePhone]
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD  DEFAULT ((0)) FOR [UpdateEmail]
GO

ALTER TABLE [dbo].[EMemberRegistration] ADD [TipActionCode] [nvarchar](10)
ALTER TABLE [dbo].[EMemberRegistration] ALTER COLUMN [Validation_Status] [nvarchar](1000) NULL

ALTER TABLE [dbo].[EMemberRegistration] ADD [Dues_Card_Signed_Date] [datetime] NULL 
ALTER TABLE [dbo].[EMemberRegistration] ADD [TIP_Card_Signed_Date] [datetime] NULL 
ALTER TABLE [dbo].[EMemberRegistration] ADD [Student_Flag] [bit] NOT NULL DEFAULT 0
ALTER TABLE [dbo].[EMemberRegistration] ADD [Beneficiary] [nvarchar](100)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Work_Phone] [nvarchar](100)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Dues_Card_BoxId] [nvarchar](256)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Country_Of_Origin] [nvarchar](5)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Discard_Reason] [nvarchar](100)

/*
ALTER TABLE [dbo].[EMemberRegistration] ADD [Department] [nvarchar](20)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Section] [nvarchar](20)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Craft] [nvarchar](20)
ALTER TABLE [dbo].[EMemberRegistration] ADD [FullPartTime] [nvarchar](20)
ALTER TABLE [dbo].[EMemberRegistration] ADD [Notes] [nvarchar](2000)
*/

CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[LocalNumber] [nvarchar](4) NULL,
	[Role] [nvarchar](20) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Table [dbo].[Error]    Script Date: 2/27/2017 6:29:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Error](
	[Id] [nvarchar](128) NOT NULL,
	[ErrorCode] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Error] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_RoleId]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE NONCLUSTERED INDEX [IX_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_UserId]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE NONCLUSTERED INDEX [IX_UserId] ON [dbo].[AspNetUserRoles]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 2/27/2017 6:29:15 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO

ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
--
/*
--  Report Setup Data
INSERT INTO [dbo].[ReportDef] ([Id],[Name],[Description],[CreatedBy],[CreatedOn],[RowVersion])
  select NEWID(), 'VotedReportByLastName','Sorted list of members that have voted.','eadmin',getdate(),0
-- Report Parameters
INSERT INTO [dbo].[ReportParameter] ([Id],[ParameterName],[DataType],[IsRequired],[DefaultValue],[Description], [ReportDefId])
Select NEWID(), 'ElectionId', 'string', 1, null, 'Election ID', Id  from [ReportDef] Where Name = 'VotedReportByLastName'
INSERT INTO [dbo].[ReportParameter] ([Id],[ParameterName],[DataType],[IsRequired],[DefaultValue],[Description], [ReportDefId])
Select NEWID(), 'FromDate', 'datetime', 0, '01/01/2000', 'From Time along with the date', Id  from [ReportDef] Where Name = 'VotedReportByLastName'
INSERT INTO [dbo].[ReportParameter] ([Id],[ParameterName],[DataType],[IsRequired],[DefaultValue],[Description], [ReportDefId])
Select NEWID(), 'ToDate', 'datetime', 0,  '12/31/3000', 'To Time along with the date', Id  from [ReportDef] Where Name = 'VotedReportByLastName'
--
*/


USE [master]
GO
ALTER DATABASE [eMemberRegApp] SET  READ_WRITE 
GO
