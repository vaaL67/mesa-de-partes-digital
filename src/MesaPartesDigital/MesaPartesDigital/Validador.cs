using System;
using System.Globalization;

namespace MesaPartesCentroLima
{
    public static class Validador
    {
        public static readonly string[] Tramites = { "Licencia", "Petición", "Reclamo", "Solicitud", "Oficio" };

        // Estados del expediente, en el orden del flujo de atención
        public static readonly string[] Estados = { "Recibido", "En trámite", "Observado", "Atendido" };
        public const string EstadoInicial = "Recibido";
        public const string EstadoFinal = "Atendido";

        // DNI: exactamente 8 caracteres y todos dígitos
        public static bool DniValido(string dni)
        {
            if (dni == null || dni.Length != 8) return false;
            foreach (char c in dni)
            {
                if (c < '0' || c > '9') return false;
            }
            return true;
        }

        // Nombre: entre 3 y 80 caracteres, solo letras y espacios
        public static bool NombreValido(string nombre)
        {
            if (nombre == null) return false;
            nombre = nombre.Trim();
            if (nombre.Length < 3 || nombre.Length > 80) return false;
            foreach (char c in nombre)
            {
                if (!char.IsLetter(c) && c != ' ') return false;
            }
            return true;
        }

        // Normaliza un nombre ya validado: quita espacios repetidos y deja cada palabra con inicial mayúscula
        // Ejemplo: "  tito   calderon " -> "Tito Calderon"
        public static string NormalizarNombre(string nombre)
        {
            if (nombre == null) return string.Empty;

            string[] palabras = nombre.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < palabras.Length; i++)
            {
                string palabra = palabras[i].ToLower(CultureInfo.InvariantCulture);
                palabras[i] = char.ToUpper(palabra[0], CultureInfo.InvariantCulture) + palabra.Substring(1);
            }
            return string.Join(" ", palabras);
        }

        // Trámite: debe pertenecer al catálogo
        public static bool TramiteValido(string tramite)
        {
            if (tramite == null) return false;
            foreach (string t in Tramites)
            {
                if (t == tramite) return true;
            }
            return false;
        }

        // Folios: entero entre 1 y 999
        public static bool FoliosValidos(int folios)
        {
            return folios >= 1 && folios <= 999;
        }

        // Código de expediente: formato EXP-AAAA-NNNNNN (15 caracteres), por ejemplo EXP-2026-000001
        public static bool CodigoValido(string codigo)
        {
            if (codigo == null || codigo.Length != 15) return false;
            if (!codigo.StartsWith("EXP-", StringComparison.Ordinal)) return false;
            if (codigo[8] != '-') return false;

            for (int i = 4; i < codigo.Length; i++)
            {
                if (i == 8) continue; // posición del guion que separa año y número
                if (codigo[i] < '0' || codigo[i] > '9') return false;
            }
            return true;
        }

        // Correo electrónico (para la notificación electrónica): entre 6 y 80 caracteres, sin espacios
        // ni '|', exactamente una '@' con texto antes, y un dominio con un punto y una terminación de
        // al menos 2 letras. Ejemplos válidos: ana@correo.com, ana.perez@upn.edu.pe
        //
        // Pseudocódigo:
        //   SI correo es nulo O su longitud no está entre 6 y 80 ENTONCES RETORNAR falso
        //   PARA CADA carácter: SI es espacio o '|' ENTONCES RETORNAR falso
        //   arroba <- posición de la primera '@'
        //   SI no hay '@', está al inicio o hay más de una ENTONCES RETORNAR falso
        //   dominio <- texto después de la '@'
        //   punto <- posición del último '.' del dominio
        //   SI no hay punto, está al inicio, o la terminación tiene menos de 2 caracteres ENTONCES RETORNAR falso
        //   SI el dominio contiene ".." ENTONCES RETORNAR falso
        //   RETORNAR verdadero
        public static bool CorreoValido(string correo)
        {
            if (correo == null || correo.Length < 6 || correo.Length > 80) return false;

            foreach (char c in correo)
            {
                if (char.IsWhiteSpace(c) || c == '|') return false;
            }

            int arroba = correo.IndexOf('@');
            if (arroba <= 0 || arroba != correo.LastIndexOf('@')) return false;

            string dominio = correo.Substring(arroba + 1);
            int punto = dominio.LastIndexOf('.');
            if (punto < 1 || dominio.Length - punto - 1 < 2) return false;
            if (dominio.Contains("..")) return false;

            return true;
        }

        // Estado: debe pertenecer al catálogo
        public static bool EstadoValido(string estado)
        {
            return IndiceEstado(estado) >= 0;
        }

        // Posición del estado en el flujo de atención (-1 si no existe)
        public static int IndiceEstado(string estado)
        {
            return Array.IndexOf(Estados, estado);
        }

        // Un expediente Atendido ya no cambia de estado, y no tiene sentido "cambiar" al mismo estado
        public static bool TransicionValida(string actual, string nuevo)
        {
            if (!EstadoValido(nuevo)) return false;
            if (actual == EstadoFinal) return false;
            return actual != nuevo;
        }
    }
}
