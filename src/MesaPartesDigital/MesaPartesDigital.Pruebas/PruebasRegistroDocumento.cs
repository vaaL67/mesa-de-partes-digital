using System;
using Xunit;

namespace MesaPartesCentroLima.Pruebas
{
    public class PruebasRegistroDocumento
    {
        private static readonly DateTime Fecha = new DateTime(2026, 11, 5, 14, 30, 0);

        [Fact]
        public void Un_expediente_nuevo_nace_Recibido_y_sin_correo()
        {
            RegistroDocumento r = new RegistroDocumento("EXP-2026-000001", "12345678", "Ana Perez", "Reclamo", 1);

            Assert.Equal("Recibido", r.Estado);
            Assert.Equal(string.Empty, r.Correo);
        }

        [Fact]
        public void ALinea_usa_el_formato_de_8_campos()
        {
            RegistroDocumento r = new RegistroDocumento("EXP-2026-000001", "12345678", "Tito Calderon", "Reclamo", 3, Fecha, "En trámite", "tito@correo.com");

            Assert.Equal("EXP-2026-000001|12345678|Tito Calderon|Reclamo|3|2026-11-05 14:30:00|En trámite|tito@correo.com", r.ALinea());
        }

        [Fact]
        public void DesdeLinea_reconstruye_el_mismo_registro_que_se_guardo()
        {
            RegistroDocumento original = new RegistroDocumento("EXP-2026-000007", "87654321", "Ana Perez", "Licencia", 12, Fecha, "Observado", "ana@correo.com");

            RegistroDocumento leido = RegistroDocumento.DesdeLinea(original.ALinea());

            Assert.NotNull(leido);
            Assert.Equal(original.CodigoExpediente, leido.CodigoExpediente);
            Assert.Equal(original.DniCiudadano, leido.DniCiudadano);
            Assert.Equal(original.NombreCompleto, leido.NombreCompleto);
            Assert.Equal(original.TipoTramite, leido.TipoTramite);
            Assert.Equal(original.NumeroFolios, leido.NumeroFolios);
            Assert.Equal(original.FechaRegistro, leido.FechaRegistro);
            Assert.Equal(original.Estado, leido.Estado);
            Assert.Equal(original.Correo, leido.Correo);
        }

        [Fact]
        public void DesdeLinea_acepta_el_formato_anterior_de_6_campos()
        {
            RegistroDocumento r = RegistroDocumento.DesdeLinea("EXP-2026-000001|12345678|Ana Perez|Reclamo|3|2026-11-05 14:30:00");

            Assert.NotNull(r);
            Assert.Equal("Recibido", r.Estado);
            Assert.Equal(string.Empty, r.Correo);
            Assert.Equal(Fecha, r.FechaRegistro);
        }

        [Fact]
        public void DesdeLinea_acepta_un_expediente_de_8_campos_sin_correo()
        {
            RegistroDocumento r = RegistroDocumento.DesdeLinea("EXP-2026-000001|12345678|Ana Perez|Reclamo|3|2026-11-05 14:30:00|Atendido|");

            Assert.NotNull(r);
            Assert.Equal("Atendido", r.Estado);
            Assert.Equal(string.Empty, r.Correo);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("EXP-2026-000001|12345678|Ana")]                                                        // faltan campos
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|3|2026-11-05 14:30:00|Recibido")]                 // 7 campos
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|3|2026-11-05 14:30:00|Recibido|a@b.co|sobra")]   // 9 campos
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|tres|2026-11-05 14:30:00")]                       // folios no numérico
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|3|05/11/2026")]                                   // fecha con otro formato
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|3|2026-11-05 14:30:00|Archivado|a@b.co")]        // estado inexistente
        [InlineData("EXP-2026-000001|12345678|Ana|Reclamo|3|2026-11-05 14:30:00|Recibido|correo-malo")]   // correo inválido
        public void DesdeLinea_devuelve_null_si_la_linea_no_es_valida(string linea)
        {
            Assert.Null(RegistroDocumento.DesdeLinea(linea));
        }
    }
}
