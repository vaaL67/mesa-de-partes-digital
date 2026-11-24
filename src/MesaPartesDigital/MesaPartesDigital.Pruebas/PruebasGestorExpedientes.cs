using System;
using System.Collections.Generic;
using Xunit;

namespace MesaPartesCentroLima.Pruebas
{
    public class PruebasGestorExpedientes
    {
        private static readonly DateTime Base = new DateTime(2026, 11, 1, 9, 0, 0);

        private static RegistroDocumento Exp(string codigo, string dni)
        {
            return new RegistroDocumento(codigo, dni, "Ana Perez", "Reclamo", 1);
        }

        // Expediente con todos los datos que importan para ordenar y contar
        private static RegistroDocumento Crear(string codigo, string nombre, string tramite, string estado, DateTime fecha)
        {
            return new RegistroDocumento(codigo, "12345678", nombre, tramite, 1, fecha, estado, "");
        }

        private static string[] Codigos(RegistroDocumento[] datos)
        {
            string[] codigos = new string[datos.Length];
            for (int i = 0; i < datos.Length; i++) codigos[i] = datos[i].CodigoExpediente;
            return codigos;
        }

        // ---------- Generación de código ----------

        [Fact]
        public void Primer_codigo_del_anio_es_000001()
        {
            Assert.Equal("EXP-2026-000001", GestorExpedientes.GenerarCodigo(new List<RegistroDocumento>(), 2026));
        }

        [Fact]
        public void El_codigo_siguiente_es_el_mayor_guardado_mas_uno_aunque_no_esten_en_orden()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000001", "11111111"));
            lista.Add(Exp("EXP-2026-000007", "22222222"));
            lista.Add(Exp("EXP-2026-000003", "33333333"));

            Assert.Equal("EXP-2026-000008", GestorExpedientes.GenerarCodigo(lista, 2026));
        }

        [Fact]
        public void El_correlativo_se_reinicia_en_un_anio_nuevo()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2025-000050", "11111111"));

            Assert.Equal("EXP-2026-000001", GestorExpedientes.GenerarCodigo(lista, 2026));
        }

        [Fact]
        public void Los_codigos_con_numero_danado_se_ignoran()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000002", "11111111"));
            lista.Add(Exp("EXP-2026-00A009", "22222222"));   // letra en el número
            lista.Add(Exp("EXP-2026--00009", "33333333"));   // signo en el número

            Assert.Equal("EXP-2026-000003", GestorExpedientes.GenerarCodigo(lista, 2026));
        }

        // ---------- Búsquedas ----------

        [Fact]
        public void Buscar_por_codigo_encuentra_el_expediente()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000001", "11111111"));
            lista.Add(Exp("EXP-2026-000002", "22222222"));

            RegistroDocumento r = GestorExpedientes.BuscarPorCodigo(lista, "EXP-2026-000002");

            Assert.NotNull(r);
            Assert.Equal("22222222", r.DniCiudadano);
        }

        [Fact]
        public void Buscar_por_codigo_inexistente_devuelve_null()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000001", "11111111"));

            Assert.Null(GestorExpedientes.BuscarPorCodigo(lista, "EXP-2026-000099"));
        }

        [Fact]
        public void Buscar_por_dni_devuelve_todos_los_expedientes_del_ciudadano()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000001", "11111111"));
            lista.Add(Exp("EXP-2026-000002", "22222222"));
            lista.Add(Exp("EXP-2026-000003", "11111111"));

            RegistroDocumento[] encontrados = GestorExpedientes.BuscarPorDni(lista, "11111111");

            Assert.Equal(new string[] { "EXP-2026-000001", "EXP-2026-000003" }, Codigos(encontrados));
        }

        [Fact]
        public void Buscar_por_dni_sin_expedientes_devuelve_arreglo_vacio()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Exp("EXP-2026-000001", "11111111"));

            Assert.Empty(GestorExpedientes.BuscarPorDni(lista, "99999999"));
        }

        // ---------- Ordenamiento por código ----------

        [Fact]
        public void Ordenar_desordenado_deja_los_codigos_ascendentes()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Exp("EXP-2026-000003", "11111111"),
                Exp("EXP-2026-000001", "22222222"),
                Exp("EXP-2026-000002", "33333333")
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPorCodigo(datos);

            Assert.Equal(new string[] { "EXP-2026-000001", "EXP-2026-000002", "EXP-2026-000003" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_respeta_el_anio_antes_que_el_numero()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Exp("EXP-2026-000001", "11111111"),
                Exp("EXP-2025-000009", "22222222")
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPorCodigo(datos);

            Assert.Equal(new string[] { "EXP-2025-000009", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_un_arreglo_ya_ordenado_no_lo_cambia()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Exp("EXP-2026-000001", "11111111"),
                Exp("EXP-2026-000002", "22222222")
            };

            Assert.Equal(new string[] { "EXP-2026-000001", "EXP-2026-000002" }, Codigos(GestorExpedientes.OrdenarPorCodigo(datos)));
        }

        [Fact]
        public void Ordenar_arreglo_vacio_o_de_un_elemento_funciona()
        {
            Assert.Empty(GestorExpedientes.OrdenarPorCodigo(new RegistroDocumento[0]));

            RegistroDocumento[] uno = new RegistroDocumento[] { Exp("EXP-2026-000001", "11111111") };
            Assert.Single(GestorExpedientes.OrdenarPorCodigo(uno));
        }

        // ---------- Ordenamiento por otros criterios ----------

        [Fact]
        public void Ordenar_por_fecha_deja_el_mas_antiguo_primero()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Crear("EXP-2026-000001", "Ana", "Reclamo", "Recibido", Base.AddDays(2)),
                Crear("EXP-2026-000002", "Beto", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000003", "Carlos", "Reclamo", "Recibido", Base.AddDays(1))
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Fecha, false);

            Assert.Equal(new string[] { "EXP-2026-000002", "EXP-2026-000003", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_por_solicitante_ignora_mayusculas()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Crear("EXP-2026-000001", "Carlos", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000002", "ana", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000003", "Beto", "Reclamo", "Recibido", Base)
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Solicitante, false);

            Assert.Equal(new string[] { "EXP-2026-000002", "EXP-2026-000003", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_por_tramite_es_alfabetico()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Crear("EXP-2026-000001", "Ana", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000002", "Ana", "Licencia", "Recibido", Base),
                Crear("EXP-2026-000003", "Ana", "Oficio", "Recibido", Base)
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Tramite, false);

            Assert.Equal(new string[] { "EXP-2026-000002", "EXP-2026-000003", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_por_estado_sigue_el_flujo_y_no_el_alfabeto()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Crear("EXP-2026-000001", "Ana", "Reclamo", "Atendido", Base),
                Crear("EXP-2026-000002", "Ana", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000003", "Ana", "Reclamo", "Observado", Base),
                Crear("EXP-2026-000004", "Ana", "Reclamo", "En trámite", Base)
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Estado, false);

            Assert.Equal(new string[] { "EXP-2026-000002", "EXP-2026-000004", "EXP-2026-000003", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_descendente_invierte_el_resultado()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Exp("EXP-2026-000002", "11111111"),
                Exp("EXP-2026-000003", "22222222"),
                Exp("EXP-2026-000001", "33333333")
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Codigo, true);

            Assert.Equal(new string[] { "EXP-2026-000003", "EXP-2026-000002", "EXP-2026-000001" }, Codigos(ordenado));
        }

        [Fact]
        public void Ordenar_es_estable_con_empates_conserva_el_orden_de_llegada()
        {
            RegistroDocumento[] datos = new RegistroDocumento[]
            {
                Crear("EXP-2026-000001", "Ana", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000002", "Ana", "Reclamo", "Recibido", Base),
                Crear("EXP-2026-000003", "Ana", "Reclamo", "Recibido", Base)
            };

            RegistroDocumento[] ordenado = GestorExpedientes.OrdenarPor(datos, CriterioOrden.Solicitante, false);

            Assert.Equal(new string[] { "EXP-2026-000001", "EXP-2026-000002", "EXP-2026-000003" }, Codigos(ordenado));
        }

        // ---------- Resumen con arreglos paralelos ----------

        [Fact]
        public void Contar_por_estado_sigue_el_orden_del_catalogo()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Crear("EXP-2026-000001", "Ana", "Reclamo", "Recibido", Base));
            lista.Add(Crear("EXP-2026-000002", "Ana", "Licencia", "Recibido", Base));
            lista.Add(Crear("EXP-2026-000003", "Ana", "Reclamo", "Atendido", Base));

            // Estados: Recibido, En trámite, Observado, Atendido
            Assert.Equal(new int[] { 2, 0, 0, 1 }, GestorExpedientes.ContarPorEstado(lista));
        }

        [Fact]
        public void Contar_por_tramite_sigue_el_orden_del_catalogo()
        {
            List<RegistroDocumento> lista = new List<RegistroDocumento>();
            lista.Add(Crear("EXP-2026-000001", "Ana", "Reclamo", "Recibido", Base));
            lista.Add(Crear("EXP-2026-000002", "Ana", "Licencia", "Recibido", Base));
            lista.Add(Crear("EXP-2026-000003", "Ana", "Reclamo", "Atendido", Base));

            // Trámites: Licencia, Petición, Reclamo, Solicitud, Oficio
            Assert.Equal(new int[] { 1, 0, 2, 0, 0 }, GestorExpedientes.ContarPorTramite(lista));
        }

        [Fact]
        public void Contar_con_lista_vacia_devuelve_todo_en_cero()
        {
            List<RegistroDocumento> vacia = new List<RegistroDocumento>();

            Assert.Equal(new int[] { 0, 0, 0, 0 }, GestorExpedientes.ContarPorEstado(vacia));
            Assert.Equal(new int[] { 0, 0, 0, 0, 0 }, GestorExpedientes.ContarPorTramite(vacia));
        }

        // ---------- Notificación electrónica ----------

        [Fact]
        public void La_notificacion_tiene_el_formato_fecha_correo_codigo_mensaje()
        {
            RegistroDocumento r = Exp("EXP-2026-000001", "12345678");
            r.Correo = "ana@correo.com";

            string linea = GestorExpedientes.ConstruirNotificacion(r, "Su expediente fue recibido.", new DateTime(2026, 11, 22, 21, 50, 0));

            Assert.Equal("2026-11-22 21:50:00|ana@correo.com|EXP-2026-000001|Estimado(a) Ana Perez: Su expediente fue recibido.", linea);
        }

        [Fact]
        public void No_se_notifica_si_el_expediente_no_tiene_correo()
        {
            RegistroDocumento sinCorreo = Exp("EXP-2026-000001", "12345678");

            Assert.False(GestorExpedientes.Notificar(sinCorreo, "Mensaje"));
            Assert.False(GestorExpedientes.Notificar(null, "Mensaje"));
        }
    }
}
