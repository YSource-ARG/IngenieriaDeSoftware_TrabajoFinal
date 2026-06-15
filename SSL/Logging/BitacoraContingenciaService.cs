using BE;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SSL.Logging
{
    public class BitacoraContingenciaService : IBitacoraContingenciaService
    {
        private readonly string _rutaArchivo;

        public BitacoraContingenciaService()
        {
            string carpetaLogs = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs"
            );

            if (!Directory.Exists(carpetaLogs))
            {
                Directory.CreateDirectory(carpetaLogs);
            }

            _rutaArchivo = Path.Combine(
                carpetaLogs,
                "bitacora-contingencia.xml"
            );
        }

        public void Guardar(Bitacora bitacora)
        {
            if (bitacora == null)
            {
                throw new ArgumentNullException(nameof(bitacora));
            }

            List<Bitacora> pendientes = LeerPendientes();

            bool yaExiste = pendientes.Any(x => x.Id == bitacora.Id);

            if (!yaExiste)
            {
                pendientes.Add(bitacora);
                GuardarPendientes(pendientes);
            }
        }

        public List<Bitacora> LeerPendientes()
        {
            if (!File.Exists(_rutaArchivo))
            {
                return new List<Bitacora>();
            }

            XmlSerializer serializer =
                new XmlSerializer(typeof(List<Bitacora>));

            using (FileStream stream = new FileStream(_rutaArchivo, FileMode.Open))
            {
                return (List<Bitacora>)serializer.Deserialize(stream);
            }
        }

        public void EliminarPendiente(Guid id)
        {
            List<Bitacora> pendientes = LeerPendientes();

            pendientes = pendientes
                .Where(x => x.Id != id)
                .ToList();

            GuardarPendientes(pendientes);
        }

        private void GuardarPendientes(List<Bitacora> pendientes)
        {
            XmlSerializer serializer =
                new XmlSerializer(typeof(List<Bitacora>));

            using (FileStream stream = new FileStream(_rutaArchivo, FileMode.Create))
            {
                serializer.Serialize(stream, pendientes);
            }
        }
    }
}