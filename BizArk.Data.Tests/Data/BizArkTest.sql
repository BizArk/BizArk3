USE [master]
GO

CREATE DATABASE [BizArkTest]
	CONTAINMENT = NONE
	ON PRIMARY 
	( 
		NAME = N'BizArkTest', 
		FILENAME = N'BizArkTest.mdf' , 
		SIZE = 8192KB , 
		MAXSIZE = UNLIMITED, 
		FILEGROWTH = 65536KB 
	)
	LOG ON 
	( 
		NAME = N'BizArkTest_log', 
		FILENAME = N'BizArkTest_log.ldf' , 
		SIZE = 8192KB , 
		MAXSIZE = 2048GB , 
		FILEGROWTH = 65536KB 
	)
GO

ALTER DATABASE [BizArkTest] SET COMPATIBILITY_LEVEL = 130
GO

USE [BizArkTest]
GO

CREATE TABLE [dbo].[Person]
(
	[Id] [int] NOT NULL IDENTITY,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[BirthDate] [date] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) 
	WITH 
	(
		PAD_INDEX = OFF, 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	) 
	ON [PRIMARY]
) 
ON [PRIMARY]
GO

-- These people need to be in the table. Do not change the names or dates.
INSERT INTO Person (FirstName, LastName, BirthDate) VALUES ('John', 'Smith', '1/2/1999')
INSERT INTO Person (FirstName, LastName, BirthDate) VALUES ('Jane', 'Doe', '5/8/1986')
INSERT INTO Person (FirstName, LastName, BirthDate) VALUES ('Nobody', NULL, NULL)
GO