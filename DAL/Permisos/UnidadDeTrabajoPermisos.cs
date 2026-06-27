using BE.Permisos;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Permisos
{
    /// <summary>
    /// Persiste operaciones compuestas de roles y permisos dentro de una única
    /// transacción. Si una instrucción falla, se revierte la operación completa.
    /// </summary>
    public class UnidadDeTrabajoPermisos : IUnidadDeTrabajoPermisos
    {
        private readonly IConnectionFactory _connectionFactory;

        public UnidadDeTrabajoPermisos(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory
                ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        public void CrearRolConComponentes(
            Rol rol,
            IReadOnlyCollection<Guid> idsComponentesHijos)
        {
            if (rol == null)
            {
                throw new ArgumentNullException(nameof(rol));
            }

            ValidarColeccion(idsComponentesHijos, nameof(idsComponentesHijos));

            EjecutarEnTransaccion(
                "No se pudo crear el rol junto con su composición.",
                (connection, transaction) =>
                {
                    InsertarRol(connection, transaction, rol);
                    InsertarComponentesDeRol(
                        connection,
                        transaction,
                        rol.Id,
                        idsComponentesHijos
                    );
                }
            );
        }

        public void ReemplazarComponentesDeRol(
            Guid idRol,
            IReadOnlyCollection<Guid> idsComponentesHijos)
        {
            ValidarId(idRol, nameof(idRol));
            ValidarColeccion(idsComponentesHijos, nameof(idsComponentesHijos));

            EjecutarEnTransaccion(
                "No se pudo reemplazar la composición del rol.",
                (connection, transaction) =>
                {
                    EliminarComponentesDeRol(connection, transaction, idRol);
                    InsertarComponentesDeRol(
                        connection,
                        transaction,
                        idRol,
                        idsComponentesHijos
                    );
                }
            );
        }

        public void ReemplazarAsignacionesDeUsuario(
            Guid idUsuario,
            IReadOnlyCollection<Guid> idsComponentes,
            Guid? asignadoPorUsuarioId)
        {
            ValidarId(idUsuario, nameof(idUsuario));
            ValidarColeccion(idsComponentes, nameof(idsComponentes));

            EjecutarEnTransaccion(
                "No se pudieron reemplazar las asignaciones de permisos del usuario.",
                (connection, transaction) =>
                {
                    EliminarAsignacionesDeUsuario(connection, transaction, idUsuario);
                    InsertarAsignacionesDeUsuario(
                        connection,
                        transaction,
                        idUsuario,
                        idsComponentes,
                        asignadoPorUsuarioId
                    );
                }
            );
        }

        public void EliminarComponente(Guid idComponente)
        {
            ValidarId(idComponente, nameof(idComponente));

            EjecutarEnTransaccion(
                "No se pudo eliminar completamente el componente de permisos.",
                (connection, transaction) =>
                {
                    EliminarRelacionesDelComponente(
                        connection,
                        transaction,
                        idComponente
                    );

                    EliminarAsignacionesDelComponente(
                        connection,
                        transaction,
                        idComponente
                    );

                    EliminarComponentePermisos(
                        connection,
                        transaction,
                        idComponente
                    );
                }
            );
        }

        private void EjecutarEnTransaccion(
            string mensajeError,
            Action<SqlConnection, SqlTransaction> operacion)
        {
            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            operacion(connection, transaction);
                            transaction.Commit();
                        }
                        catch
                        {
                            IntentarRevertir(transaction);
                            throw;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(mensajeError, ex);
            }
        }

        private void InsertarRol(
            SqlConnection connection,
            SqlTransaction transaction,
            Rol rol)
        {
            const string sql = @"
                INSERT INTO dbo.PermisoComponente
                (
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                )
                VALUES
                (
                    @Id,
                    @Nombre,
                    @Codigo,
                    @Descripcion,
                    @Activo,
                    @Tipo,
                    @FechaCreacion,
                    @FechaModificacion
                )";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = rol.Id;
                command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = rol.Nombre.Trim();
                command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 100).Value = rol.Codigo.Trim();
                command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value =
                    string.IsNullOrWhiteSpace(rol.Descripcion)
                        ? (object)DBNull.Value
                        : rol.Descripcion.Trim();
                command.Parameters.Add("@Activo", SqlDbType.Bit).Value = rol.Activo;
                command.Parameters.Add("@Tipo", SqlDbType.Int).Value = (int)TipoComponentePermisos.Rol;
                command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = DateTime.Now;
                command.Parameters.Add("@FechaModificacion", SqlDbType.DateTime2).Value = DateTime.Now;

                command.ExecuteNonQuery();
            }
        }

        private void InsertarComponentesDeRol(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idRol,
            IEnumerable<Guid> idsComponentesHijos)
        {
            const string sql = @"
                INSERT INTO dbo.RolComponente
                (
                    RolId,
                    ComponenteHijoId
                )
                VALUES
                (
                    @RolId,
                    @ComponenteHijoId
                )";

            foreach (Guid idComponenteHijo in idsComponentesHijos)
            {
                using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponenteHijo;
                    command.ExecuteNonQuery();
                }
            }
        }

        private void EliminarComponentesDeRol(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idRol)
        {
            const string sql = @"
                DELETE FROM dbo.RolComponente
                WHERE RolId = @RolId";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;
                command.ExecuteNonQuery();
            }
        }

        private void EliminarAsignacionesDeUsuario(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idUsuario)
        {
            const string sql = @"
                DELETE FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;
                command.ExecuteNonQuery();
            }
        }

        private void InsertarAsignacionesDeUsuario(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idUsuario,
            IEnumerable<Guid> idsComponentes,
            Guid? asignadoPorUsuarioId)
        {
            const string sql = @"
                INSERT INTO dbo.UsuarioPermisoComponente
                (
                    UsuarioId,
                    ComponenteId,
                    FechaAsignacion,
                    AsignadoPorUsuarioId
                )
                VALUES
                (
                    @UsuarioId,
                    @ComponenteId,
                    @FechaAsignacion,
                    @AsignadoPorUsuarioId
                )";

            DateTime fechaAsignacion = DateTime.Now;

            foreach (Guid idComponente in idsComponentes)
            {
                using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;
                    command.Parameters.Add("@FechaAsignacion", SqlDbType.DateTime2).Value = fechaAsignacion;
                    command.Parameters.Add("@AsignadoPorUsuarioId", SqlDbType.UniqueIdentifier).Value =
                        asignadoPorUsuarioId.HasValue
                            ? (object)asignadoPorUsuarioId.Value
                            : DBNull.Value;

                    command.ExecuteNonQuery();
                }
            }
        }

        private void EliminarRelacionesDelComponente(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idComponente)
        {
            const string sql = @"
                DELETE FROM dbo.RolComponente
                WHERE RolId = @IdComponente
                   OR ComponenteHijoId = @IdComponente";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@IdComponente", SqlDbType.UniqueIdentifier).Value = idComponente;
                command.ExecuteNonQuery();
            }
        }

        private void EliminarAsignacionesDelComponente(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idComponente)
        {
            const string sql = @"
                DELETE FROM dbo.UsuarioPermisoComponente
                WHERE ComponenteId = @IdComponente";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@IdComponente", SqlDbType.UniqueIdentifier).Value = idComponente;
                command.ExecuteNonQuery();
            }
        }

        private void EliminarComponentePermisos(
            SqlConnection connection,
            SqlTransaction transaction,
            Guid idComponente)
        {
            const string sql = @"
                DELETE FROM dbo.PermisoComponente
                WHERE Id = @IdComponente";

            using (SqlCommand command = new SqlCommand(sql, connection, transaction))
            {
                command.Parameters.Add("@IdComponente", SqlDbType.UniqueIdentifier).Value = idComponente;
                command.ExecuteNonQuery();
            }
        }

        private void IntentarRevertir(SqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
                // Se conserva la excepción original, que explica la causa del fallo.
            }
        }

        private void ValidarId(Guid id, string nombreParametro)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador no puede estar vacío.",
                    nombreParametro
                );
            }
        }

        private void ValidarColeccion<T>(
            IReadOnlyCollection<T> elementos,
            string nombreParametro)
        {
            if (elementos == null)
            {
                throw new ArgumentNullException(nombreParametro);
            }
        }
    }
}
