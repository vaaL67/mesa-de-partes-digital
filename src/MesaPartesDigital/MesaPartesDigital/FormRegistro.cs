using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MesaPartesCentroLima
{
    public class FormRegistro : Form
    {
        private TextBox txtDni;
        private TextBox txtNombre;
        private ComboBox cboTramite;
        private NumericUpDown numFolios;
        private TextBox txtCorreo;
        private Label lblUltimo;

        public FormRegistro()
        {
            Text = "Registro de expedientes";
            ClientSize = new Size(480, 365);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblTitulo = new Label();
            lblTitulo.Text = "REGISTRO DE NUEVO EXPEDIENTE";
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.SetBounds(10, 12, 460, 28);

            Label lblDni = new Label();
            lblDni.Text = "DNI (8 dígitos):";
            lblDni.SetBounds(30, 60, 130, 23);
            txtDni = new TextBox();
            txtDni.MaxLength = 8;
            txtDni.SetBounds(170, 57, 270, 23);
            txtDni.KeyPress += SoloDigitos_KeyPress;

            Label lblNombre = new Label();
            lblNombre.Text = "Nombres y apellidos:";
            lblNombre.SetBounds(30, 98, 130, 23);
            txtNombre = new TextBox();
            txtNombre.MaxLength = 80;
            txtNombre.SetBounds(170, 95, 270, 23);

            Label lblTramite = new Label();
            lblTramite.Text = "Tipo de trámite:";
            lblTramite.SetBounds(30, 136, 130, 23);
            cboTramite = new ComboBox();
            cboTramite.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTramite.Items.AddRange(Validador.Tramites);
            cboTramite.SetBounds(170, 133, 270, 23);

            Label lblFolios = new Label();
            lblFolios.Text = "Número de folios:";
            lblFolios.SetBounds(30, 174, 130, 23);
            numFolios = new NumericUpDown();
            numFolios.Minimum = 1;
            numFolios.Maximum = 999;
            numFolios.Value = 1;
            numFolios.SetBounds(170, 171, 100, 23);

            Label lblCorreo = new Label();
            lblCorreo.Text = "Correo electrónico:";
            lblCorreo.SetBounds(30, 212, 130, 23);
            txtCorreo = new TextBox();
            txtCorreo.MaxLength = 80;
            txtCorreo.SetBounds(170, 209, 270, 23);

            Button btnRegistrar = new Button();
            btnRegistrar.Text = "Registrar";
            btnRegistrar.SetBounds(60, 262, 110, 34);
            btnRegistrar.Click += BtnRegistrar_Click;

            Button btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.SetBounds(185, 262, 110, 34);
            btnLimpiar.Click += (s, e) => Limpiar();

            Button btnCerrar = new Button();
            btnCerrar.Text = "Cerrar";
            btnCerrar.SetBounds(310, 262, 110, 34);
            btnCerrar.Click += (s, e) => Close();

            lblUltimo = new Label();
            lblUltimo.Text = "";
            lblUltimo.ForeColor = Color.DarkGreen;
            lblUltimo.TextAlign = ContentAlignment.MiddleCenter;
            lblUltimo.SetBounds(10, 315, 460, 30);

            Controls.Add(lblTitulo);
            Controls.Add(lblDni);
            Controls.Add(txtDni);
            Controls.Add(lblNombre);
            Controls.Add(txtNombre);
            Controls.Add(lblTramite);
            Controls.Add(cboTramite);
            Controls.Add(lblFolios);
            Controls.Add(numFolios);
            Controls.Add(lblCorreo);
            Controls.Add(txtCorreo);
            Controls.Add(btnRegistrar);
            Controls.Add(btnLimpiar);
            Controls.Add(btnCerrar);
            Controls.Add(lblUltimo);

            AcceptButton = btnRegistrar;
        }

        private void SoloDigitos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void Advertir(string mensaje, Control control)
        {
            MessageBox.Show(mensaje, "Dato no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
        }

        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string tramite = cboTramite.SelectedItem as string;
            int folios = (int)numFolios.Value;
            string correo = txtCorreo.Text.Trim();

            if (!Validador.DniValido(dni))
            {
                Advertir("El DNI debe tener exactamente 8 dígitos numéricos.", txtDni);
                return;
            }
            if (!Validador.NombreValido(nombre))
            {
                Advertir("Ingrese nombres y apellidos válidos (solo letras y espacios, mínimo 3 caracteres).", txtNombre);
                return;
            }
            if (!Validador.TramiteValido(tramite))
            {
                Advertir("Seleccione un tipo de trámite de la lista.", cboTramite);
                return;
            }
            if (!Validador.FoliosValidos(folios))
            {
                Advertir("El número de folios debe estar entre 1 y 999.", numFolios);
                return;
            }

            if (!Validador.CorreoValido(correo))
            {
                Advertir("Ingrese un correo electrónico válido (por ejemplo: nombre@correo.com).\nSe usa para enviar la notificación electrónica del expediente.", txtCorreo);
                return;
            }

            // Normaliza el nombre: "  tito   calderon " -> "Tito Calderon"
            nombre = Validador.NormalizarNombre(nombre);
            correo = correo.ToLowerInvariant();

            RegistroDocumento registro;
            try
            {
                registro = GestorExpedientes.Registrar(dni, nombre, tramite, folios, correo);
            }
            catch (IOException ex)
            {
                AvisarErrorDeArchivo(ex.Message);
                return; // no se limpia el formulario: el usuario no pierde lo que escribió
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarErrorDeArchivo(ex.Message);
                return;
            }

            // La notificación electrónica es simulada (se registra en notificaciones.txt)
            bool notificado = GestorExpedientes.Notificar(registro,
                "Su expediente " + registro.CodigoExpediente + " fue recibido por la mesa de partes digital.");

            lblUltimo.Text = "Expediente registrado: " + registro.CodigoExpediente;
            MessageBox.Show("Expediente registrado con código:\n" + registro.CodigoExpediente + "\n\n"
                            + (notificado ? "Notificación electrónica generada para " + registro.Correo + "."
                                          : "No se pudo generar la notificación electrónica."),
                            "Cargo de recepción", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Limpiar();
        }

        private void AvisarErrorDeArchivo(string detalle)
        {
            MessageBox.Show("No se pudo guardar el expediente en el archivo expedientes.txt.\n"
                            + "Verifique que el archivo no esté abierto en otro programa ni protegido contra escritura.\n\n"
                            + "Detalle: " + detalle,
                            "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Limpiar()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtCorreo.Clear();
            cboTramite.SelectedIndex = -1;
            numFolios.Value = 1;
            txtDni.Focus();
        }
    }
}
