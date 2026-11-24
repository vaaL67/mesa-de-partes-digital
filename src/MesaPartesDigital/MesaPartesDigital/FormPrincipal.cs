using System;
using System.Drawing;
using System.Windows.Forms;

namespace MesaPartesCentroLima
{
    public class FormPrincipal : Form
    {
        // true si el usuario eligió "Cerrar sesión" (vuelve al login); false si eligió "Salir"
        public bool CerrarSesion { get; private set; }

        public FormPrincipal()
        {
            Text = "Mesa de Partes Digital - Menú principal";
            ClientSize = new Size(520, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem mExpedientes = new ToolStripMenuItem("Expedientes");
            ToolStripMenuItem mRegistro = new ToolStripMenuItem("Registro de expedientes");
            mRegistro.Click += (s, e) => AbrirRegistro();
            ToolStripMenuItem mGestion = new ToolStripMenuItem("Gestión de expedientes");
            mGestion.Click += (s, e) => AbrirGestion();
            mExpedientes.DropDownItems.Add(mRegistro);
            mExpedientes.DropDownItems.Add(mGestion);

            ToolStripMenuItem mSesion = new ToolStripMenuItem("Sesión");
            ToolStripMenuItem mCerrar = new ToolStripMenuItem("Cerrar sesión");
            mCerrar.Click += (s, e) => { CerrarSesion = true; Close(); };
            ToolStripMenuItem mSalir = new ToolStripMenuItem("Salir");
            mSalir.Click += (s, e) => { CerrarSesion = false; Close(); };
            mSesion.DropDownItems.Add(mCerrar);
            mSesion.DropDownItems.Add(mSalir);

            menu.Items.Add(mExpedientes);
            menu.Items.Add(mSesion);
            MainMenuStrip = menu;
            Controls.Add(menu);

            Label lblTitulo = new Label();
            lblTitulo.Text = "SISTEMA DE MESA DE PARTES";
            lblTitulo.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            lblTitulo.SetBounds(10, 50, 500, 36);

            Label lblSub = new Label();
            lblSub.Text = "Bienvenido. Seleccione una opción:";
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            lblSub.SetBounds(10, 90, 500, 24);

            Button btnRegistro = new Button();
            btnRegistro.Text = "Registro de expedientes";
            btnRegistro.SetBounds(140, 135, 240, 40);
            btnRegistro.Click += (s, e) => AbrirRegistro();

            Button btnGestion = new Button();
            btnGestion.Text = "Gestión de expedientes";
            btnGestion.SetBounds(140, 185, 240, 40);
            btnGestion.Click += (s, e) => AbrirGestion();

            Button btnCerrarSesion = new Button();
            btnCerrarSesion.Text = "Cerrar sesión";
            btnCerrarSesion.SetBounds(140, 240, 240, 32);
            btnCerrarSesion.Click += (s, e) => { CerrarSesion = true; Close(); };

            Controls.Add(lblTitulo);
            Controls.Add(lblSub);
            Controls.Add(btnRegistro);
            Controls.Add(btnGestion);
            Controls.Add(btnCerrarSesion);
        }

        private void AbrirRegistro()
        {
            using (FormRegistro f = new FormRegistro())
            {
                f.ShowDialog(this);
            }
        }

        private void AbrirGestion()
        {
            using (FormGestion f = new FormGestion())
            {
                f.ShowDialog(this);
            }
        }
    }
}
