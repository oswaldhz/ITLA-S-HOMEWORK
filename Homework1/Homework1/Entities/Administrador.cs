using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework1.Entities;

namespace Homework1.Entities
{
    internal class Administrador : Empleado
    {
        public string Email { get; set; }
        public string departamento { get; set; }


        public Administrador(int id, string nombre, string apellido, string position, decimal salary, string email, string departamento)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Position = position;
            Salary = salary;
            Email = email;
            Departamento = departamento;
        }
    }
}
