using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class Estudiante:MiembroDeLaComunidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string StudentNumber { get; set; }

        public Estudiante(int id, string nombre, string apellido, string studentNumber)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            StudentNumber = studentNumber;
        }
    }
}
