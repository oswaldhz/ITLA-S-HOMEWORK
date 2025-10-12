using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class ExEstudiante:MiembroDeLaComunidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string StudentNumber { get; set; }
        public DateTime GraduationDate { get; set; }

        public ExEstudiante(int id, string nombre, string apellido, string studentNumber, DateTime graduationDate)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            StudentNumber = studentNumber;
            GraduationDate = graduationDate;
        }
    }
}
