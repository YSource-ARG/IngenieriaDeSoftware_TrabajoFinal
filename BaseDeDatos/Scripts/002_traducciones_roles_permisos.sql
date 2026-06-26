USE TP_Ing_De_Software;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Traducciones TABLE
(
    CodigoIdioma nvarchar(10) NOT NULL,
    Clave nvarchar(200) NOT NULL,
    Texto nvarchar(500) NOT NULL,
    PRIMARY KEY (CodigoIdioma, Clave)
);

INSERT INTO @Traducciones
(
    CodigoIdioma,
    Clave,
    Texto
)
VALUES
    -- Botón Permisos de Gestión de usuarios.
    (N'es-AR', N'Usuarios.Permisos', N'Permisos'),
    (N'en-US', N'Usuarios.Permisos', N'Permissions'),

    -- Formulario Gestión de roles: textos estáticos del diseñador.
    (N'es-AR', N'Roles.Gestion.TituloVentana', N'Gestión de roles'),
    (N'en-US', N'Roles.Gestion.TituloVentana', N'Role management'),
    (N'es-AR', N'Roles.Gestion.Titulo', N'Gestión de roles'),
    (N'en-US', N'Roles.Gestion.Titulo', N'Role management'),
    (N'es-AR', N'Roles.Gestion.Ayuda', N'Seleccione un rol existente para administrar su composición o cree uno nuevo cargando sus atributos y marcando permisos o subroles en el árbol.'),
    (N'en-US', N'Roles.Gestion.Ayuda', N'Select an existing role to manage its composition, or create a new one by entering its details and selecting permissions or subroles in the tree.'),
    (N'es-AR', N'Roles.Gestion.RolExistente', N'Rol existente'),
    (N'en-US', N'Roles.Gestion.RolExistente', N'Existing role'),
    (N'es-AR', N'Roles.Gestion.NuevoRol', N'Nuevo rol'),
    (N'en-US', N'Roles.Gestion.NuevoRol', N'New role'),
    (N'es-AR', N'Roles.Gestion.Nombre', N'Nombre'),
    (N'en-US', N'Roles.Gestion.Nombre', N'Name'),
    (N'es-AR', N'Roles.Gestion.Codigo', N'Código'),
    (N'en-US', N'Roles.Gestion.Codigo', N'Code'),
    (N'es-AR', N'Roles.Gestion.Descripcion', N'Descripción'),
    (N'en-US', N'Roles.Gestion.Descripcion', N'Description'),
    (N'es-AR', N'Roles.Gestion.Activo', N'Activo'),
    (N'en-US', N'Roles.Gestion.Activo', N'Active'),
    (N'es-AR', N'Roles.Gestion.Cancelar', N'Cancelar'),
    (N'en-US', N'Roles.Gestion.Cancelar', N'Cancel'),
    (N'es-AR', N'Roles.Gestion.Cerrar', N'Cerrar'),
    (N'en-US', N'Roles.Gestion.Cerrar', N'Close'),

    -- Formulario Gestión de roles: textos generados dinámicamente.
    (N'es-AR', N'Roles.Gestion.Roles', N'Roles y subroles'),
    (N'en-US', N'Roles.Gestion.Roles', N'Roles and subroles'),
    (N'es-AR', N'Roles.Gestion.Permisos', N'Permisos directos'),
    (N'en-US', N'Roles.Gestion.Permisos', N'Direct permissions'),
    (N'es-AR', N'Roles.Gestion.RolActual', N' - rol actual'),
    (N'en-US', N'Roles.Gestion.RolActual', N' - current role'),
    (N'es-AR', N'Roles.Gestion.CreadoOk', N'El rol se creó correctamente.'),
    (N'en-US', N'Roles.Gestion.CreadoOk', N'The role was created successfully.'),
    (N'es-AR', N'Roles.Gestion.GuardadoOk', N'La composición del rol se guardó correctamente.'),
    (N'en-US', N'Roles.Gestion.GuardadoOk', N'The role composition was saved successfully.'),
    (N'es-AR', N'Roles.Gestion.CrearRol', N'Crear rol'),
    (N'en-US', N'Roles.Gestion.CrearRol', N'Create role'),
    (N'es-AR', N'Roles.Gestion.GuardarComposicion', N'Guardar composición'),
    (N'en-US', N'Roles.Gestion.GuardarComposicion', N'Save composition'),

    -- Formulario Asignación de permisos a usuarios.
    (N'es-AR', N'Permisos.Asignacion.Titulo', N'Asignación de permisos'),
    (N'en-US', N'Permisos.Asignacion.Titulo', N'Permission assignment'),
    (N'es-AR', N'Permisos.Asignacion.Usuario', N'Usuario: {0}'),
    (N'en-US', N'Permisos.Asignacion.Usuario', N'User: {0}'),
    (N'es-AR', N'Permisos.Asignacion.Ayuda', N'Al marcar un rol, sus hijos se marcan visualmente en forma recursiva dentro del árbol Composite.'),
    (N'en-US', N'Permisos.Asignacion.Ayuda', N'Selecting a role visually selects its children recursively within the Composite tree.'),
    (N'es-AR', N'Permisos.Asignacion.Roles', N'Roles'),
    (N'en-US', N'Permisos.Asignacion.Roles', N'Roles'),
    (N'es-AR', N'Permisos.Asignacion.PermisosDirectos', N'Permisos directos'),
    (N'en-US', N'Permisos.Asignacion.PermisosDirectos', N'Direct permissions'),
    (N'es-AR', N'Permisos.Asignacion.GuardadoOk', N'Los permisos del usuario se guardaron correctamente.'),
    (N'en-US', N'Permisos.Asignacion.GuardadoOk', N'User permissions were saved successfully.');

IF EXISTS
(
    SELECT 1
    FROM @Traducciones origen
    LEFT JOIN dbo.Idioma idioma
        ON idioma.Codigo = origen.CodigoIdioma
    WHERE idioma.Id IS NULL
)
BEGIN
    THROW 50001, N'No se encontraron en dbo.Idioma todos los idiomas requeridos por el script.', 1;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE destino
    SET destino.Texto = origen.Texto
    FROM dbo.Traduccion destino
    INNER JOIN dbo.Idioma idioma
        ON idioma.Id = destino.IdiomaId
    INNER JOIN @Traducciones origen
        ON origen.CodigoIdioma = idioma.Codigo
       AND origen.Clave = destino.Clave;

    INSERT INTO dbo.Traduccion
    (
        Id,
        Clave,
        IdiomaId,
        Texto
    )
    SELECT
        NEWID(),
        origen.Clave,
        idioma.Id,
        origen.Texto
    FROM @Traducciones origen
    INNER JOIN dbo.Idioma idioma
        ON idioma.Codigo = origen.CodigoIdioma
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Traduccion existente
        WHERE existente.IdiomaId = idioma.Id
          AND existente.Clave = origen.Clave
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;

SELECT
    idioma.Codigo AS CodigoIdioma,
    traduccion.Clave,
    traduccion.Texto
FROM dbo.Traduccion traduccion
INNER JOIN dbo.Idioma idioma
    ON idioma.Id = traduccion.IdiomaId
INNER JOIN @Traducciones esperada
    ON esperada.CodigoIdioma = idioma.Codigo
   AND esperada.Clave = traduccion.Clave
ORDER BY
    traduccion.Clave,
    idioma.Codigo;
GO
