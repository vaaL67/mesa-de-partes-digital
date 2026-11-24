using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MesaPartesCentroLima
{
    // Criterios por los que se puede ordenar la lista de expedientes
    public enum CriterioOrden
    {
        Codigo = 0,
        Fecha = 1,
        Solicitante = 2,
        Tramite = 3,
        Estado = 4
    }

    public static class GestorExpedientes
    {
        private static List<RegistroDocumento> lista = new List<RegistroDocumento>();

        public static void Cargar()
        {
            lista = ArchivoExpedientes.Cargar();
        }

        // ------------------------------------------------------------------
        // Generación de códigos
        // ------------------------------------------------------------------

        // Código EXP-AAAA-NNNNNN: el correlativo se reinicia cada año.
        // Toma el mayor número guardado del año indicado y le suma 1.
        //
        // Pseudocódigo:
        //   prefijo <- "EXP-" + año + "-"
        //   mayor <- 0
        //   PARA CADA expediente EN existentes
        //       SI su código tiene 15 caracteres Y empieza con prefijo Y los 6 últimos son un número n Y n > mayor
        //           mayor <- n
        //   RETORNAR prefijo + (mayor + 1) con 6 dígitos
        // Eficiencia: O(n), recorre la lista una sola vez.
        public static string GenerarCodigo(List<RegistroDocumento> existentes, int anio)
        {
            string prefijo = "EXP-" + anio + "-";
            int mayor = 0;
            foreach (RegistroDocumento e in existentes)
            {
                string codigo = e.CodigoExpediente;
                if (codigo != null && codigo.Length == 15 && codigo.StartsWith(prefijo, StringComparison.Ordinal))
                {
                    // "EXP-AAAA-" ocupa 9 caracteres, así que el número empieza en la posición 9.
                    // NumberStyles.None rechaza signos y espacios: solo se aceptan dígitos.
                    int n;
                    if (int.TryParse(codigo.Substring(9, 6), NumberStyles.None, CultureInfo.InvariantCulture, out n) && n > mayor)
                        mayor = n;
                }
            }
            return prefijo + (mayor + 1).ToString("D6", CultureInfo.InvariantCulture);
        }

        public static string GenerarCodigo()
        {
            return GenerarCodigo(lista, DateTime.Now.Year);
        }

        // ------------------------------------------------------------------
        // Registro y cambio de estado
        // ------------------------------------------------------------------

        // Pseudocódigo:
        //   codigo <- GenerarCodigo()
        //   nuevo  <- crear expediente con los datos recibidos, estado "Recibido" y la fecha actual
        //   GUARDAR nuevo en el archivo        (si falla, se detiene aquí)
        //   AGREGAR nuevo a la lista en memoria
        //   RETORNAR nuevo
        public static RegistroDocumento Registrar(string dni, string nombre, string tramite, int folios, string correo)
        {
            string codigo = GenerarCodigo();
            RegistroDocumento nuevo = new RegistroDocumento(codigo, dni, nombre, tramite, folios);
            nuevo.Correo = correo;

            // Primero se guarda en el archivo: si falla y lanza una excepción, el expediente NO llega
            // a la lista. Así la memoria siempre coincide con lo que realmente está guardado en disco.
            ArchivoExpedientes.Anexar(nuevo);
            lista.Add(nuevo);
            return nuevo;
        }

        // Pseudocódigo:
        //   expediente <- buscar por código
        //   SI no existe O la transición de estado no es válida ENTONCES lanzar error
        //   anterior <- estado actual
        //   estado actual <- nuevo estado
        //   REESCRIBIR el archivo con todos los expedientes
        //   SI falla ENTONCES volver al estado anterior y relanzar el error
        public static void CambiarEstado(string codigo, string nuevoEstado)
        {
            RegistroDocumento r = BuscarPorCodigo(codigo);
            if (r == null)
                throw new ArgumentException("No existe el expediente " + codigo);
            if (!Validador.TransicionValida(r.Estado, nuevoEstado))
                throw new InvalidOperationException("No se puede cambiar de «" + r.Estado + "» a «" + nuevoEstado + "».");

            string anterior = r.Estado;
            r.Estado = nuevoEstado;
            try
            {
                ArchivoExpedientes.Reescribir(lista);
            }
            catch
            {
                // La memoria vuelve a coincidir con lo que realmente quedó guardado en disco
                r.Estado = anterior;
                throw;
            }
        }

        // ------------------------------------------------------------------
        // Notificación electrónica (simulada)
        // Ley N.° 31170: las mesas de partes digitales deben notificar electrónicamente.
        // En el prototipo la "notificación" se registra en notificaciones.txt.
        // ------------------------------------------------------------------

        // Línea: fecha|correo|codigo|mensaje
        public static string ConstruirNotificacion(RegistroDocumento r, string motivo, DateTime fecha)
        {
            return fecha.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + "|" + r.Correo + "|" +
                   r.CodigoExpediente + "|Estimado(a) " + r.NombreCompleto + ": " + motivo;
        }

        // Devuelve true si la notificación quedó registrada. Si el expediente no tiene correo o el archivo
        // no se puede escribir devuelve false: el expediente ya está guardado y no se pierde por esto.
        public static bool Notificar(RegistroDocumento r, string motivo)
        {
            if (r == null || string.IsNullOrEmpty(r.Correo)) return false;

            try
            {
                ArchivoExpedientes.AnexarNotificacion(ConstruirNotificacion(r, motivo, DateTime.Now));
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // ------------------------------------------------------------------
        // Consultas
        // ------------------------------------------------------------------

        public static RegistroDocumento[] ObtenerTodos()
        {
            return lista.ToArray();
        }

        // Búsqueda secuencial por código (devuelve null si no existe)
        //   PARA CADA expediente EN datos: SI su código es igual al buscado ENTONCES RETORNAR expediente
        //   RETORNAR null
        // Eficiencia: O(n); el código es único, por eso se detiene en la primera coincidencia.
        public static RegistroDocumento BuscarPorCodigo(List<RegistroDocumento> datos, string codigo)
        {
            foreach (RegistroDocumento e in datos)
            {
                if (e.CodigoExpediente == codigo) return e;
            }
            return null;
        }

        public static RegistroDocumento BuscarPorCodigo(string codigo)
        {
            return BuscarPorCodigo(lista, codigo);
        }

        // Búsqueda secuencial por DNI (un ciudadano puede tener varios expedientes)
        //   encontrados <- lista vacía
        //   PARA CADA expediente EN datos: SI su DNI es igual al buscado ENTONCES agregarlo a encontrados
        //   RETORNAR encontrados
        // Eficiencia: O(n); debe recorrer toda la lista porque puede haber varias coincidencias.
        public static RegistroDocumento[] BuscarPorDni(List<RegistroDocumento> datos, string dni)
        {
            List<RegistroDocumento> encontrados = new List<RegistroDocumento>();
            foreach (RegistroDocumento e in datos)
            {
                if (e.DniCiudadano == dni) encontrados.Add(e);
            }
            return encontrados.ToArray();
        }

        public static RegistroDocumento[] BuscarPorDni(string dni)
        {
            return BuscarPorDni(lista, dni);
        }

        // ------------------------------------------------------------------
        // Ordenamiento
        // ------------------------------------------------------------------

        // Ordenamiento por inserción por el criterio elegido (ascendente o descendente)
        //   PARA i DESDE 1 HASTA longitud - 1
        //       actual <- arreglo[i];  j <- i - 1
        //       MIENTRAS j >= 0 Y Comparar(arreglo[j], actual) > 0
        //           arreglo[j + 1] <- arreglo[j];  j <- j - 1
        //       arreglo[j + 1] <- actual
        //   SI descendente ENTONCES invertir el arreglo
        // Eficiencia: O(n²) en el peor caso y O(n) si ya está ordenado; suficiente para el volumen de
        // una mesa de partes pequeña. Es estable: si dos expedientes empatan en el criterio, conservan
        // su orden de llegada. Como los códigos se generan en orden creciente, ordenar por código o por
        // fecha casi siempre es el mejor caso.
        public static RegistroDocumento[] OrdenarPor(RegistroDocumento[] arreglo, CriterioOrden criterio, bool descendente)
        {
            for (int i = 1; i < arreglo.Length; i++)
            {
                RegistroDocumento actual = arreglo[i];
                int j = i - 1;
                while (j >= 0 && Comparar(arreglo[j], actual, criterio) > 0)
                {
                    arreglo[j + 1] = arreglo[j];
                    j--;
                }
                arreglo[j + 1] = actual;
            }

            if (descendente) Array.Reverse(arreglo);
            return arreglo;
        }

        // Orden ascendente por código. Los códigos tienen ancho fijo (año de 4 dígitos y 6 dígitos con
        // ceros a la izquierda), por eso comparar como texto da el mismo resultado que comparar por año
        // y número.
        public static RegistroDocumento[] OrdenarPorCodigo(RegistroDocumento[] arreglo)
        {
            return OrdenarPor(arreglo, CriterioOrden.Codigo, false);
        }

        // Negativo si a va antes que b, cero si empatan, positivo si a va después de b
        private static int Comparar(RegistroDocumento a, RegistroDocumento b, CriterioOrden criterio)
        {
            switch (criterio)
            {
                case CriterioOrden.Fecha:
                    return a.FechaRegistro.CompareTo(b.FechaRegistro);
                case CriterioOrden.Solicitante:
                    return string.Compare(a.NombreCompleto, b.NombreCompleto, StringComparison.CurrentCultureIgnoreCase);
                case CriterioOrden.Tramite:
                    return string.Compare(a.TipoTramite, b.TipoTramite, StringComparison.CurrentCultureIgnoreCase);
                case CriterioOrden.Estado:
                    // Por posición en el flujo (Recibido, En trámite, Observado, Atendido), no alfabético
                    return Validador.IndiceEstado(a.Estado).CompareTo(Validador.IndiceEstado(b.Estado));
                default:
                    return string.CompareOrdinal(a.CodigoExpediente, b.CodigoExpediente);
            }
        }

        // ------------------------------------------------------------------
        // Resumen con arreglos paralelos: Validador.Estados[i] corresponde a conteos[i]
        // ------------------------------------------------------------------

        // Pseudocódigo:
        //   conteos <- arreglo de ceros del tamaño del catálogo de estados
        //   PARA CADA expediente EN datos
        //       i <- posición de su estado en el catálogo
        //       SI i >= 0 ENTONCES conteos[i] <- conteos[i] + 1
        //   RETORNAR conteos
        public static int[] ContarPorEstado(List<RegistroDocumento> datos)
        {
            int[] conteos = new int[Validador.Estados.Length];
            foreach (RegistroDocumento e in datos)
            {
                int i = Array.IndexOf(Validador.Estados, e.Estado);
                if (i >= 0) conteos[i]++;
            }
            return conteos;
        }

        // Igual que el anterior, pero con el catálogo de trámites: Validador.Tramites[i] <-> conteos[i]
        public static int[] ContarPorTramite(List<RegistroDocumento> datos)
        {
            int[] conteos = new int[Validador.Tramites.Length];
            foreach (RegistroDocumento e in datos)
            {
                int i = Array.IndexOf(Validador.Tramites, e.TipoTramite);
                if (i >= 0) conteos[i]++;
            }
            return conteos;
        }

        public static string ResumenTexto(List<RegistroDocumento> datos)
        {
            int[] porEstado = ContarPorEstado(datos);
            int[] porTramite = ContarPorTramite(datos);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Total de expedientes: " + datos.Count);
            sb.AppendLine();
            sb.AppendLine("Por estado:");
            for (int i = 0; i < Validador.Estados.Length; i++)
            {
                sb.AppendLine("   " + Validador.Estados[i] + ": " + porEstado[i]);
            }
            sb.AppendLine();
            sb.AppendLine("Por tipo de trámite:");
            for (int i = 0; i < Validador.Tramites.Length; i++)
            {
                sb.AppendLine("   " + Validador.Tramites[i] + ": " + porTramite[i]);
            }
            return sb.ToString();
        }

        public static string ResumenTexto()
        {
            return ResumenTexto(lista);
        }
    }
}
