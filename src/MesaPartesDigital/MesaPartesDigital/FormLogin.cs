using System;
using System.Drawing;
using System.Windows.Forms;

namespace MesaPartesCentroLima
{
    public class FormLogin : Form
    {
        // Credenciales fijas solo para el prototipo académico
        private const string UsuarioValido = "admin";
        private const string ClaveValida = "1234";

        private TextBox txtUsuario;
        private TextBox txtClave;
        private Button btnIngresar;
        private Button btnSalir;
        private int intentos = 0;

        public FormLogin()
        {
            Text = "Mesa de Partes Digital - Inicio de sesión";
            ClientSize = new Size(360, 230);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblTitulo = new Label();
            lblTitulo.Text = "MESA DE PARTES DIGITAL";
            lblTitulo.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.SetBounds(10, 15, 340, 30);

            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.SetBounds(40, 70, 80, 23);

            txtUsuario = new TextBox();
            txtUsuario.SetBounds(130, 67, 190, 23);

            Label lblClave = new Label();
            lblClave.Text = "Contraseña:";
            lblClave.SetBounds(40, 105, 80, 23);

            txtClave = new TextBox();
            txtClave.UseSystemPasswordChar = true;
            txtClave.SetBounds(130, 102, 190, 23);

            btnIngresar = new Button();
            btnIngresar.Text = "Ingresar";
            btnIngresar.SetBounds(60, 160, 110, 32);
            btnIngresar.Click += BtnIngresar_Click;

            btnSalir = new Button();
            btnSalir.Text = "Salir";
            btnSalir.SetBounds(190, 160, 110, 32);
            btnSalir.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.Add(lblTitulo);
            Controls.Add(lblUsuario);
            Controls.Add(txtUsuario);
            Controls.Add(lblClave);
            Controls.Add(txtClave);
            Controls.Add(btnIngresar);
            Controls.Add(btnSalir);

            AcceptButton = btnIngresar;
            CancelButton = btnSalir;
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;

            if (usuario.Length == 0 || clave.Length == 0)
            {
                MessageBox.Show("Ingrese el usuario y la contraseña.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (usuario == UsuarioValido && clave == ClaveValida)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            intentos++;
            if (intentos >= 3)
            {
                MessageBox.Show("Se superó el número de intentos permitidos. El sistema se cerrará.", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            MessageBox.Show("Usuario o contraseña incorrectos. Intento " + intentos + " de 3.", "Credenciales incorrectas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtClave.Clear();
            txtClave.Focus();
        }
    }
}
