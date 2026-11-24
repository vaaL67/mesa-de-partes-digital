using Xunit;

namespace MesaPartesCentroLima.Pruebas
{
    public class PruebasValidador
    {
        [Theory]
        [InlineData("12345678", true)]
        [InlineData("00000000", true)]
        [InlineData("1234567", false)]     // 7 dígitos
        [InlineData("123456789", false)]   // 9 dígitos
        [InlineData("1234567a", false)]    // letra
        [InlineData("1234 678", false)]    // espacio
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Dni_solo_es_valido_con_8_digitos(string dni, bool esperado)
        {
            Assert.Equal(esperado, Validador.DniValido(dni));
        }

        [Theory]
        [InlineData("Tito Calderon", true)]
        [InlineData("María Peña", true)]   // tildes y ñ
        [InlineData("Ana", true)]          // mínimo 3 caracteres
        [InlineData("  Ana  ", true)]      // se recortan los espacios
        [InlineData("Al", false)]          // muy corto
        [InlineData("Juan123", false)]     // números
        [InlineData("Ana|Luis", false)]    // el separador del archivo no se permite
        [InlineData("   ", false)]
        [InlineData(null, false)]
        public void Nombre_solo_letras_y_espacios(string nombre, bool esperado)
        {
            Assert.Equal(esperado, Validador.NombreValido(nombre));
        }

        [Fact]
        public void Nombre_acepta_80_caracteres_y_rechaza_81()
        {
            Assert.True(Validador.NombreValido(new string('a', 80)));
            Assert.False(Validador.NombreValido(new string('a', 81)));
        }

        [Theory]
        [InlineData("  tito   calderon ", "Tito Calderon")]
        [InlineData("MARIA LOPEZ", "Maria Lopez")]
        [InlineData("ana", "Ana")]
        public void Normalizar_nombre_deja_inicial_mayuscula_y_un_solo_espacio(string entrada, string esperado)
        {
            Assert.Equal(esperado, Validador.NormalizarNombre(entrada));
        }

        [Fact]
        public void Normalizar_nombre_nulo_devuelve_texto_vacio()
        {
            Assert.Equal(string.Empty, Validador.NormalizarNombre(null));
        }

        [Theory]
        [InlineData("Licencia", true)]
        [InlineData("Petición", true)]
        [InlineData("Reclamo", true)]
        [InlineData("reclamo", false)]     // distingue mayúsculas: solo se acepta el catálogo
        [InlineData("Otro", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Tramite_debe_pertenecer_al_catalogo(string tramite, bool esperado)
        {
            Assert.Equal(esperado, Validador.TramiteValido(tramite));
        }

        [Theory]
        [InlineData(-5, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(999, true)]
        [InlineData(1000, false)]
        public void Folios_entre_1_y_999(int folios, bool esperado)
        {
            Assert.Equal(esperado, Validador.FoliosValidos(folios));
        }

        [Theory]
        [InlineData("ana@correo.com", true)]
        [InlineData("ana.perez@upn.edu.pe", true)]
        [InlineData("ANA@CORREO.COM", true)]      // se acepta; el formulario lo guarda en minúsculas
        [InlineData("a@b.co", true)]              // mínimo posible
        [InlineData("ana@correo", false)]         // sin punto en el dominio
        [InlineData("ana@@correo.com", false)]    // dos arrobas
        [InlineData("@correo.com", false)]        // nada antes de la arroba
        [InlineData("ana correo@x.com", false)]   // espacio
        [InlineData("ana@.com", false)]           // dominio empieza con punto
        [InlineData("ana@correo.", false)]        // termina en punto
        [InlineData("ana@correo.c", false)]       // terminación de 1 letra
        [InlineData("ana@co..com", false)]        // puntos seguidos
        [InlineData("ana|x@correo.com", false)]   // el separador del archivo no se permite
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Correo_con_formato_basico_valido(string correo, bool esperado)
        {
            Assert.Equal(esperado, Validador.CorreoValido(correo));
        }

        [Fact]
        public void Correo_acepta_80_caracteres_y_rechaza_81()
        {
            Assert.True(Validador.CorreoValido(new string('a', 74) + "@x.com"));   // 80
            Assert.False(Validador.CorreoValido(new string('a', 75) + "@x.com"));  // 81
        }

        [Theory]
        [InlineData("Recibido", true)]
        [InlineData("En trámite", true)]
        [InlineData("Observado", true)]
        [InlineData("Atendido", true)]
        [InlineData("atendido", false)]    // distingue mayúsculas
        [InlineData("Archivado", false)]
        [InlineData(null, false)]
        public void Estado_debe_pertenecer_al_catalogo(string estado, bool esperado)
        {
            Assert.Equal(esperado, Validador.EstadoValido(estado));
        }

        [Theory]
        [InlineData("Recibido", "En trámite", true)]
        [InlineData("En trámite", "Atendido", true)]
        [InlineData("En trámite", "Observado", true)]
        [InlineData("Observado", "En trámite", true)]
        [InlineData("Atendido", "En trámite", false)]   // Atendido es final
        [InlineData("Recibido", "Recibido", false)]     // mismo estado
        [InlineData("Recibido", "Archivado", false)]    // estado inexistente
        [InlineData("Recibido", null, false)]
        public void Transicion_de_estado_valida(string actual, string nuevo, bool esperado)
        {
            Assert.Equal(esperado, Validador.TransicionValida(actual, nuevo));
        }

        [Theory]
        [InlineData("EXP-2026-000001", true)]
        [InlineData("EXP-2027-999999", true)]
        [InlineData("exp-2026-000001", false)]   // minúsculas
        [InlineData("EXP-2026-00001", false)]    // falta un dígito
        [InlineData("EXP-2026-0000011", false)]  // sobra un dígito
        [InlineData("EXP-2026-00000A", false)]   // letra en el número
        [InlineData("EXP-20X6-000001", false)]   // letra en el año
        [InlineData("EXP-2026_000001", false)]   // separador incorrecto
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Codigo_con_formato_EXP_AAAA_NNNNNN(string codigo, bool esperado)
        {
            Assert.Equal(esperado, Validador.CodigoValido(codigo));
        }
    }
}
