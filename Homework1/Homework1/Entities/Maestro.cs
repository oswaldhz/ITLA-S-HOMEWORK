using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework1.Entities
{
    internal class Maestro : Empleado
    {
        public string Subject { get; set; }
        public int hoursPerWeek { get; set; }
        public int numberOfStudents { get; set; }

    

    public Maestro(int id, string nombre, string apellido, string position, decimal salary, string subject, int hoursPerWeek, int numberOfStudents)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Position = position;
            Salary = salary;
            Subject = subject;
            hoursPerWeek = hoursPerWeek;
            numberOfStudents = numberOfStudents;
        }
    }
}