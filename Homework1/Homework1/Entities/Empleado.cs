using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class Empleado:MiembroDeLaComunidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public string Departamento { get; set; }
    }
}
