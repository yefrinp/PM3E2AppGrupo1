
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2Grupo1.Models
{
    public class Sitios
    {

        public int id { get; set; }
        public string? Descripcion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string? Fotografia { get; set; }
        public string? Audiofile { get; set; }

    }
}
