IF DB_ID('TP_Ing_De_Software') IS NULL CREATE DATABASE TP_Ing_De_Software
GO

USE TP_Ing_De_Software
GO

IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuario
    (
        Id uniqueidentifier NOT NULL,
        NombreUsuario nvarchar(100) NOT NULL,
        NombreCompleto nvarchar(150) NULL,
        PasswordHash nvarchar(255) NOT NULL,
        Activo bit NOT NULL CONSTRAINT DF_Usuario_Activo DEFAULT (1),
        DebeCambiarPassword bit NOT NULL
            CONSTRAINT DF_Usuario_DebeCambiarPassword DEFAULT (0),
        IntentosFallidosLogin int NOT NULL
            CONSTRAINT DF_Usuario_IntentosFallidosLogin DEFAULT (0),
        BloqueadoHasta datetime2(7) NULL,
        FechaCreacion datetime2(7) NOT NULL
            CONSTRAINT DF_Usuario_FechaCreacion DEFAULT (SYSDATETIME()),
        FechaUltimoAcceso datetime2(7) NULL,
        CONSTRAINT PK_Usuario PRIMARY KEY (Id)
    )
END
GO

IF COL_LENGTH('dbo.Usuario', 'NombreCompleto') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD NombreCompleto nvarchar(150) NULL
END
GO

IF COL_LENGTH('dbo.Usuario', 'DebeCambiarPassword') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD DebeCambiarPassword bit NOT NULL
        CONSTRAINT DF_Usuario_DebeCambiarPassword DEFAULT (0)
END
GO

IF COL_LENGTH('dbo.Usuario', 'IntentosFallidosLogin') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD IntentosFallidosLogin int NOT NULL
        CONSTRAINT DF_Usuario_IntentosFallidosLogin DEFAULT (0)
END
GO

IF COL_LENGTH('dbo.Usuario', 'BloqueadoHasta') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD BloqueadoHasta datetime2(7) NULL
END
GO

IF COL_LENGTH('dbo.Usuario', 'PasswordSalt') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Usuario DROP COLUMN PasswordSalt
END
GO

IF OBJECT_ID('dbo.Bitacora', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bitacora
    (
        Id uniqueidentifier NOT NULL,
        Fecha datetime2(7) NOT NULL
            CONSTRAINT DF_Bitacora_Fecha DEFAULT (SYSDATETIME()),
        UsuarioId uniqueidentifier NULL,
        Usuario nvarchar(100) NULL,
        Modulo nvarchar(100) NOT NULL,
        Accion nvarchar(100) NOT NULL,
        Descripcion nvarchar(500) NULL,
        Tipo nvarchar(50) NOT NULL,
        CONSTRAINT PK_Bitacora PRIMARY KEY (Id)
    )
END
GO

IF COL_LENGTH('dbo.Bitacora', 'UsuarioId') IS NULL
BEGIN
    ALTER TABLE dbo.Bitacora
    ADD UsuarioId uniqueidentifier NULL
END
GO

IF COL_LENGTH('dbo.Bitacora', 'Modulo') IS NULL
BEGIN
    ALTER TABLE dbo.Bitacora
    ADD Modulo nvarchar(100) NOT NULL
        CONSTRAINT DF_Bitacora_Modulo DEFAULT ('General')
END
GO

DECLARE @pkUsuarioActual sysname;

SELECT @pkUsuarioActual = kc.name
FROM sys.key_constraints kc
WHERE kc.parent_object_id = OBJECT_ID('dbo.Usuario')
  AND kc.[type] = 'PK';

IF @pkUsuarioActual IS NOT NULL
   AND @pkUsuarioActual <> 'PK_Usuario'
BEGIN
    EXEC sp_rename @pkUsuarioActual, 'PK_Usuario';
END
GO

DECLARE @pkBitacoraActual sysname;

SELECT @pkBitacoraActual = kc.name
FROM sys.key_constraints kc
WHERE kc.parent_object_id = OBJECT_ID('dbo.Bitacora')
  AND kc.[type] = 'PK';

IF @pkBitacoraActual IS NOT NULL
   AND @pkBitacoraActual <> 'PK_Bitacora'
BEGIN
    EXEC sp_rename @pkBitacoraActual, 'PK_Bitacora';
END
GO

IF OBJECT_ID('dbo.FK_Bitacora_Usuario', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.Bitacora
    ADD CONSTRAINT FK_Bitacora_Usuario
        FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuario(Id)
END
GO

CREATE OR ALTER PROCEDURE dbo.Bitacora_Registrar
    @Id UNIQUEIDENTIFIER,
    @Fecha DATETIME2(7),
    @UsuarioId UNIQUEIDENTIFIER = NULL,
    @Usuario NVARCHAR(100) = NULL,
    @Modulo NVARCHAR(100),
    @Accion NVARCHAR(100),
    @Descripcion NVARCHAR(500) = NULL,
    @Tipo NVARCHAR(50)
AS
BEGIN
    INSERT INTO dbo.Bitacora
    (
        Id,
        Fecha,
        UsuarioId,
        Usuario,
        Modulo,
        Accion,
        Descripcion,
        Tipo
    )
    VALUES
    (
        @Id,
        @Fecha,
        @UsuarioId,
        @Usuario,
        @Modulo,
        @Accion,
        @Descripcion,
        @Tipo
    )
END
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Usuario
    WHERE NombreUsuario = 'admin'
)
BEGIN
    INSERT INTO dbo.Usuario
    (
        Id,
        NombreUsuario,
        NombreCompleto,
        PasswordHash,
        Activo,
        DebeCambiarPassword,
        IntentosFallidosLogin,
        BloqueadoHasta,
        FechaCreacion,
        FechaUltimoAcceso
    )
    VALUES
    (
        NEWID(),
        'admin',
        'Administrador',
        'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=',
        1,
        0,
        0,
        NULL,
        SYSDATETIME(),
        NULL
    )
END
ELSE
BEGIN
    UPDATE dbo.Usuario
    SET PasswordHash = 'A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=',
        Activo = 1
    WHERE NombreUsuario = 'admin'
END
GO