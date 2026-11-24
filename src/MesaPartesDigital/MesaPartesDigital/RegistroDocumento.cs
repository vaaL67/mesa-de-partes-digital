using System;
using System.Globalization;

namespace MesaPartesCentroLima
{
    public class RegistroDocumento
    {
        // El orden de estas propiedades es el orden de las columnas en la tabla de gestión
        public string CodigoExpediente { get; set; }
        public string DniCiudadano { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoTramite { get; set; }
        public int NumeroFolios { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Estado { get; set; }
        public string Correo { get; set; }

        public RegistroDocumento(string codigo, string dni, string nombre, string tramite, int folios)
        {
            CodigoExpediente = codigo;
            DniCiudadano = dni;
            NombreCompleto = nombre;
            TipoTramite = tramite;
            NumeroFolios = folios;
            FechaRegistro = DateTime.Now;
            Estado = Validador.EstadoInicial;
            Correo = string.Empty;
        }

        // Constructor usado al cargar desde el archivo (conserva la fecha original)
        public RegistroDocumento(string codigo, string dni, string nombre, string tramite, int folios, DateTime fecha)
            : this(codigo, dni, nombre, tramite, folios)
        {
            FechaRegistro = fecha;
        }

        // Constructor completo, usado al cargar expedientes con estado y correo
        public RegistroDocumento(string codigo, string dni, string nombre, string tramite, int folios, DateTime fecha, string estado, string correo)
            : this(codigo, dni, nombre, tramite, folios, fecha)
        {
            Estado = estado;
            Correo = correo;
        }

        public override string ToString()
        {
            return "Expediente: " + CodigoExpediente + " | DNI: " + DniCiudadano + " | Solicitante: " + NombreCompleto + " | Trámite: " + TipoTramite + " | Folios: " + NumeroFolios + " | Estado: " + Estado;
        }

        // Línea de texto para guardar en el archivo: codigo|dni|nombre|tramite|folios|fecha|estado|correo
        public string ALinea()
        {
            return CodigoExpediente + "|" + DniCiudadano + "|" + NombreCompleto + "|" + TipoTramite + "|" +
                   NumeroFolios + "|" + FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) +
                   "|" + Estado + "|" + Correo;
        }

        // Convierte una línea del archivo en un registro; devuelve null si la línea no es válida.
        // Acepta el formato anterior de 6 campos (sin estado ni correo): quedan como "Recibido" y sin correo.
        //
        // Pseudocódigo:
        //   partes <- dividir la línea por '|'
        //   SI partes no tiene 6 ni 8 elementos ENTONCES RETORNAR null
        //   SI folios no es un entero O la fecha no tiene el formato esperado ENTONCES RETORNAR null
        //   SI son 6 partes ENTONCES RETORNAR registro con estado inicial y sin correo
        //   SI el estado no está en el catálogo O el correo (si existe) no es válido ENTONCES RETORNAR null
        //   RETORNAR registro completo
        public static RegistroDocumento DesdeLinea(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea)) return null;
            string[] partes = linea.Split('|');
            if (partes.Length != 6 && partes.Length != 8) return null;

            int folios;
            DateTime fecha;
            if (!int.TryParse(partes[4], out folios)) return null;
            if (!DateTime.TryParseExact(partes[5], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha)) return null;

            if (partes.Length == 6)
                return new RegistroDocumento(partes[0], partes[1], partes[2], partes[3], folios, fecha);

            string estado = partes[6];
            string correo = partes[7];
            if (!Validador.EstadoValido(estado)) return null;
            if (correo.Length > 0 && !Validador.CorreoValido(correo)) return null;

            return new RegistroDocumento(partes[0], partes[1], partes[2], partes[3], folios, fecha, estado, correo);
        }
    }
}
