using System;
using System.Collections.Generic;

namespace MesaPartesCentroLima
{
    class Program
    {
        private static List<RegistroDocumento> listaExpedientes = new List<RegistroDocumento>();
        private static int contadorCorrelativo = 1;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool sistemaActivo = true;

            while (sistemaActivo)
            {
                Console.Clear();
                Console.WriteLine("=========================================================================");
                Console.WriteLine("          SISTEMA DE MESA DE PARTES - CENTRO DE LIMA                     ");
                Console.WriteLine("=========================================================================");
                Console.WriteLine(" 1. Registrar nuevo expediente");
                Console.WriteLine(" 2. Mostrar todos los expedientes");
                Console.WriteLine(" 3. Buscar expediente por DNI");
                Console.WriteLine(" 4. Salir");
                Console.WriteLine("=========================================================================");
                Console.Write(" Seleccione una opción (1-4): ");

                string opcion = Console.ReadLine();

                if (opcion == "1")
                {
                    RegistrarNuevoExpediente();
                }
                else if (opcion == "2")
                {
                    MostrarListaExpedientes();
                }
                else if (opcion == "3")
                {
                    BuscarPorDni();
                }
                else if (opcion == "4")
                {
                    sistemaActivo = false;
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Opción no válida. Presione Enter para continuar.");
                    Console.ReadLine();
                }
            }
        }

        static void RegistrarNuevoExpediente()
        {
            Console.Clear();
            Console.WriteLine("--- REGISTRO DE NUEVO EXPEDIENTE ---");

            Console.Write("\nIngrese DNI del Ciudadano (8 dígitos): ");
            string dni = Console.ReadLine();
            if (string.IsNullOrEmpty(dni)) dni = "00000000";

            Console.Write("Ingrese Nombres y Apellidos del Solicitante: ");
            string nombre = Console.ReadLine();
            if (string.IsNullOrEmpty(nombre)) nombre = "Ciudadano";

            Console.Write("Ingrese Tipo de Trámite (ej. Licencia, Reclamo, Carta): ");
            string tramite = Console.ReadLine();
            if (string.IsNullOrEmpty(tramite)) tramite = "Solicitud";

            Console.Write("Ingrese cantidad de folios: ");
            int folios = 1;
            int.TryParse(Console.ReadLine(), out folios);

            string codigoExpediente = "EXP-2026-00" + contadorCorrelativo;
            contadorCorrelativo++;

            RegistroDocumento nuevoDoc = new RegistroDocumento(codigoExpediente, dni, nombre, tramite, folios);
            listaExpedientes.Add(nuevoDoc);

            Console.WriteLine("\n[ÉXITO] Expediente registrado con código: " + codigoExpediente);
            Console.WriteLine("\nPresione Enter para volver al menú...");
            Console.ReadLine();
        }

        static void MostrarListaExpedientes()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE EXPEDIENTES ---");

            if (listaExpedientes.Count == 0)
            {
                Console.WriteLine("\nNo hay ningún expediente registrado.");
            }
            else
            {
                foreach (var exp in listaExpedientes)
                {
                    Console.WriteLine(exp.ToString());
                    Console.WriteLine("------------------------------------------------------------------");
                }
            }

            Console.WriteLine("\nPresione Enter para volver al menú...");
            Console.ReadLine();
        }

        static void BuscarPorDni()
        {
            Console.Clear();
            Console.WriteLine("--- BÚSQUEDA POR DNI ---");
            Console.Write("\nIngrese DNI a buscar: ");
            string dniBuscar = Console.ReadLine();

            bool encontrado = false;
            foreach (var item in listaExpedientes)
            {
                if (item.DniCiudadano == dniBuscar)
                {
                    Console.WriteLine(item.ToString());
                    encontrado = true;
                }
            }

            if (!encontrado)
            {
                Console.WriteLine("\nNo se encontraron registros con ese DNI.");
            }

            Console.WriteLine("\nPresione Enter para volver al menú...");
            Console.ReadLine();
        }
    }
}
