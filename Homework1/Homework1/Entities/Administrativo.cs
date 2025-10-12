using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class Administrativo : Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
 

        public Administrativo(int id, string nombre, string apellido, string position, decimal salary)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Position = position;
            Salary = salary;
        }
    }
}