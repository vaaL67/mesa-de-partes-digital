using System;

namespace MesaPartesCentroLima
{
    public class RegistroDocumento
    {
        public string CodigoExpediente { get; set; }
        public string DniCiudadano { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoTramite { get; set; }
        public int NumeroFolios { get; set; }
        public DateTime FechaRegistro { get; set; }

        public RegistroDocumento(string codigo, string dni, string nombre, string tramite, int folios)
        {
            CodigoExpediente = codigo;
            DniCiudadano = dni;
            NombreCompleto = nombre;
            TipoTramite = tramite;
            NumeroFolios = folios;
            FechaRegistro = DateTime.Now;
        }

        public override string ToString()
        {
            return "Expediente: " + CodigoExpediente + " | DNI: " + DniCiudadano + " | Solicitante: " + NombreCompleto + " | Trámite: " + TipoTramite + " | Folios: " + NumeroFolios;
        }
    }
}
