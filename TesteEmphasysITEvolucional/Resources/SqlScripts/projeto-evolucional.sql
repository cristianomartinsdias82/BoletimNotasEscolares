CREATE DATABASE [projeto-evolucional]
GO

CREATE TABLE [dbo].[Disciplines](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Disciplines] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Students](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](20) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[StudentsGrades](
	[StudentId] [int] NOT NULL,
	[DisciplineId] [int] NOT NULL,
	[Grade] [decimal](4, 2) NOT NULL,
 CONSTRAINT [PK_StudentsGrades] PRIMARY KEY CLUSTERED 
(
	[StudentId] ASC,
	[DisciplineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](MAX) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[StudentsGrades]  WITH CHECK ADD  CONSTRAINT [FK_StudentsGrades_Disciplines1] FOREIGN KEY([DisciplineId])
REFERENCES [dbo].[Disciplines] ([Id])
GO
ALTER TABLE [dbo].[StudentsGrades] CHECK CONSTRAINT [FK_StudentsGrades_Disciplines1]
GO
ALTER TABLE [dbo].[StudentsGrades]  WITH CHECK ADD  CONSTRAINT [FK_StudentsGrades_Students1] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Students] ([Id])
GO
ALTER TABLE [dbo].[StudentsGrades] CHECK CONSTRAINT [FK_StudentsGrades_Students1]
GO
ALTER TABLE [dbo].[StudentsGrades]  WITH CHECK ADD  CONSTRAINT [CT_CK_StudentsGrades_Grade] CHECK  (([Grade]>=(0) AND [Grade]<=(10)))
GO
ALTER TABLE [dbo].[StudentsGrades] CHECK CONSTRAINT [CT_CK_StudentsGrades_Grade]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UN_Disciplines_Title] ON [dbo].[Disciplines]
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UN_Students_Name] ON [dbo].[Students]
(
	[FirstName] ASC,
	[LastName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UN_Users_Username] ON [dbo].[Users]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO

CREATE VIEW [dbo].[V_RandomValue]
AS
SELECT RAND() * 10 AS [Value]
GO

CREATE FUNCTION [dbo].[F_RandomValue]()
RETURNS DECIMAL(4,2)
AS
BEGIN
   DECLARE @RtnValue DECIMAL(4,2)
   
   SELECT @RtnValue = (SELECT [Value] FROM dbo.V_RandomValue)
   RETURN @RtnValue;
END
GO

CREATE VIEW [dbo].[V_StudentsGrades]
AS

	SELECT
		FirstName Nome,
		LastName Sobrenome,
		[História],
		[Geografia],
		[Português],
		[Filosofia],
		[Física],
		[Matemática],
		[Biologia],
		[Química],
		[Inglês]
	FROM
	(
		SELECT
			S.FirstName,
			S.LastName,
			D.Title,
			SG.Grade
		FROM Students S, Disciplines D, StudentsGrades SG
		WHERE	S.Id = SG.StudentId
			AND D.Id = SG.DisciplineId
	) sq
	PIVOT
	(
		SUM(Grade) FOR Title IN([História],	[Geografia],[Português],[Filosofia],[Física],[Matemática],[Biologia],[Química],[Inglês])
	) AS pt
GO