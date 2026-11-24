using System;
using System.IO;
using System.Windows.Forms;

namespace MesaPartesCentroLima
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Si no se puede leer el archivo, el programa se cierra: seguir con la lista vacía haría
            // que se generen códigos repetidos (EXP-AAAA-000001) sobre expedientes ya guardados.
            try
            {
                GestorExpedientes.Cargar();
            }
            catch (IOException ex)
            {
                AvisarErrorDeLectura(ex.Message);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarErrorDeLectura(ex.Message);
                return;
            }

            // Las líneas dañadas se ignoran, pero se avisa para que el usuario revise el archivo.
            if (ArchivoExpedientes.LineasInvalidas > 0)
            {
                MessageBox.Show("Se ignoraron " + ArchivoExpedientes.LineasInvalidas + " línea(s) con formato inválido en expedientes.txt.\n"
                                + "Esas líneas no aparecerán en el sistema y se eliminarán del archivo la próxima vez que se modifique un expediente.\n"
                                + "Si desea conservarlas, haga una copia de expedientes.txt antes de continuar.",
                                "Advertencia al cargar los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            while (true)
            {
                using (FormLogin login = new FormLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK) break;
                }

                using (FormPrincipal principal = new FormPrincipal())
                {
                    principal.ShowDialog();
                    if (!principal.CerrarSesion) break;
                }
            }
        }

        private static void AvisarErrorDeLectura(string detalle)
        {
            MessageBox.Show("No se pudo leer el archivo de expedientes (expedientes.txt).\n"
                            + "Verifique que no esté abierto en otro programa y vuelva a iniciar el sistema.\n\n"
                            + "Detalle: " + detalle,
                            "Error al cargar los datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
