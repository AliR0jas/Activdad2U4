using System;
using System.Collections.Generic;

namespace Activdad2U4.Models
{
    public partial class Maestro
    {
        public Maestro()
        {
            Alumno = new HashSet<Alumno>();
        }

        public int Id { get; set; }
        public int NoControl { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public ulong? Activo { get; set; }

        public virtual ICollection<Alumno> Alumno { get; set; }
    }
}
