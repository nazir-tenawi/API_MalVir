USE [master]
GO
/****** Object:  Database [db_marvir]    Script Date: 4/20/2022 10:13:18 PM ******/
CREATE DATABASE [db_marvir]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'db_marvir', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\db_marvir.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'db_marvir_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\db_marvir_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [db_marvir] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_marvir].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [db_marvir] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [db_marvir] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [db_marvir] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [db_marvir] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [db_marvir] SET ARITHABORT OFF 
GO
ALTER DATABASE [db_marvir] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [db_marvir] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [db_marvir] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [db_marvir] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [db_marvir] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [db_marvir] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [db_marvir] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [db_marvir] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [db_marvir] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [db_marvir] SET  DISABLE_BROKER 
GO
ALTER DATABASE [db_marvir] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [db_marvir] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [db_marvir] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [db_marvir] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [db_marvir] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [db_marvir] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [db_marvir] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [db_marvir] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [db_marvir] SET  MULTI_USER 
GO
ALTER DATABASE [db_marvir] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [db_marvir] SET DB_CHAINING OFF 
GO
ALTER DATABASE [db_marvir] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [db_marvir] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [db_marvir] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [db_marvir] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [db_marvir] SET QUERY_STORE = OFF
GO
USE [db_marvir]
GO
/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Split] 
( 
    @string NVARCHAR(MAX), 
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(items NVARCHAR(MAX) 
) 
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (items)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)
        
    END 
    RETURN 
END

GO
/****** Object:  Table [dbo].[Hash]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Hash](
	[HashID] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](max) NULL,
	[HashContent] [nvarchar](max) NULL,
	[Size] [decimal](18, 2) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Hash] PRIMARY KEY CLUSTERED 
(
	[HashID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[History]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[History](
	[HistoryID] [bigint] IDENTITY(1,1) NOT NULL,
	[ScanTypeID] [bigint] NULL,
	[HashID] [bigint] NULL,
	[CreatedUser] [bigint] NULL,
	[UpdatedUser] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[HashContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_History] PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserManagement]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserManagement](
	[UserID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NULL,
	[DisplayName] [nvarchar](max) NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](max) NULL,
	[TimeZoneID] [int] NULL,
	[Is_SendMail_Password] [bit] NULL,
	[ProfilePicture] [nvarchar](max) NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_UserManagement] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScanType]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScanType](
	[ScanTypeID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_ScanType] PRIMARY KEY CLUSTERED 
(
	[ScanTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_Historys]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_Historys]
AS
SELECT DISTINCT 
                         dbo.History.HistoryID, dbo.History.HashContent, dbo.History.ScanTypeID, dbo.History.HashID, dbo.History.CreatedUser, dbo.History.UpdatedUser, 
                         dbo.History.CreatedDate, dbo.History.UpdatedDate, ST.Name AS ScanTypeName, Loc.Type AS Type, 
                         UsrCre.DisplayName AS CreatedUserName
FROM         dbo.History LEFT OUTER JOIN
                         dbo.ScanType AS ST ON ST.ScanTypeID = dbo.History.ScanTypeID LEFT OUTER JOIN
                         dbo.Hash AS Loc ON Loc.HashID = dbo.History.HashID LEFT OUTER JOIN
                         dbo.UserManagement AS UsrCre ON UsrCre.UserID = dbo.History.CreatedUser
GO
/****** Object:  Table [dbo].[AgentSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgentSetting](
	[AgentSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[Is_Profile_Visible] [bit] NULL,
	[Is_CommonSetting_Visible] [bit] NULL,
	[PageSize] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_AgentSetting] PRIMARY KEY CLUSTERED 
(
	[AgentSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ApplicationSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationSetting](
	[ApplicationSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[DefaultPassword] [nvarchar](max) NULL,
	[Is_Chat_Visible] [bit] NULL,
	[Is_LockUser] [bit] NULL,
	[CompanyTitle] [nvarchar](max) NULL,
	[CompanyLogo] [nvarchar](max) NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[Is_History_StartPage] [bit] NULL,
 CONSTRAINT [PK_ApplicationSetting] PRIMARY KEY CLUSTERED 
(
	[ApplicationSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientSetting](
	[ClientSettingID] [bigint] IDENTITY(1,1) NOT NULL,
	[Is_Profile_Visible] [bit] NULL,
	[PageSize] [int] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_ClientSetting] PRIMARY KEY CLUSTERED 
(
	[ClientSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Menu]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Menu](
	[MenuID] [bigint] IDENTITY(1,1) NOT NULL,
	[MenuName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED 
(
	[MenuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermission]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermission](
	[RolePermissionID] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleID] [bigint] NULL,
	[MenuID] [bigint] NULL,
	[Is_Full] [bit] NULL,
	[Is_View] [bit] NULL,
	[Is_Add] [bit] NULL,
	[Is_Edit] [bit] NULL,
	[Is_Delete] [bit] NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_RolePermission] PRIMARY KEY CLUSTERED 
(
	[RolePermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Is_Agent] [bit] NULL,
	[Is_Client] [bit] NULL,
	[Is_Active] [bit] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AgentSetting] ON 

INSERT [dbo].[AgentSetting] ([AgentSettingID], [Is_Profile_Visible], [Is_CommonSetting_Visible], [PageSize], [CreatedDate]) VALUES (1, 1, 1, 20, CAST(N'2018-10-17T14:10:07.373' AS DateTime))
SET IDENTITY_INSERT [dbo].[AgentSetting] OFF
GO
SET IDENTITY_INSERT [dbo].[ApplicationSetting] ON 

INSERT [dbo].[ApplicationSetting] ([ApplicationSettingID], [DefaultPassword], [Is_Chat_Visible], [Is_LockUser], [CompanyTitle], [CompanyLogo], [Is_Active], [CreatedDate], [Is_History_StartPage]) VALUES (1, N'abc@123', 0, 0, N'MalVirDetector', N'logo.png', 1, CAST(N'2018-10-17T17:42:53.743' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[ApplicationSetting] OFF
GO
SET IDENTITY_INSERT [dbo].[ClientSetting] ON 

INSERT [dbo].[ClientSetting] ([ClientSettingID], [Is_Profile_Visible], [PageSize], [CreatedDate]) VALUES (1, 1, 20, CAST(N'2018-10-17T14:11:03.360' AS DateTime))
SET IDENTITY_INSERT [dbo].[ClientSetting] OFF
GO
SET IDENTITY_INSERT [dbo].[Hash] ON 

INSERT [dbo].[Hash] ([HashID], [Type], [HashContent], [Size], [CreatedDate]) VALUES (1, N'JPEG', N'f04849f932f573711d6a02684aa7e461', CAST(12.15 AS Decimal(18, 2)), CAST(N'2022-04-16T01:44:37.393' AS DateTime))
INSERT [dbo].[Hash] ([HashID], [Type], [HashContent], [Size], [CreatedDate]) VALUES (2, N'EXE', N'123', CAST(10.11 AS Decimal(18, 2)), CAST(N'2022-04-16T01:45:25.477' AS DateTime))
SET IDENTITY_INSERT [dbo].[Hash] OFF
GO
SET IDENTITY_INSERT [dbo].[History] ON 

INSERT [dbo].[History] ([HistoryID], [ScanTypeID], [HashID], [CreatedUser], [UpdatedUser], [UpdatedDate], [CreatedDate], [HashContent]) VALUES (16, 1, 1, 1, 1, CAST(N'2022-04-20T22:07:20.760' AS DateTime), CAST(N'2022-04-20T22:07:20.760' AS DateTime), N'f04849f932f573711d6a02684aa7e461')
INSERT [dbo].[History] ([HistoryID], [ScanTypeID], [HashID], [CreatedUser], [UpdatedUser], [UpdatedDate], [CreatedDate], [HashContent]) VALUES (17, 1, 1, 1, 1, CAST(N'2022-04-20T22:07:43.727' AS DateTime), CAST(N'2022-04-20T22:07:43.727' AS DateTime), N'f04849f932f573711d6a02684aa7e461')
SET IDENTITY_INSERT [dbo].[History] OFF
GO
SET IDENTITY_INSERT [dbo].[Menu] ON 

INSERT [dbo].[Menu] ([MenuID], [MenuName], [Description], [Is_Active], [CreatedDate]) VALUES (1, N'History', N'History', 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[Menu] ([MenuID], [MenuName], [Description], [Is_Active], [CreatedDate]) VALUES (6, N'Admin', N'Admin', 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
SET IDENTITY_INSERT [dbo].[Menu] OFF
GO
SET IDENTITY_INSERT [dbo].[RolePermission] ON 

INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (1, 1, 1, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (2, 1, 4, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (3, 1, 5, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (4, 1, 6, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (5, 1, 2, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (6, 1, 3, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (7, 1, 7, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (8, 1, 8, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (9, 1, 9, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (10, 1, 10, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (11, 2, 1, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (12, 2, 4, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (13, 2, 5, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (14, 2, 6, 1, 1, 1, 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (15, 2, 2, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (16, 2, 3, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (17, 2, 7, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (18, 2, 8, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (19, 2, 9, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (20, 2, 10, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (21, 3, 1, 0, 1, 1, 1, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (22, 3, 4, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (23, 3, 5, 0, 1, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (24, 3, 6, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (25, 3, 2, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (26, 3, 3, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (27, 3, 7, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (28, 3, 8, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (29, 3, 9, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (30, 3, 10, 1, 1, 1, 1, 1, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (31, NULL, 1, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (32, NULL, 4, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (33, NULL, 5, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (34, NULL, 6, 0, 0, 0, 0, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (35, NULL, 2, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (36, NULL, 3, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (37, NULL, 7, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (38, NULL, 8, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (39, NULL, 9, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[RolePermission] ([RolePermissionID], [RoleID], [MenuID], [Is_Full], [Is_View], [Is_Add], [Is_Edit], [Is_Delete], [Is_Active], [CreatedDate]) VALUES (40, NULL, 10, 0, 0, 0, 0, 0, 0, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
SET IDENTITY_INSERT [dbo].[RolePermission] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RoleID], [Name], [Description], [Is_Agent], [Is_Client], [Is_Active], [CreatedDate]) VALUES (1, N'Administrator', N'Administrator', 1, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[Roles] ([RoleID], [Name], [Description], [Is_Agent], [Is_Client], [Is_Active], [CreatedDate]) VALUES (2, N'Agent', N'Agent', 1, 0, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
INSERT [dbo].[Roles] ([RoleID], [Name], [Description], [Is_Agent], [Is_Client], [Is_Active], [CreatedDate]) VALUES (3, N'Client', N'Client can create only historys and view solutions.', 0, 1, 1, CAST(N'2018-10-10T14:00:38.877' AS DateTime))
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[ScanType] ON 

INSERT [dbo].[ScanType] ([ScanTypeID], [Name], [Description], [Is_Active], [CreatedDate]) VALUES (1, N'Search', N'Search', 1, CAST(N'2019-02-28T11:59:01.563' AS DateTime))
INSERT [dbo].[ScanType] ([ScanTypeID], [Name], [Description], [Is_Active], [CreatedDate]) VALUES (2, N'File', N'File', 1, CAST(N'2019-02-28T11:59:07.963' AS DateTime))
INSERT [dbo].[ScanType] ([ScanTypeID], [Name], [Description], [Is_Active], [CreatedDate]) VALUES (3, N'Wireshark', N'Wireshark', 1, CAST(N'2019-02-28T11:59:13.263' AS DateTime))
SET IDENTITY_INSERT [dbo].[ScanType] OFF
GO
SET IDENTITY_INSERT [dbo].[UserManagement] ON 

INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (1, 1, N'Admin', N'admin', N'vSueZeIxdzz/Xy4nt1SK7w==', N'admin@gmail.com', N'0123456789', NULL, 1, NULL, 1, CAST(N'2021-05-13T16:04:44.583' AS DateTime))
INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (2, 2, N'1', N'2', N'kOe26StCv1B/EvaNtSabpg==', N'admin@demo.com', N'3', NULL, 1, NULL, 1, CAST(N'2022-04-17T02:24:13.750' AS DateTime))
INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (3, 3, N'a', N's', N'kOe26StCv1B/EvaNtSabpg==', N'admin@demo.com', N'c', NULL, 0, NULL, 1, CAST(N'2022-04-17T02:25:07.420' AS DateTime))
INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (4, 2, N'12', N'23', N'2KkXkfKzfxVRoaabOFL3Xw==', N'admin@demo.com', N'231', NULL, 0, NULL, 1, CAST(N'2022-04-17T02:28:00.167' AS DateTime))
INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (6, 3, NULL, N'11', N'kOe26StCv1B/EvaNtSabpg==', N'33@33.com', N'44', NULL, 0, NULL, 0, CAST(N'2022-04-17T02:46:38.873' AS DateTime))
INSERT [dbo].[UserManagement] ([UserID], [RoleID], [DisplayName], [UserName], [Password], [Email], [PhoneNo], [TimeZoneID], [Is_SendMail_Password], [ProfilePicture], [Is_Active], [CreatedDate]) VALUES (7, 3, N'222', N'111', N'kOe26StCv1B/EvaNtSabpg==', N'333', N'444', NULL, 0, NULL, 0, CAST(N'2022-04-17T02:48:47.090' AS DateTime))
SET IDENTITY_INSERT [dbo].[UserManagement] OFF
GO
ALTER TABLE [dbo].[AgentSetting] ADD  CONSTRAINT [DF_Table_1_Is_Client_Visible]  DEFAULT ((1)) FOR [Is_Profile_Visible]
GO
ALTER TABLE [dbo].[AgentSetting] ADD  CONSTRAINT [DF_Table_1_Is_Profile_Visible1]  DEFAULT ((1)) FOR [Is_CommonSetting_Visible]
GO
ALTER TABLE [dbo].[AgentSetting] ADD  CONSTRAINT [DF_AgentSetting_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Is_Chat_Visible]  DEFAULT ((0)) FOR [Is_Chat_Visible]
GO
ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Is_LockUser]  DEFAULT ((0)) FOR [Is_LockUser]
GO
ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ApplicationSetting] ADD  CONSTRAINT [DF_ApplicationSetting_Is_History_StartPage]  DEFAULT ((0)) FOR [Is_History_StartPage]
GO
ALTER TABLE [dbo].[ClientSetting] ADD  CONSTRAINT [DF_ClientSetting_Is_Profile_Visible]  DEFAULT ((1)) FOR [Is_Profile_Visible]
GO
ALTER TABLE [dbo].[ClientSetting] ADD  CONSTRAINT [DF_ClientSetting_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Hash] ADD  CONSTRAINT [DF_Hash_Is_Active]  DEFAULT ((0)) FOR [Size]
GO
ALTER TABLE [dbo].[Hash] ADD  CONSTRAINT [DF_Hash_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[History] ADD  CONSTRAINT [DF_History_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[Menu] ADD  CONSTRAINT [DF_Menu_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[RolePermission] ADD  CONSTRAINT [DF_RolePermission_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_Is_Agent]  DEFAULT ((0)) FOR [Is_Agent]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_Is_Client]  DEFAULT ((0)) FOR [Is_Client]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ScanType] ADD  CONSTRAINT [DF_ScanType_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[ScanType] ADD  CONSTRAINT [DF_ScanType_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[UserManagement] ADD  CONSTRAINT [DF_UserManagement_Is_Active]  DEFAULT ((0)) FOR [Is_Active]
GO
ALTER TABLE [dbo].[UserManagement] ADD  CONSTRAINT [DF_UserManagement_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_Hash] FOREIGN KEY([HashID])
REFERENCES [dbo].[Hash] ([HashID])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_Hash]
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_ScanType] FOREIGN KEY([ScanTypeID])
REFERENCES [dbo].[ScanType] ([ScanTypeID])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_ScanType]
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_UserManagement2] FOREIGN KEY([CreatedUser])
REFERENCES [dbo].[UserManagement] ([UserID])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_UserManagement2]
GO
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_UserManagement3] FOREIGN KEY([UpdatedUser])
REFERENCES [dbo].[UserManagement] ([UserID])
GO
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_UserManagement3]
GO
ALTER TABLE [dbo].[RolePermission]  WITH CHECK ADD  CONSTRAINT [FK_RolePermission_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[RolePermission] CHECK CONSTRAINT [FK_RolePermission_Roles]
GO
ALTER TABLE [dbo].[UserManagement]  WITH CHECK ADD  CONSTRAINT [FK_UserManagement_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[UserManagement] CHECK CONSTRAINT [FK_UserManagement_Roles]
GO
/****** Object:  StoredProcedure [dbo].[ActiveDeActive_User]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ActiveDeActive_User 
-- =============================================
CREATE PROCEDURE [dbo].[ActiveDeActive_User]
	@UserID BIGINT,
	@Is_Active bit
AS
BEGIN
		SET ARITHABORT ON;
	
		UPDATE UserManagement
		SET Is_Active = @Is_Active
		WHERE UserID = @UserID
	
END

GO
/****** Object:  StoredProcedure [dbo].[Check_UserName_Available]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- AgentSetting_Update 
-- =============================================

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ApplicationSetting_Update 
-- =============================================

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Check_UserName_Available 0,0,'bhavesh'
-- Check_UserName_Available 0,1,'bhavesh'
-- Check_UserName_Available 0,2,'bhavesh'
-- =============================================
CREATE PROCEDURE [dbo].[Check_UserName_Available]
	@ReturnValue INT OUTPUT,
	@UserID BIGINT,
	@UserName NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;

	IF EXISTS(SELECT TOP 1 UserID FROM UserManagement WHERE UserName = @UserName AND UserID != @UserID)
		BEGIN
			SELECT @ReturnValue = 0
			--PRINT '0'
		END
	ELSE
		BEGIN
			SELECT @ReturnValue = 1
			--PRINT '1'
		END
END

GO
/****** Object:  StoredProcedure [dbo].[Get_AgentSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_AgentSetting
-- =============================================
CREATE PROCEDURE [dbo].[Get_AgentSetting]
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT TOP 1 * FROM AgentSetting 
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_ApplicationSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_ApplicationSetting
-- =============================================
CREATE PROCEDURE [dbo].[Get_ApplicationSetting]
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT TOP 1 * FROM ApplicationSetting 
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_ClientSetting]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_ClientSetting
-- =============================================
CREATE PROCEDURE [dbo].[Get_ClientSetting]
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT TOP 1 * FROM ClientSetting 
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_Email_ByUserName]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Email_ByUserName 'bhavesh'
-- =============================================
CREATE PROCEDURE [dbo].[Get_Email_ByUserName]
	@UserName NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT Email FROM UserManagement WHERE UserName = @UserName	
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_Hash_ByID]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Hash_ByID 1
-- =============================================
CREATE PROCEDURE [dbo].[Get_Hash_ByID]
	@HashID BIGINT = 0
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT * FROM Hash 
	WHERE HashID = @HashID

END

GO
/****** Object:  StoredProcedure [dbo].[Get_Hash_List]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Hash_List 
-- =============================================
CREATE PROCEDURE [dbo].[Get_Hash_List]
	
AS
BEGIN
	SET ARITHABORT ON;

	SELECT * FROM Hash 	

END

GO
/****** Object:  StoredProcedure [dbo].[Get_History_ByID]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_History_ByID 0,'History-1'
-- =============================================
CREATE PROCEDURE [dbo].[Get_History_ByID]
	@HistoryID BIGINT,
	@DisplayHistoryID NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;

	IF(@HistoryID > 0)
	BEGIN
		SELECT * FROM vw_Historys WHERE HistoryID = @HistoryID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[Get_History_Detail_Data]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- exec Get_History_Detail_Data  1
-- =============================================  
CREATE PROCEDURE [dbo].[Get_History_Detail_Data]  
    @Is_Agent BIT  
AS  
BEGIN  
	SET ARITHABORT ON;
  
	 SELECT HashID AS [Value] FROM [Hash]
	 SELECT ScanTypeID AS [Value] FROM [ScanType] 
  
END  
GO
/****** Object:  StoredProcedure [dbo].[Get_History_List]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_History_List 0,1,1
-- =============================================
CREATE PROCEDURE [dbo].[Get_History_List]
	@Is_Agent BIT,
	@Is_Client BIT,
	@UserID BIGINT
AS
BEGIN
	SET ARITHABORT ON;

	IF(@Is_Agent = 1 AND @Is_Client = 1) -- admin
	BEGIN
		SELECT * FROM vw_Historys
		ORDER BY CreatedDate DESC
	END
	ELSE IF(@Is_Agent = 1 AND @Is_Client = 0) --agent
	BEGIN
		SELECT * FROM vw_Historys
		ORDER BY CreatedDate DESC
	END
	BEGIN --requester
		SELECT * FROM vw_Historys WHERE CreatedUser = @UserID
		ORDER BY CreatedDate DESC
	END



END


GO
/****** Object:  StoredProcedure [dbo].[Get_Role_List_KeyValue]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Role_List_KeyValue
-- =============================================
CREATE PROCEDURE [dbo].[Get_Role_List_KeyValue]	
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT RoleID AS [Value],[Name] AS [Key] FROM Roles
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_Roles_ByID]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Roles_ByID 5
-- =============================================
CREATE PROCEDURE [dbo].[Get_Roles_ByID]
	@RoleID BIGINT = NULL
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT * FROM Roles WHERE RoleID = @RoleID 
		
	DECLARE @Cnt INT = 0;
	SELECT @Cnt = COUNT(1) FROM RolePermission WHERE RoleID = @RoleID
	IF(@Cnt > 0)
	BEGIN
			SELECT RP.*,Menu.MenuName,Menu.[Description] FROM Menu
			LEFT JOIN RolePermission RP ON RP.MenuID  = menu.MenuID
			WHERE Menu.Is_Active = 1 AND RoleID = @RoleID
	END
	ELSE
	BEGIN
			SELECT 
			0 AS RolePermissionID,
			RP.RoleID,
			RP.MenuID,
			RP.Is_Full,
			RP.Is_View,
			RP.Is_Add,
			RP.Is_Edit,
			RP.Is_Delete,
			RP.Is_Active,
			Menu.MenuName,
			Menu.[Description] 
			FROM Menu
			LEFT JOIN RolePermission RP ON RP.MenuID  = menu.MenuID
			WHERE Menu.Is_Active = 1 AND RoleID IS NULL			
	END


END

GO
/****** Object:  StoredProcedure [dbo].[Get_Roles_List]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Roles_List
CREATE PROCEDURE [dbo].[Get_Roles_List]
	
AS
BEGIN
	SET ARITHABORT ON;

	SELECT * FROM Roles 

END

GO
/****** Object:  StoredProcedure [dbo].[Get_ScanType_ByID]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_ScanType_ByID 1
-- =============================================
CREATE PROCEDURE [dbo].[Get_ScanType_ByID]
	@ScanTypeID BIGINT = 0
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT * FROM ScanType 
	WHERE ScanTypeID = @ScanTypeID 

END

GO
/****** Object:  StoredProcedure [dbo].[Get_ScanType_List]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_ScanType_List
CREATE PROCEDURE [dbo].[Get_ScanType_List]
	
AS
BEGIN
	SET ARITHABORT ON;

	SELECT * FROM ScanType 

END

GO
/****** Object:  StoredProcedure [dbo].[Get_UserManagement_ByID]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_UserManagement_ByID 1
-- =============================================
CREATE PROCEDURE [dbo].[Get_UserManagement_ByID]
	@UserID BIGINT = 0
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT Usr.*, Roles.[Name] AS RoleName
	FROM UserManagement AS Usr
	JOIN Roles ON  Roles.RoleID = Usr.RoleID
	WHERE UserID = @UserID
		
END

GO
/****** Object:  StoredProcedure [dbo].[Get_UserManagement_List]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_UserManagement_List 0,1
-- =============================================
CREATE PROCEDURE [dbo].[Get_UserManagement_List]
	@Is_Agent BIT,
	@Is_Client BIT = 0,
	@Is_Active BIT = 0
AS
BEGIN
	SET ARITHABORT ON;

	IF(@Is_Agent = 1 AND @Is_Client = 1)
	BEGIN
		SELECT Usr.*, Roles.[Name] AS RoleName
		FROM UserManagement AS Usr
		JOIN Roles ON  Roles.RoleID = Usr.RoleID
		WHERE Roles.Is_Agent = 1 AND Roles.Is_Client = 1 AND (@Is_Active = 0 OR Usr.Is_Active = @Is_Active)
	END
	ELSE IF(@Is_Agent = 1)
	BEGIN
		SELECT Usr.*, Roles.[Name] AS RoleName
		FROM UserManagement AS Usr
		JOIN Roles ON  Roles.RoleID = Usr.RoleID
		WHERE Roles.Is_Agent = 1 AND Roles.Is_Client = 0 AND (@Is_Active = 0 OR Usr.Is_Active = @Is_Active)
	END
	ELSE
	BEGIN
		SELECT Usr.*, Roles.[Name] AS RoleName
		FROM UserManagement AS Usr
		JOIN Roles ON  Roles.RoleID = Usr.RoleID
		WHERE  Roles.Is_Client = 1 AND Roles.Is_Agent = 0 AND (@Is_Active = 0 OR Usr.Is_Active = @Is_Active)
	END

END

GO
/****** Object:  StoredProcedure [dbo].[Hash_Delete]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Hash_Delete 
-- =============================================
CREATE PROCEDURE [dbo].[Hash_Delete]
	@HashID NVARCHAR(MAX)	
AS
BEGIN
	SET ARITHABORT ON;
	
	DELETE FROM Hash WHERE HashID IN (SELECT items FROM dbo.Split(@HashID,','))

END

GO
/****** Object:  StoredProcedure [dbo].[Hash_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- HashContent:	<HashContent,,>
-- Hash_Update 
-- =============================================
CREATE PROCEDURE [dbo].[Hash_Update]
	@HashID BIGINT OUTPUT,
	@Type NVARCHAR(MAX),
	@HashContent NVARCHAR(MAX),
	@Size decimal(18, 2)

AS
BEGIN
	SET ARITHABORT ON;
	
	IF(@HashID > 0)
	BEGIN 
		
			UPDATE [Hash]
			SET [Type] = @Type,
				[HashContent] = @HashContent,
				Size = @Size
			WHERE HashID = @HashID

	END
	ELSE
	BEGIN
			
			INSERT INTO [Hash]
			(
			[Type],
			[HashContent],
			Size,
			CreatedDate
			)
			VALUES (
			@Type,
			@HashContent,
			@Size,
			GETDATE()
			)

			SELECT @HashID = SCOPE_IDENTITY()
	END

END

GO
/****** Object:  StoredProcedure [dbo].[History_Create]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- History_Create
-- =============================================
CREATE PROCEDURE [dbo].[History_Create]
	@HistoryID BIGINT OUTPUT,
	@ScanTypeID BIGINT ,
	@HashID BIGINT ,
	@CreatedUser BIGINT ,	
	@UpdatedUser BIGINT,
	@UpdatedDate DATETIME,
	@CreatedDate DATETIME,
	@HashContent nvarchar(MAX)
AS
BEGIN	
		SET ARITHABORT ON;
			
		INSERT INTO [dbo].[History]
           (
           [ScanTypeID]
           ,[HashID]
           ,[CreatedUser]           
           ,[CreatedDate]           
		   ,[UpdatedUser]
		   ,[UpdatedDate]
		   ,HashContent
		   )
     VALUES
           (
			CASE WHEN @ScanTypeID = 0 THEN NULL ELSE @ScanTypeID END 
			,CASE WHEN @HashID = 0 THEN NULL ELSE @HashID END 
			,@CreatedUser
			,@CreatedDate
			,@UpdatedUser
			,@UpdatedDate
			,@HashContent
		   )

		SELECT @HistoryID = SCOPE_IDENTITY()

		SELECT @HistoryID = HistoryID FROM History WHERE HistoryID = @HistoryID

END


GO
/****** Object:  StoredProcedure [dbo].[Login_User]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Login_User 'bhavesh','asd'
-- Login_User 'bhavesh','0mjENZvxtPSxH4TGWktt6Q=='
-- =============================================
CREATE PROCEDURE [dbo].[Login_User]
	@UserName NVARCHAR(MAX),
	@Password NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT * FROM UserManagement WHERE UserName = @UserName AND [Password] = @Password AND Is_Active = 1
	--DECLARE @UserID BIGINT = 0;
	--SELECT @UserID = UserID FROM UserManagement WHERE UserName = @UserName AND [Password] = @Password AND Is_Active = 1	
	--IF(@UserID > 0)
	--BEGIN
	--		EXEC Page_Permission @UserID = @UserID
	--END

END

GO
/****** Object:  StoredProcedure [dbo].[Logo_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Logo_Update
-- =============================================
CREATE PROCEDURE [dbo].[Logo_Update]
	@ApplicationSettingID BIGINT OUTPUT,
	@CompanyLogo NVARCHAR(MAX),
	@CompanyTitle NVARCHAR(MAX)
AS
BEGIN		
	SET ARITHABORT ON;
		
			UPDATE ApplicationSetting
			SET CompanyLogo = CASE WHEN ISNULL(@CompanyLogo,'') = '' THEN CompanyLogo ELSE @CompanyLogo END,
				CompanyTitle = @CompanyTitle
			WHERE ApplicationSettingID = @ApplicationSettingID				

END

GO
/****** Object:  StoredProcedure [dbo].[Page_Permission]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Page_Permission 1
-- =============================================
CREATE PROCEDURE [dbo].[Page_Permission]	
	@UserID BIGINT
AS
BEGIN	
	SET ARITHABORT ON;

		--UserManagement
		DECLARE @RoleID BIGINT;
		DECLARE @DisplayName NVARCHAR(MAX) = '';
		DECLARE @UserName NVARCHAR(MAX) = '';
		DECLARE @Email NVARCHAR(MAX) = '';
		DECLARE @ProfilePicture NVARCHAR(MAX) = '';
		--Roles
		DECLARE @Is_Agent BIT;
		DECLARE @Is_Client BIT;
		--ApplicationSetting
		DECLARE @CompanyLogo NVARCHAR(MAX) = '';
		DECLARE @CompanyTitle NVARCHAR(MAX) = '';
		DECLARE @DefaultLanguage NVARCHAR(MAX) = '';
		DECLARE @DefaultPassword NVARCHAR(MAX) = '';
		DECLARE @Is_EasyAddOn_Visible BIT;
		DECLARE @Is_Chat_Visible BIT;
		--DECLARE @Is_Ticket_Search BIT;
		--DECLARE @Is_Solution_Search BIT;
		--DECLARE @Is_Ticket_Search_Client  BIT;
		--DECLARE @Is_Solution_Search_Client BIT;
		DECLARE @Is_Admin_Search BIT;
		--DECLARE @Is_Print BIT;
		--DECLARE @Is_Export BIT;
		--DECLARE @Is_Export_Clilent BIT;
		DECLARE @Is_Pickup BIT;
		DECLARE @Is_AssignTo_Dropdown BIT;
		DECLARE @Is_Close_Ticket BIT;
		DECLARE @Is_Ticket_StartPage BIT;
		DECLARE @Is_EditRow_On_DoubleClick BIT;
		--DECLARE @Is_Column_Filter_Ticket BIT;
		--DECLARE @Is_Column_Filter_Solution BIT;

		--Agent/Client
		DECLARE @Is_Profile_Visible BIT; DECLARE @Is_Profile_Visible_Client BIT;
		DECLARE @Is_CommonSetting_Visible BIT; DECLARE @Is_CommonSetting_Visible_Client BIT;
		DECLARE @Is_Help_Visible BIT; DECLARE @Is_Help_Visible_Client BIT;
		DECLARE @Is_Solution_Visible BIT; DECLARE @Is_Solution_Visible_Client BIT;
		DECLARE @Is_ColumnChooser_Visible BIT; DECLARE @Is_ColumnChooser_Visible_Client BIT;				
		DECLARE @PageSize INT; DECLARE @PageSize_Client INT;
		DECLARE @Is_Ticket_Visible_Client BIT; -- for client side
		DECLARE @Is_Search_Visible_Client BIT; -- for client side

		DECLARE @Is_Print BIT; DECLARE @Is_Print_Client BIT; 
		DECLARE @Is_Export BIT; DECLARE @Is_Export_Client BIT;
		DECLARE @Is_Ticket_Search BIT; DECLARE @Is_Ticket_Search_Client BIT;
		DECLARE @Is_Solution_Search BIT; DECLARE @Is_Solution_Search_Client BIT;
		DECLARE @Is_Column_Filter_Ticket BIT; DECLARE @Is_Column_Filter_Ticket_Client BIT;
		DECLARE @Is_Column_Filter_Solution BIT; DECLARE @Is_Column_Filter_Solution_Client BIT;
		DECLARE @Is_Clone_Ticket BIT; DECLARE @Is_Clone_Ticket_Client BIT;
		DECLARE @Is_Clone_Solution BIT; DECLARE @Is_Clone_Solution_Client BIT;


		--Role Permission
		DECLARE @Is_Full_Ticket BIT;DECLARE @Is_View_Ticket BIT;DECLARE @Is_Add_Ticket BIT;
		DECLARE @Is_Edit_Ticket BIT;DECLARE @Is_Delete_Ticket BIT; -- Ticket Permission
		DECLARE @Is_Full_Ticket_Client BIT;DECLARE @Is_View_Ticket_Client BIT;DECLARE @Is_Add_Ticket_Client BIT;
		DECLARE @Is_Edit_Ticket_Client BIT;DECLARE @Is_Delete_Ticket_Client BIT; -- Ticket Permission Client

		DECLARE @Is_Full_Summary BIT;DECLARE @Is_View_Summary BIT;DECLARE @Is_Add_Summary BIT;
		DECLARE @Is_Edit_Summary BIT;DECLARE @Is_Delete_Summary BIT; -- Summary Permission
		
		DECLARE @Is_Full_Solution BIT;DECLARE @Is_View_Solution BIT;DECLARE @Is_Add_Solution BIT;
		DECLARE @Is_Edit_Solution BIT;DECLARE @Is_Delete_Solution BIT; -- Solution Permission
		DECLARE @Is_Full_Solution_Client BIT;DECLARE @Is_View_Solution_Client BIT;DECLARE @Is_Add_Solution_Client BIT;
		DECLARE @Is_Edit_Solution_Client BIT;DECLARE @Is_Delete_Solution_Client BIT; -- Solution Permission Client
		
		DECLARE @Is_Full_Admin BIT;DECLARE @Is_View_Admin BIT;DECLARE @Is_Add_Admin BIT;
		DECLARE @Is_Edit_Admin BIT;DECLARE @Is_Delete_Admin BIT; -- Admin Permission


		SELECT @RoleID = UM.RoleID, @DisplayName = DisplayName, @UserName = UserName , @Email = Email, @ProfilePicture = ProfilePicture,
			   @Is_Agent = Roles.Is_Agent , @Is_Client = Roles.Is_Client
		FROM UserManagement UM
		INNER JOIN Roles ON Roles.RoleID = UM.RoleID AND UM.UserID = @UserID
		
		--ApplicationSetting
		SELECT @CompanyLogo = CompanyLogo, @CompanyTitle= CompanyTitle, @DefaultPassword = DefaultPassword
			   		   
			  
		FROM ApplicationSetting 

		--Agent/Technician
		IF(@Is_Agent = 1) 
		BEGIN
				SELECT @Is_Profile_Visible = Is_Profile_Visible, @Is_CommonSetting_Visible = Is_CommonSetting_Visible,
					  @PageSize = PageSize		   
					  
				FROM AgentSetting

		END
		--Client/Requester
		--ELSE IF(@Is_Agent = 0 AND @Is_Client = 1) 
		BEGIN
				SELECT @Is_Profile_Visible_Client = Is_Profile_Visible,
				@PageSize_Client = PageSize
				 
				FROM ClientSetting
		END

		IF(@Is_Agent = 1) 
		BEGIN			
			--Ticket Permission - MenuID = 1
			SELECT @Is_Full_Ticket =  Is_Full, @Is_View_Ticket = Is_View, @Is_Add_Ticket = Is_Add,
				   @Is_Edit_Ticket = Is_Edit, @Is_Delete_Ticket = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 1
		
			--Summary Permission - MenuID = 4
			SELECT @Is_Full_Summary =  Is_Full, @Is_View_Summary = Is_View, @Is_Add_Summary = Is_Add,
				   @Is_Edit_Summary = Is_Edit, @Is_Delete_Summary = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 4

			--Solution Permission - MenuID = 5
			SELECT @Is_Full_Solution =  Is_Full, @Is_View_Solution = Is_View, @Is_Add_Solution = Is_Add,
				   @Is_Edit_Solution = Is_Edit, @Is_Delete_Solution = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 5

			--Admin Permission - MenuID = 6
			SELECT @Is_Full_Admin =  Is_Full, @Is_View_Admin = Is_View, @Is_Add_Admin = Is_Add,
				   @Is_Edit_Admin = Is_Edit, @Is_Delete_Admin = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 6
		
		END
		IF(@Is_Agent = 1 OR @Is_Client = 1)
		BEGIN			
			--Ticket Permission - MenuID = 1
			SELECT @Is_Full_Ticket_Client =  Is_Full, @Is_View_Ticket_Client = Is_View, @Is_Add_Ticket_Client = Is_Add,
				   @Is_Edit_Ticket_Client = Is_Edit, @Is_Delete_Ticket_Client = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 1

			--Solution Permission - MenuID = 5
			SELECT @Is_Full_Solution_Client =  Is_Full, @Is_View_Solution_Client = Is_View, @Is_Add_Solution_Client = Is_Add,
				   @Is_Edit_Solution_Client = Is_Edit, @Is_Delete_Solution_Client = Is_Delete
			FROM RolePermission WHERE RoleID = @RoleID AND MenuID = 5

		END

		

			   
		SELECT --UserManagement
			   @UserID AS 'UserID' , @DisplayName AS 'DisplayName', @UserName AS 'UserName', @Email AS 'Email',
			   @ProfilePicture AS 'ProfilePicture',
			   --Roles
			   @Is_Agent AS 'Is_Agent', @Is_Client AS 'Is_Client',
			   --ApplicationSetting
			   @CompanyLogo AS 'CompanyLogo', @CompanyTitle AS 'CompanyTitle', @DefaultLanguage AS 'DefaultLanguage',
			   @DefaultPassword AS 'DefaultPassword', @Is_EasyAddOn_Visible AS 'Is_EasyAddOn_Visible',
			   @Is_Chat_Visible AS 'Is_Chat_Visible', 
			   @Is_Admin_Search AS 'Is_Admin_Search',@Is_Pickup AS 'Is_Pickup',
			   @Is_AssignTo_Dropdown AS 'Is_AssignTo_Dropdown', @Is_Close_Ticket AS 'Is_Close_Ticket',
			   @Is_Ticket_StartPage AS 'Is_Ticket_StartPage', @Is_EditRow_On_DoubleClick AS 'Is_EditRow_On_DoubleClick',
			   
			   --Agent/Client
			   @Is_Profile_Visible AS 'Is_Profile_Visible', @Is_Profile_Visible_Client AS 'Is_Profile_Visible_Client', 
			   @Is_CommonSetting_Visible AS 'Is_CommonSetting_Visible', @Is_CommonSetting_Visible_Client AS 'Is_CommonSetting_Visible_Client',
			   @Is_Help_Visible AS 'Is_Help_Visible', @Is_Help_Visible_Client AS 'Is_Help_Visible_Client', 
			   @Is_Solution_Visible AS 'Is_Solution_Visible', @Is_Solution_Visible_Client AS 'Is_Solution_Visible_Client',
			   @Is_ColumnChooser_Visible AS 'Is_ColumnChooser_Visible', @Is_ColumnChooser_Visible_Client AS 'Is_ColumnChooser_Visible_Client', 
			   @PageSize AS 'PageSize', @PageSize_Client AS 'PageSize_Client', 
			   @Is_Ticket_Visible_Client AS 'Is_Ticket_Visible_Client', --only for client
			   @Is_Search_Visible_Client AS 'Is_Search_Visible_Client', --only for client

			   @Is_Print AS 'Is_Print', @Is_Print_Client AS 'Is_Print_Client', 
			   @Is_Export AS 'Is_Export', @Is_Export_Client AS 'Is_Export_Client', 
			   @Is_Ticket_Search AS 'Is_Ticket_Search', @Is_Ticket_Search_Client AS 'Is_Ticket_Search_Client', 
			   @Is_Solution_Search AS 'Is_Solution_Search', @Is_Solution_Search_Client AS 'Is_Solution_Search_Client', 
			   @Is_Column_Filter_Ticket AS 'Is_Column_Filter_Ticket', @Is_Column_Filter_Ticket_Client AS 'Is_Column_Filter_Ticket_Client', 
			   @Is_Column_Filter_Solution AS 'Is_Column_Filter_Solution', @Is_Column_Filter_Solution_Client AS 'Is_Column_Filter_Solution_Client', 

			   @Is_Clone_Ticket AS 'Is_Clone_Ticket', @Is_Clone_Ticket_Client AS 'Is_Clone_Ticket_Client', 
			   @Is_Clone_Solution AS 'Is_Clone_Solution', @Is_Clone_Solution_Client AS 'Is_Clone_Solution_Client', 

			   --Role Permission
			   @Is_Full_Ticket AS 'Is_Full_Ticket', @Is_View_Ticket AS 'Is_View_Ticket', @Is_Add_Ticket AS 'Is_Add_Ticket',
			   @Is_Edit_Ticket AS 'Is_Edit_Ticket', @Is_Delete_Ticket AS 'Is_Delete_Ticket', -- Ticket Permission
			   @Is_Full_Ticket_Client AS 'Is_Full_Ticket_Client', @Is_View_Ticket_Client AS 'Is_View_Ticket_Client', @Is_Add_Ticket_Client AS 'Is_Add_Ticket_Client',
			   @Is_Edit_Ticket_Client AS 'Is_Edit_Ticket_Client', @Is_Delete_Ticket_Client AS 'Is_Delete_Ticket_Client', -- Ticket Permission Client

			   @Is_Full_Summary AS 'Is_Full_Summary', @Is_View_Summary AS 'Is_View_Summary', @Is_Add_Summary AS 'Is_Add_Summary',
			   @Is_Edit_Summary AS 'Is_Edit_Summary', @Is_Delete_Summary AS 'Is_Delete_Summary', -- Summary Permission
			   
			   @Is_Full_Solution AS 'Is_Full_Solution', @Is_View_Solution AS 'Is_View_Solution', @Is_Add_Solution AS 'Is_Add_Solution',
			   @Is_Edit_Solution AS 'Is_Edit_Solution', @Is_Delete_Solution AS 'Is_Delete_Solution', -- Solution Permission
			   @Is_Full_Solution_Client AS 'Is_Full_Solution_Client', @Is_View_Solution_Client AS 'Is_View_Solution_Client', @Is_Add_Solution_Client AS 'Is_Add_Solution_Client',
			   @Is_Edit_Solution_Client AS 'Is_Edit_Solution_Client', @Is_Delete_Solution_Client AS 'Is_Delete_Solution_Client', -- Solution Permission Client
			   
			   @Is_Full_Admin AS 'Is_Full_Admin', @Is_View_Admin AS 'Is_View_Admin', @Is_Add_Admin AS 'Is_Add_Admin',
			   @Is_Edit_Admin AS 'Is_Edit_Admin', @Is_Delete_Admin AS 'Is_Delete_Admin' -- Admin Permission
			    
				
		
END

GO
/****** Object:  StoredProcedure [dbo].[Requester_UserManagement_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Requester_UserManagement_Update 
-- =============================================
CREATE PROCEDURE [dbo].[Requester_UserManagement_Update]
	@UserID BIGINT OUTPUT,	
	@DisplayName NVARCHAR(MAX),
	--@UserName NVARCHAR(MAX),	
	@Email NVARCHAR(MAX),
	@PhoneNo NVARCHAR(MAX),
	@CellPhoneNo NVARCHAR(MAX),
	@City NVARCHAR(MAX),
	@State NVARCHAR(MAX),
	@Country NVARCHAR(MAX),
	@Pincode NVARCHAR(MAX),
	@JobTitle NVARCHAR(MAX),
	@Address NVARCHAR(MAX),
	@TimeZoneID INT,
	@Organization NVARCHAR(MAX),	
	@Description NVARCHAR(MAX)	
AS
BEGIN
	SET ARITHABORT ON;
	
	IF(@UserID > 0)
	BEGIN 
		
			UPDATE UserManagement
			SET DisplayName = @DisplayName,
				Email = @Email,
				PhoneNo = @PhoneNo,
				TimeZoneID = @TimeZoneID
				--ProfilePicture = @ProfilePicture,
			WHERE UserID = @UserID

	END
	
END

GO
/****** Object:  StoredProcedure [dbo].[ResetDefaultPassword_User]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ResetDefaultPassword_User 
-- ===============================================
CREATE PROCEDURE [dbo].[ResetDefaultPassword_User]
	@UserID NVARCHAR(MAX),
	@DefaultPassword NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;

	UPDATE UserManagement 
	SET [Password] = @DefaultPassword
	WHERE UserID IN (SELECT items FROM dbo.Split(@UserID,','))

END

GO
/****** Object:  StoredProcedure [dbo].[ResetPassword]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ResetPassword 
-- =============================================
CREATE PROCEDURE [dbo].[ResetPassword]
	@ReturnValue INT OUTPUT,
	@UserID BIGINT = 0,
	@UserName NVARCHAR(MAX),
	@Email NVARCHAR(MAX),
	@Password NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;

	IF(@UserID  > 0) --Change Password
	BEGIN

			UPDATE UserManagement
			SET [Password] = @Password
			WHERE UserID = @UserID

			SELECT @ReturnValue = 1;
	END
	ELSE
	BEGIN			--Reset Password
		DECLARE @cnt INT =0 ;
		SELECT @cnt = COUNT(1) FROM UserManagement WHERE UserName = @UserName AND Email = @Email
		IF(@cnt > 0)
		BEGIN
		
			UPDATE UserManagement
			SET [Password] = @Password
			WHERE UserName = @UserName AND Email = @Email 

			SELECT @ReturnValue = 1;

		END
		ELSE
		BEGIN
			SELECT @ReturnValue = -1;
		END
	END

END

GO
/****** Object:  StoredProcedure [dbo].[RolePermission_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- RolePermission_Update 
-- =============================================
CREATE PROCEDURE [dbo].[RolePermission_Update]
	@RolePermissionID BIGINT OUTPUT,
	@RoleID BIGINT,
	@MenuID BIGINT,	
	@Is_Full bit,
	@Is_View bit,
	@Is_Add bit,
	@Is_Edit bit,
	@Is_Delete bit,
	@Is_Active bit
AS
BEGIN
	SET ARITHABORT ON;
	
	DECLARE @Cnt INT = 0;
	SELECT @Cnt = COUNT(1) FROM RolePermission WHERE RolePermissionID = @RolePermissionID AND RoleID = @RoleID
	IF(@Cnt > 0)
	BEGIN 
		
			UPDATE RolePermission
			SET Is_Full = @Is_Full,
				Is_View = @Is_View,
				Is_Add = @Is_Add,
				Is_Edit = @Is_Edit,
				Is_Delete = @Is_Delete
			WHERE RolePermissionID = @RolePermissionID

	END
	ELSE
	BEGIN
			
			INSERT INTO RolePermission
			(
			RoleID,
			MenuID,
			Is_Full,
			Is_View,
			Is_Add,
			Is_Edit,
			Is_Delete,
			Is_Active,
			CreatedDate
			)
			VALUES (
			@RoleID,
			@MenuID,
			@Is_Full,
			@Is_View,
			@Is_Add,
			@Is_Edit,
			@Is_Delete,
			1,
			GETDATE()
			)

			SELECT @RolePermissionID = SCOPE_IDENTITY()
	END

END

GO
/****** Object:  StoredProcedure [dbo].[Roles_Delete]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Roles_Delete
-- =============================================
CREATE PROCEDURE [dbo].[Roles_Delete]
	@RoleID NVARCHAR(MAX)	
AS
BEGIN
	SET ARITHABORT ON;
	
	DECLARE @count INT = 0;
	SELECT @count =  COUNT(UserID) FROM UserManagement WHERE RoleID = @RoleID
	IF(@count <= 0)
	BEGIN
		DELETE FROM RolePermission WHERE RoleID  IN (SELECT items FROM dbo.Split(@RoleID,','))
		DELETE FROM Roles WHERE RoleID IN (SELECT items FROM dbo.Split(@RoleID,','))
	END

END

GO
/****** Object:  StoredProcedure [dbo].[Roles_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Roles_Update 
-- =============================================
CREATE PROCEDURE [dbo].[Roles_Update]
	@RoleID BIGINT OUTPUT,
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@Is_Agent bit,
	@Is_Client bit,
	@Is_Active bit
AS
BEGIN
	SET ARITHABORT ON;
	
	IF(@RoleID > 0)
	BEGIN 
		
			UPDATE Roles
			SET [Name] = @Name,
				[Description] = @Description,
				Is_Agent = @Is_Agent,
				Is_Client = @Is_Client,
				Is_Active = @Is_Active
			WHERE RoleID = @RoleID

	END
	ELSE
	BEGIN
			
			INSERT INTO Roles
			(
			[Name],
			[Description],
			Is_Agent,
			Is_Client,
			Is_Active,
			CreatedDate
			)
			VALUES (
			@Name,
			@Description,
			@Is_Agent,
			@Is_Client,
			@Is_Active,
			GETDATE()
			)

			SELECT @RoleID = SCOPE_IDENTITY()
	END

END

GO
/****** Object:  StoredProcedure [dbo].[Scan_Search]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Get_Hash_ByID 1
-- =============================================
CREATE PROCEDURE [dbo].[Scan_Search]
	@HashContent nvarchar(MAX) = 0
AS
BEGIN
	SET ARITHABORT ON;
	
	SELECT * FROM Hash 
	WHERE HashContent = @HashContent

END

GO
/****** Object:  StoredProcedure [dbo].[ScanType_Delete]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ScanType_Delete 
-- =============================================
CREATE PROCEDURE [dbo].[ScanType_Delete]
	@ScanTypeID NVARCHAR(MAX)	
AS
BEGIN
	SET ARITHABORT ON;
	
	DELETE FROM ScanType WHERE ScanTypeID IN (SELECT items FROM dbo.Split(@ScanTypeID,','))

END

GO
/****** Object:  StoredProcedure [dbo].[ScanType_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- ScanType_Update 
-- =============================================
CREATE PROCEDURE [dbo].[ScanType_Update]
	@ScanTypeID BIGINT OUTPUT,
	@Name NVARCHAR(MAX),
	@Description NVARCHAR(MAX),
	@Is_Active bit
AS
BEGIN
	SET ARITHABORT ON;
	
	IF(@ScanTypeID > 0)
	BEGIN 
		
			UPDATE ScanType
			SET [Name] = @Name,
				[Description] = @Description,
				Is_Active = @Is_Active
			WHERE ScanTypeID = @ScanTypeID

	END
	ELSE
	BEGIN
			
			INSERT INTO ScanType
			(
			[Name],
			[Description],
			Is_Active,
			CreatedDate
			)
			VALUES (
			@Name,
			@Description,
			@Is_Active,
			GETDATE()
			)

			SELECT @ScanTypeID = SCOPE_IDENTITY()
	END

END

GO
/****** Object:  StoredProcedure [dbo].[Update_User_ProfilePic]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- Update_User_ProfilePic 
-- =============================================
CREATE PROCEDURE [dbo].[Update_User_ProfilePic]
	@UserID BIGINT,
	@ProfilePicture NVARCHAR(MAX)
AS
BEGIN
	SET ARITHABORT ON;
	
		UPDATE UserManagement
		SET ProfilePicture = @ProfilePicture
		WHERE UserID = @UserID

END

GO
/****** Object:  StoredProcedure [dbo].[UserManagement_Delete]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- UserManagement_Delete 
-- =============================================
CREATE PROCEDURE [dbo].[UserManagement_Delete]
	@UserID NVARCHAR(MAX)	
AS
BEGIN
	SET ARITHABORT ON;
	
	DELETE FROM UserManagement WHERE UserID IN (SELECT items FROM dbo.Split(@UserID,','))

END

GO
/****** Object:  StoredProcedure [dbo].[UserManagement_Update]    Script Date: 4/20/2022 10:13:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- UserManagement_Update 
-- =============================================
CREATE PROCEDURE [dbo].[UserManagement_Update]
	@UserID BIGINT OUTPUT,
	@RoleID BIGINT,
	@DisplayName NVARCHAR(MAX),
	@UserName NVARCHAR(MAX),
	@Password NVARCHAR(MAX),
	@Email NVARCHAR(MAX),
	@PhoneNo NVARCHAR(MAX),
	@TimeZoneID INT,
	@Is_SendMail_Password BIT,
	@Description NVARCHAR(MAX),
	--@ProfilePicture NVARCHAR(MAX),
	@Is_Active BIT
AS
BEGIN
	SET ARITHABORT ON;
	
	IF(@UserID > 0)
	BEGIN 
		
			UPDATE UserManagement
			SET RoleID = @RoleID,
				DisplayName = @DisplayName,
				[Password] = @Password,
				Email = @Email,
				PhoneNo = @PhoneNo,
				TimeZoneID = @TimeZoneID,
				Is_SendMail_Password = @Is_SendMail_Password,
				--ProfilePicture = @ProfilePicture,
				Is_Active = @Is_Active
			WHERE UserID = @UserID

	END
	ELSE
	BEGIN
			
			IF NOT EXISTS(SELECT TOP 1 UserID FROM UserManagement WHERE UserName = @UserName)
			BEGIN
				INSERT INTO UserManagement
				(
				RoleID,
				[DisplayName],
				[UserName],
				[Password],
				[Email],
				PhoneNo,
				TimeZoneID,
				Is_SendMail_Password,
				--ProfilePicture,
				Is_Active,
				CreatedDate
				)
				VALUES (
				@RoleID,
				@DisplayName,
				@UserName,
				@Password,
				@Email,
				@PhoneNo,
				@TimeZoneID,
				@Is_SendMail_Password,
				--@ProfilePicture,
				@Is_Active,
				GETDATE()
				)

				SELECT @UserID = SCOPE_IDENTITY()
			END
			ELSE
			BEGIN
				SELECT @UserID = -1 ---1 for Username not available
			END
	END

END

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "History"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 214
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ST"
            Begin Extent = 
               Top = 6
               Left = 252
               Bottom = 136
               Right = 422
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Loc"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UsrCre"
            Begin Extent = 
               Top = 138
               Left = 246
               Bottom = 268
               Right = 452
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5730
         Alias = 3990
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_Historys'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'    Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_Historys'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vw_Historys'
GO
USE [master]
GO
ALTER DATABASE [db_marvir] SET  READ_WRITE 
GO
