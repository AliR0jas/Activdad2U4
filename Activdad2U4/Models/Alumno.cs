using System;
using System.Collections.Generic;

namespace Activdad2U4.Models
{
    public partial class Alumno
    {
        public int Id { get; set; }
        public string NoControl { get; set; }
        public string Nombre { get; set; }
        public int IdMaestro { get; set; }

        public virtual Maestro IdMaestroNavigation { get; set; }
    }
}
