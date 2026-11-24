using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MesaPartesCentroLima
{
    public static class ArchivoExpedientes
    {
        // Los archivos se guardan junto al ejecutable (bin\Debug\net8.0-windows\)
        public static readonly string Ruta = Path.Combine(AppContext.BaseDirectory, "expedientes.txt");
        public static readonly string RutaRespaldo = Path.Combine(AppContext.BaseDirectory, "expedientes.bak");
        public static readonly string RutaNotificaciones = Path.Combine(AppContext.BaseDirectory, "notificaciones.txt");

        // Cantidad de líneas no vacías que no se pudieron interpretar en la última carga
        public static int LineasInvalidas { get; private set; }

        // Pseudocódigo:
        //   lista <- lista vacía;  invalidas <- 0
        //   SI el archivo no existe ENTONCES RETORNAR lista
        //   PARA CADA línea EN el archivo
        //       SI la línea está vacía ENTONCES continuar
        //       registro <- convertir la línea
        //       SI registro es válido ENTONCES agregarlo a lista SINO invalidas <- invalidas + 1
        //   RETORNAR lista
        public static List<RegistroDocumento> Cargar()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            LineasInvalidas = 0;
            if (!File.Exists(Ruta)) return lista;

            string[] lineas = File.ReadAllLines(Ruta, Encoding.UTF8);
            foreach (string linea in lineas)
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                RegistroDocumento r = RegistroDocumento.DesdeLinea(linea);
                if (r != null) lista.Add(r);
                else LineasInvalidas++;
            }
            return lista;
        }

        // Agrega un expediente nuevo al final del archivo
        public static void Anexar(RegistroDocumento registro)
        {
            AnexarLinea(Ruta, registro.ALinea());
        }

        // Agrega una notificación electrónica (simulada) al registro de notificaciones
        public static void AnexarNotificacion(string linea)
        {
            AnexarLinea(RutaNotificaciones, linea);
        }

        // Pseudocódigo:
        //   texto <- línea + salto de línea
        //   SI el archivo ya tiene contenido Y su último carácter NO es un salto de línea
        //       texto <- salto de línea + texto      (evita pegar el registro a la línea anterior)
        //   AGREGAR texto al final del archivo
        private static void AnexarLinea(string ruta, string linea)
        {
            string texto = linea + Environment.NewLine;
            if (!TerminaEnSaltoDeLinea(ruta)) texto = Environment.NewLine + texto;
            File.AppendAllText(ruta, texto, Encoding.UTF8);
        }

        // Guarda TODOS los expedientes (se usa al cambiar el estado de uno).
        //
        // Pseudocódigo:
        //   escribir todas las líneas en un archivo temporal       (si falla, el original queda intacto)
        //   copiar el archivo actual a expedientes.bak             (respaldo)
        //   reemplazar el archivo actual por el temporal
        public static void Reescribir(List<RegistroDocumento> registros)
        {
            string temporal = Ruta + ".tmp";

            string[] lineas = new string[registros.Count];
            for (int i = 0; i < registros.Count; i++)
            {
                lineas[i] = registros[i].ALinea();
            }
            File.WriteAllLines(temporal, lineas, Encoding.UTF8);

            if (File.Exists(Ruta)) File.Copy(Ruta, RutaRespaldo, true);
            File.Move(temporal, Ruta, true);
        }

        // true si el archivo no existe, está vacío o su último byte es un salto de línea (\n)
        private static bool TerminaEnSaltoDeLinea(string ruta)
        {
            if (!File.Exists(ruta)) return true;

            using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length == 0) return true;
                fs.Seek(-1, SeekOrigin.End);
                return fs.ReadByte() == '\n';
            }
        }
    }
}
