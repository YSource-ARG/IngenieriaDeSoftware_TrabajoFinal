using BE;
using BE.Permisos;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Integridad
{
    public class DigitoVerificadorRepositorio : IDigitoVerificadorRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public DigitoVerificadorRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<Usuario> ListarUsuariosParaIntegridad()
        {
            const string sql = @"
                SELECT
                    Id,
                    NombreUsuario,
                    NombreCompleto,
                    Email,
                    IdiomaPreferidoId,
                    PasswordHash,
                    Activo,
                    DebeCambiarPassword,
                    IntentosFallidosLogin,
                    BloqueadoHasta,
                    BloqueadoPorIntegridad,
                    DigitoVerificadorHorizontal,
                    FechaCreacion,
                    FechaUltimoAcceso
                FROM dbo.Usuario
                ORDER BY Id";

            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(MapearUsuario(reader));
                        }
                    }
                }

                return usuarios;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron listar los usuarios para verificar integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron obtener los usuarios para verificar integridad.",
                    ex
                );
            }
        }

        public List<ComponentePermisos> ListarComponentesPermisosParaIntegridad()
        {
            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion,
                    DigitoVerificadorHorizontal
                FROM dbo.PermisoComponente
                ORDER BY Id";

            List<ComponentePermisos> componentes =
                new List<ComponentePermisos>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            componentes.Add(MapearComponentePermisos(reader));
                        }
                    }
                }

                return componentes;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron listar los componentes de permisos para verificar integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron obtener los componentes de permisos para verificar integridad.",
                    ex
                );
            }
        }

        public List<RolComponente> ListarRolComponentesParaIntegridad()
        {
            const string sql = @"
                SELECT
                    RolId,
                    ComponenteHijoId,
                    DigitoVerificadorHorizontal
                FROM dbo.RolComponente
                ORDER BY RolId, ComponenteHijoId";

            List<RolComponente> relaciones = new List<RolComponente>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            relaciones.Add(MapearRolComponente(reader));
                        }
                    }
                }

                return relaciones;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron listar las relaciones de roles para verificar integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron obtener las relaciones de roles para verificar integridad.",
                    ex
                );
            }
        }

        public List<UsuarioPermisoComponente>
            ListarUsuarioPermisoComponentesParaIntegridad()
        {
            const string sql = @"
                SELECT
                    UsuarioId,
                    ComponenteId,
                    FechaAsignacion,
                    AsignadoPorUsuarioId,
                    DigitoVerificadorHorizontal
                FROM dbo.UsuarioPermisoComponente
                ORDER BY UsuarioId, ComponenteId";

            List<UsuarioPermisoComponente> asignaciones =
                new List<UsuarioPermisoComponente>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            asignaciones.Add(
                                MapearUsuarioPermisoComponente(reader)
                            );
                        }
                    }
                }

                return asignaciones;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron listar las asignaciones de permisos para verificar integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron obtener las asignaciones de permisos para verificar integridad.",
                    ex
                );
            }
        }

        public string ObtenerDigitoVerificadorVertical(string entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad))
            {
                throw new ArgumentException(
                    "La entidad no puede estar vacía.",
                    nameof(entidad)
                );
            }

            const string sql = @"
                SELECT Valor
                FROM dbo.DigitoVerificadorVertical
                WHERE Entidad = @Entidad";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@Entidad", SqlDbType.NVarChar, 100)
                        .Value = entidad.Trim();

                    connection.Open();

                    object resultado = command.ExecuteScalar();

                    return resultado == null || resultado == DBNull.Value
                        ? null
                        : resultado.ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo obtener el dígito verificador vertical.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo abrir o utilizar la conexión con la base de datos.",
                    ex
                );
            }
        }

        public void ActualizarDigitoVerificadorHorizontal(
            Guid idUsuario,
            string valor)
        {
            ValidarIdentificador(idUsuario, nameof(idUsuario));
            ValidarValorDigito(valor);

            const string sql = @"
                UPDATE dbo.Usuario
                SET DigitoVerificadorHorizontal = @DigitoVerificadorHorizontal
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@Id", SqlDbType.UniqueIdentifier)
                        .Value = idUsuario;

                    command.Parameters
                        .Add(
                            "@DigitoVerificadorHorizontal",
                            SqlDbType.NVarChar,
                            256
                        )
                        .Value = valor;

                    connection.Open();

                    VerificarFilaActualizada(
                        command.ExecuteNonQuery(),
                        "No se encontró el usuario cuyo DVH debía actualizarse."
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el dígito verificador horizontal del usuario.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el dígito verificador horizontal del usuario.",
                    ex
                );
            }
        }

        public void ActualizarDigitoVerificadorHorizontalComponentePermisos(
            Guid idComponente,
            string valor)
        {
            ValidarIdentificador(idComponente, nameof(idComponente));
            ValidarValorDigito(valor);

            const string sql = @"
                UPDATE dbo.PermisoComponente
                SET DigitoVerificadorHorizontal = @DigitoVerificadorHorizontal
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@Id", SqlDbType.UniqueIdentifier)
                        .Value = idComponente;

                    command.Parameters
                        .Add(
                            "@DigitoVerificadorHorizontal",
                            SqlDbType.NVarChar,
                            256
                        )
                        .Value = valor;

                    connection.Open();

                    VerificarFilaActualizada(
                        command.ExecuteNonQuery(),
                        "No se encontró el componente cuyo DVH debía actualizarse."
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH del componente de permisos.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH del componente de permisos.",
                    ex
                );
            }
        }

        public void ActualizarDigitoVerificadorHorizontalRolComponente(
            Guid idRol,
            Guid idComponenteHijo,
            string valor)
        {
            ValidarIdentificador(idRol, nameof(idRol));
            ValidarIdentificador(idComponenteHijo, nameof(idComponenteHijo));
            ValidarValorDigito(valor);

            const string sql = @"
                UPDATE dbo.RolComponente
                SET DigitoVerificadorHorizontal = @DigitoVerificadorHorizontal
                WHERE RolId = @RolId
                  AND ComponenteHijoId = @ComponenteHijoId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@RolId", SqlDbType.UniqueIdentifier)
                        .Value = idRol;

                    command.Parameters
                        .Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier)
                        .Value = idComponenteHijo;

                    command.Parameters
                        .Add(
                            "@DigitoVerificadorHorizontal",
                            SqlDbType.NVarChar,
                            256
                        )
                        .Value = valor;

                    connection.Open();

                    VerificarFilaActualizada(
                        command.ExecuteNonQuery(),
                        "No se encontró la relación de rol cuyo DVH debía actualizarse."
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH de la relación de rol.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH de la relación de rol.",
                    ex
                );
            }
        }

        public void
            ActualizarDigitoVerificadorHorizontalUsuarioPermisoComponente(
                Guid idUsuario,
                Guid idComponente,
                string valor)
        {
            ValidarIdentificador(idUsuario, nameof(idUsuario));
            ValidarIdentificador(idComponente, nameof(idComponente));
            ValidarValorDigito(valor);

            const string sql = @"
                UPDATE dbo.UsuarioPermisoComponente
                SET DigitoVerificadorHorizontal = @DigitoVerificadorHorizontal
                WHERE UsuarioId = @UsuarioId
                  AND ComponenteId = @ComponenteId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@UsuarioId", SqlDbType.UniqueIdentifier)
                        .Value = idUsuario;

                    command.Parameters
                        .Add("@ComponenteId", SqlDbType.UniqueIdentifier)
                        .Value = idComponente;

                    command.Parameters
                        .Add(
                            "@DigitoVerificadorHorizontal",
                            SqlDbType.NVarChar,
                            256
                        )
                        .Value = valor;

                    connection.Open();

                    VerificarFilaActualizada(
                        command.ExecuteNonQuery(),
                        "No se encontró la asignación cuyo DVH debía actualizarse."
                    );
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH de la asignación de permisos.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el DVH de la asignación de permisos.",
                    ex
                );
            }
        }

        public void GuardarDigitoVerificadorVertical(
            string entidad,
            string valor)
        {
            if (string.IsNullOrWhiteSpace(entidad))
            {
                throw new ArgumentException(
                    "La entidad no puede estar vacía.",
                    nameof(entidad)
                );
            }

            ValidarValorDigito(valor);

            const string sql = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.DigitoVerificadorVertical
                    WHERE Entidad = @Entidad
                )
                BEGIN
                    UPDATE dbo.DigitoVerificadorVertical
                    SET Valor = @Valor,
                        FechaCalculo = SYSDATETIME()
                    WHERE Entidad = @Entidad
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.DigitoVerificadorVertical
                    (
                        Entidad,
                        Valor,
                        FechaCalculo
                    )
                    VALUES
                    (
                        @Entidad,
                        @Valor,
                        SYSDATETIME()
                    )
                END";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@Entidad", SqlDbType.NVarChar, 100)
                        .Value = entidad.Trim();

                    command.Parameters
                        .Add("@Valor", SqlDbType.NVarChar, 256)
                        .Value = valor;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo guardar el dígito verificador vertical.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo abrir o utilizar la conexión con la base de datos.",
                    ex
                );
            }
        }

        public void BloquearUsuariosPorIntegridadExceptoAdmin()
        {
            const string sql = @"
                UPDATE dbo.Usuario
                SET BloqueadoPorIntegridad = 1
                WHERE NombreUsuario <> 'admin';

                UPDATE dbo.Usuario
                SET BloqueadoPorIntegridad = 0
                WHERE NombreUsuario = 'admin';";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron bloquear los usuarios por integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo abrir o utilizar la conexión con la base de datos.",
                    ex
                );
            }
        }

        public void DesbloquearUsuariosPorIntegridad()
        {
            const string sql = @"
                UPDATE dbo.Usuario
                SET BloqueadoPorIntegridad = 0";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudieron desbloquear los usuarios por integridad.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo abrir o utilizar la conexión con la base de datos.",
                    ex
                );
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                NombreUsuario = reader["NombreUsuario"].ToString(),

                NombreCompleto = reader["NombreCompleto"] == DBNull.Value
                    ? null
                    : reader["NombreCompleto"].ToString(),

                Email = reader["Email"] == DBNull.Value
                    ? null
                    : reader["Email"].ToString(),

                IdiomaPreferidoId = reader["IdiomaPreferidoId"] == DBNull.Value
                    ? (Guid?)null
                    : Guid.Parse(reader["IdiomaPreferidoId"].ToString()),

                PasswordHash = reader["PasswordHash"].ToString(),
                Activo = Convert.ToBoolean(reader["Activo"]),
                DebeCambiarPassword =
                    Convert.ToBoolean(reader["DebeCambiarPassword"]),
                IntentosFallidosLogin =
                    Convert.ToInt32(reader["IntentosFallidosLogin"]),

                BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["BloqueadoHasta"]),

                BloqueadoPorIntegridad =
                    Convert.ToBoolean(reader["BloqueadoPorIntegridad"]),

                DigitoVerificadorHorizontal =
                    reader["DigitoVerificadorHorizontal"].ToString(),

                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),

                FechaUltimoAcceso =
                    reader["FechaUltimoAcceso"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["FechaUltimoAcceso"])
            };
        }

        private ComponentePermisos MapearComponentePermisos(
            SqlDataReader reader)
        {
            TipoComponentePermisos tipo =
                (TipoComponentePermisos)Convert.ToInt32(reader["Tipo"]);

            ComponentePermisos componente;

            if (tipo == TipoComponentePermisos.Permiso)
            {
                componente = new Permiso();
            }
            else if (tipo == TipoComponentePermisos.Rol)
            {
                componente = new Rol();
            }
            else
            {
                throw new InvalidOperationException(
                    "Se encontró un tipo de componente de permisos inválido."
                );
            }

            componente.Id = Guid.Parse(reader["Id"].ToString());
            componente.Nombre = reader["Nombre"].ToString();
            componente.Codigo = reader["Codigo"].ToString();

            componente.Descripcion = reader["Descripcion"] == DBNull.Value
                ? null
                : reader["Descripcion"].ToString();

            componente.Activo = Convert.ToBoolean(reader["Activo"]);
            componente.Tipo = tipo;
            componente.FechaCreacion =
                Convert.ToDateTime(reader["FechaCreacion"]);

            componente.FechaModificacion =
                reader["FechaModificacion"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["FechaModificacion"]);

            componente.DigitoVerificadorHorizontal =
                reader["DigitoVerificadorHorizontal"].ToString();

            return componente;
        }

        private RolComponente MapearRolComponente(SqlDataReader reader)
        {
            return new RolComponente
            {
                RolId = Guid.Parse(reader["RolId"].ToString()),
                ComponenteHijoId =
                    Guid.Parse(reader["ComponenteHijoId"].ToString()),
                DigitoVerificadorHorizontal =
                    reader["DigitoVerificadorHorizontal"].ToString()
            };
        }

        private UsuarioPermisoComponente
            MapearUsuarioPermisoComponente(SqlDataReader reader)
        {
            return new UsuarioPermisoComponente
            {
                UsuarioId = Guid.Parse(reader["UsuarioId"].ToString()),
                ComponenteId =
                    Guid.Parse(reader["ComponenteId"].ToString()),
                FechaAsignacion =
                    Convert.ToDateTime(reader["FechaAsignacion"]),

                AsignadoPorUsuarioId =
                    reader["AsignadoPorUsuarioId"] == DBNull.Value
                        ? (Guid?)null
                        : Guid.Parse(
                            reader["AsignadoPorUsuarioId"].ToString()
                        ),

                DigitoVerificadorHorizontal =
                    reader["DigitoVerificadorHorizontal"].ToString()
            };
        }

        private void ValidarIdentificador(Guid identificador, string parametro)
        {
            if (identificador == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador no puede estar vacío.",
                    parametro
                );
            }
        }

        private void ValidarValorDigito(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException(
                    "El valor del dígito verificador no puede estar vacío.",
                    nameof(valor)
                );
            }
        }

        private void VerificarFilaActualizada(
            int filasAfectadas,
            string mensaje)
        {
            if (filasAfectadas == 0)
            {
                throw new InvalidOperationException(mensaje);
            }
        }
    }
}
