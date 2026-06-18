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
        NombreCompleto nvarchar(200) NULL,
        Email nvarchar(255) NULL,
        PasswordHash nvarchar(500) NOT NULL,
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

-- Historial de cambios de email de usuarios.
-- Permite conservar trazabilidad de cada modificación realizada sobre
-- la propiedad Email, registrando valor anterior, valor nuevo, fecha
-- y usuario que realizó el cambio.
IF OBJECT_ID('dbo.UsuarioEmailHistorial', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioEmailHistorial
    (
        Id uniqueidentifier NOT NULL,
        UsuarioId uniqueidentifier NOT NULL,
        EmailAnterior nvarchar(255) NULL,
        EmailNuevo nvarchar(255) NULL,
        FechaCambio datetime2(7) NOT NULL
            CONSTRAINT DF_UsuarioEmailHistorial_FechaCambio DEFAULT (SYSDATETIME()),
        UsuarioCambioId uniqueidentifier NULL,
        UsuarioCambioNombre nvarchar(100) NULL,
        CONSTRAINT PK_UsuarioEmailHistorial PRIMARY KEY (Id),
        CONSTRAINT FK_UsuarioEmailHistorial_Usuario FOREIGN KEY (UsuarioId)
            REFERENCES dbo.Usuario(Id)
    )
END
GO

IF COL_LENGTH('dbo.Usuario', 'NombreCompleto') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD NombreCompleto nvarchar(150) NULL
END
GO

IF COL_LENGTH('dbo.Usuario', 'Email') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD Email nvarchar(255) NULL
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

-- Bloqueo independiente del estado Activo.
-- No se reutiliza Activo porque Activo representa una decisión administrativa
-- y BloqueadoPorIntegridad representa un bloqueo temporal por falla de integridad.
IF COL_LENGTH('dbo.Usuario', 'BloqueadoPorIntegridad') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD BloqueadoPorIntegridad bit NOT NULL
        CONSTRAINT DF_Usuario_BloqueadoPorIntegridad DEFAULT (0)
END
GO

-- DVH: dígito verificador horizontal de cada usuario.
-- Se guarda en la misma entidad protegida para detectar modificaciones externas
-- sobre los datos del registro.
IF COL_LENGTH('dbo.Usuario', 'DigitoVerificadorHorizontal') IS NULL
BEGIN
    ALTER TABLE dbo.Usuario
    ADD DigitoVerificadorHorizontal nvarchar(256) NOT NULL
        CONSTRAINT DF_Usuario_DigitoVerificadorHorizontal DEFAULT ('')
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

-- DVV: dígito verificador vertical.
-- Se guarda separado de la entidad Usuario porque representa la integridad
-- del conjunto completo de registros controlados.
IF OBJECT_ID('dbo.DigitoVerificadorVertical', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DigitoVerificadorVertical
    (
        Entidad nvarchar(100) NOT NULL,
        Valor nvarchar(256) NOT NULL,
        FechaCalculo datetime2(7) NOT NULL
            CONSTRAINT DF_DigitoVerificadorVertical_FechaCalculo DEFAULT (SYSDATETIME()),
        CONSTRAINT PK_DigitoVerificadorVertical PRIMARY KEY (Entidad)
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
    Activo = 1,
    BloqueadoPorIntegridad = 0
    WHERE NombreUsuario = 'admin'
END
GO

-- Multiidioma - Idiomas disponibles del sistema.
-- Permite agregar nuevos idiomas sin modificar la estructura de la base.
IF OBJECT_ID('dbo.Idioma', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Idioma
    (
        Id uniqueidentifier NOT NULL,
        Codigo nvarchar(10) NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        Activo bit NOT NULL
            CONSTRAINT DF_Idioma_Activo DEFAULT (1),
        CONSTRAINT PK_Idioma PRIMARY KEY (Id),
        CONSTRAINT UQ_Idioma_Codigo UNIQUE (Codigo)
    )
END
GO

-- Multiidioma - Traducciones por clave e idioma.
-- Cada control visible usa una clave estable en Tag, por ejemplo Login.Usuario.
IF OBJECT_ID('dbo.Traduccion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Traduccion
    (
        Id uniqueidentifier NOT NULL,
        Clave nvarchar(200) NOT NULL,
        IdiomaId uniqueidentifier NOT NULL,
        Texto nvarchar(500) NOT NULL,
        CONSTRAINT PK_Traduccion PRIMARY KEY (Id),
        CONSTRAINT FK_Traduccion_Idioma FOREIGN KEY (IdiomaId)
            REFERENCES dbo.Idioma(Id),
        CONSTRAINT UQ_Traduccion_Clave_Idioma UNIQUE (Clave, IdiomaId)
    )
END
GO

-- Idioma base del sistema.
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Idioma
    WHERE Codigo = 'es-AR'
)
BEGIN
    INSERT INTO dbo.Idioma
    (
        Id,
        Codigo,
        Nombre,
        Activo
    )
    VALUES
    (
        NEWID(),
        'es-AR',
        'Espanol Argentina',
        1
    )
END
GO

-- Idioma adicional inicial.
-- Las traducciones se administran desde el ABM de traducciones.
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Idioma
    WHERE Codigo = 'en-US'
)
BEGIN
    INSERT INTO dbo.Idioma
    (
        Id,
        Codigo,
        Nombre,
        Activo
    )
    VALUES
    (
        NEWID(),
        'en-US',
        'English',
        1
    )
END
GO