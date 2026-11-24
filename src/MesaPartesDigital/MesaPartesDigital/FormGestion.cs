using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MesaPartesCentroLima
{
    public class FormGestion : Form
    {
        private ComboBox cboCriterio;
        private TextBox txtBuscar;
        private ComboBox cboOrden;
        private CheckBox chkDescendente;
        private ComboBox cboEstado;
        private DataGridView dgv;
        private Label lblResultados;

        // Expedientes que se están mostrando ahora en la tabla (todos, o el resultado de una búsqueda)
        private RegistroDocumento[] mostrados = new RegistroDocumento[0];

        public FormGestion()
        {
            Text = "Gestión de expedientes";
            ClientSize = new Size(860, 500);
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(880, 480);

            // ---------- Fila 1: búsqueda y resumen ----------
            Label lblBuscar = new Label();
            lblBuscar.Text = "Buscar por:";
            lblBuscar.SetBounds(15, 18, 75, 23);

            cboCriterio = new ComboBox();
            cboCriterio.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCriterio.Items.Add("Código");
            cboCriterio.Items.Add("DNI");
            cboCriterio.SelectedIndex = 0;
            cboCriterio.SetBounds(95, 15, 90, 23);

            txtBuscar = new TextBox();
            txtBuscar.SetBounds(195, 15, 200, 23);

            Button btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.SetBounds(405, 12, 90, 30);
            btnBuscar.Click += BtnBuscar_Click;

            Button btnTodos = new Button();
            btnTodos.Text = "Mostrar todos";
            btnTodos.SetBounds(505, 12, 110, 30);
            btnTodos.Click += (s, e) => MostrarTodos();

            Button btnResumen = new Button();
            btnResumen.Text = "Resumen";
            btnResumen.SetBounds(625, 12, 110, 30);
            btnResumen.Click += (s, e) => MessageBox.Show(GestorExpedientes.ResumenTexto(), "Resumen de expedientes", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // ---------- Fila 2: ordenar y cambiar estado ----------
            Label lblOrden = new Label();
            lblOrden.Text = "Ordenar por:";
            lblOrden.SetBounds(15, 63, 80, 23);

            cboOrden = new ComboBox();
            cboOrden.DropDownStyle = ComboBoxStyle.DropDownList;
            // El orden de esta lista coincide con los valores de CriterioOrden
            cboOrden.Items.AddRange(new object[] { "Código", "Fecha", "Solicitante", "Trámite", "Estado" });
            cboOrden.SelectedIndex = 0;
            cboOrden.SetBounds(95, 60, 110, 23);

            chkDescendente = new CheckBox();
            chkDescendente.Text = "Descendente";
            chkDescendente.SetBounds(215, 61, 105, 23);

            Button btnOrdenar = new Button();
            btnOrdenar.Text = "Ordenar";
            btnOrdenar.SetBounds(325, 57, 90, 30);
            btnOrdenar.Click += BtnOrdenar_Click;

            Label lblEstado = new Label();
            lblEstado.Text = "Nuevo estado:";
            lblEstado.SetBounds(445, 63, 85, 23);

            cboEstado = new ComboBox();
            cboEstado.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEstado.Items.AddRange(Validador.Estados);
            cboEstado.SetBounds(535, 60, 120, 23);

            Button btnCambiarEstado = new Button();
            btnCambiarEstado.Text = "Cambiar estado";
            btnCambiarEstado.SetBounds(665, 57, 130, 30);
            btnCambiarEstado.Click += BtnCambiarEstado_Click;

            // ---------- Tabla ----------
            dgv = new DataGridView();
            dgv.SetBounds(15, 100, 830, 350);
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;

            lblResultados = new Label();
            lblResultados.SetBounds(15, 462, 600, 23);
            lblResultados.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            Button btnCerrar = new Button();
            btnCerrar.Text = "Cerrar";
            btnCerrar.SetBounds(745, 458, 100, 30);
            btnCerrar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCerrar.Click += (s, e) => Close();

            Controls.Add(lblBuscar);
            Controls.Add(cboCriterio);
            Controls.Add(txtBuscar);
            Controls.Add(btnBuscar);
            Controls.Add(btnTodos);
            Controls.Add(btnResumen);
            Controls.Add(lblOrden);
            Controls.Add(cboOrden);
            Controls.Add(chkDescendente);
            Controls.Add(btnOrdenar);
            Controls.Add(lblEstado);
            Controls.Add(cboEstado);
            Controls.Add(btnCambiarEstado);
            Controls.Add(dgv);
            Controls.Add(lblResultados);
            Controls.Add(btnCerrar);

            AcceptButton = btnBuscar;
            Load += (s, e) => MostrarTodos();
        }

        private void MostrarTodos()
        {
            Mostrar(GestorExpedientes.ObtenerTodos());
        }

        private void Mostrar(RegistroDocumento[] datos)
        {
            mostrados = datos;

            dgv.DataSource = null;
            dgv.DataSource = datos;

            RenombrarColumna("CodigoExpediente", "Código");
            RenombrarColumna("DniCiudadano", "DNI");
            RenombrarColumna("NombreCompleto", "Solicitante");
            RenombrarColumna("TipoTramite", "Trámite");
            RenombrarColumna("NumeroFolios", "Folios");
            RenombrarColumna("FechaRegistro", "Fecha de registro");
            RenombrarColumna("Estado", "Estado");
            RenombrarColumna("Correo", "Correo electrónico");
            if (dgv.Columns.Contains("FechaRegistro"))
                dgv.Columns["FechaRegistro"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";

            lblResultados.Text = datos.Length + " expediente(s) mostrado(s)";
        }

        private void RenombrarColumna(string nombre, string encabezado)
        {
            if (dgv.Columns.Contains(nombre)) dgv.Columns[nombre].HeaderText = encabezado;
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string texto = txtBuscar.Text.Trim();
            if (texto.Length == 0)
            {
                MessageBox.Show("Ingrese el dato a buscar.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBuscar.Focus();
                return;
            }

            RegistroDocumento[] resultado;

            if (cboCriterio.SelectedIndex == 0)
            {
                string codigo = texto.ToUpperInvariant();
                if (!Validador.CodigoValido(codigo))
                {
                    MessageBox.Show("El código debe tener el formato EXP-AAAA-NNNNNN (por ejemplo: EXP-2026-000001).", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBuscar.Focus();
                    return;
                }
                RegistroDocumento r = GestorExpedientes.BuscarPorCodigo(codigo);
                resultado = (r == null) ? new RegistroDocumento[0] : new RegistroDocumento[] { r };
            }
            else
            {
                if (!Validador.DniValido(texto))
                {
                    MessageBox.Show("El DNI debe tener exactamente 8 dígitos numéricos.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBuscar.Focus();
                    return;
                }
                resultado = GestorExpedientes.BuscarPorDni(texto);
            }

            Mostrar(resultado);

            if (resultado.Length == 0)
            {
                MessageBox.Show("No se encontraron expedientes con ese dato.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Ordena lo que se está mostrando (todos o el resultado de una búsqueda)
        private void BtnOrdenar_Click(object sender, EventArgs e)
        {
            RegistroDocumento[] copia = (RegistroDocumento[])mostrados.Clone();
            CriterioOrden criterio = (CriterioOrden)cboOrden.SelectedIndex;
            Mostrar(GestorExpedientes.OrdenarPor(copia, criterio, chkDescendente.Checked));
        }

        private void BtnCambiarEstado_Click(object sender, EventArgs e)
        {
            RegistroDocumento seleccionado = null;
            if (dgv.CurrentRow != null) seleccionado = dgv.CurrentRow.DataBoundItem as RegistroDocumento;

            if (seleccionado == null)
            {
                MessageBox.Show("Seleccione un expediente de la tabla.", "Cambiar estado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nuevo = cboEstado.SelectedItem as string;
            if (!Validador.EstadoValido(nuevo))
            {
                MessageBox.Show("Seleccione el nuevo estado de la lista.", "Cambiar estado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboEstado.Focus();
                return;
            }

            if (!Validador.TransicionValida(seleccionado.Estado, nuevo))
            {
                string motivo = (seleccionado.Estado == Validador.EstadoFinal)
                    ? "Un expediente «Atendido» ya no puede cambiar de estado."
                    : "El expediente ya se encuentra en el estado «" + nuevo + "».";
                MessageBox.Show(motivo, "Cambio no permitido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirmar = MessageBox.Show(
                "¿Cambiar el estado del expediente " + seleccionado.CodigoExpediente + " de «" + seleccionado.Estado + "» a «" + nuevo + "»?",
                "Confirmar cambio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmar != DialogResult.Yes) return;

            try
            {
                GestorExpedientes.CambiarEstado(seleccionado.CodigoExpediente, nuevo);
            }
            catch (IOException ex)
            {
                AvisarErrorDeArchivo(ex.Message);
                return;
            }
            catch (UnauthorizedAccessException ex)
            {
                AvisarErrorDeArchivo(ex.Message);
                return;
            }

            bool notificado = GestorExpedientes.Notificar(seleccionado,
                "El estado de su expediente " + seleccionado.CodigoExpediente + " cambió a «" + nuevo + "».");

            Mostrar(mostrados);

            string aviso = "Estado actualizado a «" + nuevo + "».";
            aviso += notificado
                ? "\nNotificación electrónica generada para " + seleccionado.Correo + "."
                : "\nNo se generó notificación (el expediente no tiene correo o no se pudo escribir el archivo).";
            MessageBox.Show(aviso, "Estado actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AvisarErrorDeArchivo(string detalle)
        {
            MessageBox.Show("No se pudo guardar el cambio en el archivo expedientes.txt.\n"
                            + "Verifique que el archivo no esté abierto en otro programa ni protegido contra escritura.\n\n"
                            + "Detalle: " + detalle,
                            "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
