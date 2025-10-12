using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class Docente : Empleado
    {
        public string Especialidad { get; set; }
        public string Titulo { get; set; }

        public Docente(int id, string nombre, string apellido, string position, decimal salary,string titulo)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Position = position;
            Salary = salary;
            Titulo = titulo;
        }
    }
}