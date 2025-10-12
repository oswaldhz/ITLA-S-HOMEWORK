using Homework1.Entities;
using System;

class Program
{
    static void Main()
    {
        MiembroDeLaComunidad miembro = new MiembroDeLaComunidad();
        miembro.Id = 1;
        miembro.Nombre = "Juan";
        miembro.Apellido = "Perez";
        Console.WriteLine($"ID: {miembro.Id} \nNombre: {miembro.Nombre} \nApellido: {miembro.Apellido}\n");

        Administrador admin = new Administrador(1,"Oswald","Flete","admin",20000,"oswald@gmail.com","Finanzas" );
        Console.WriteLine($"ID: {admin.Id} \nNombre: {admin.Nombre} \nApellido: {admin.Apellido}\nDepartamento:{admin.Departamento}\n");


        Administrativo admintrativo = new Administrativo(1, "Ana", "Garcia", "admin", 20000);
        Console.WriteLine($"ID: {admintrativo.Id} \nNombre: {admintrativo.Nombre} \nApellido: {admintrativo.Apellido}\n");
        

        Docente docente = new Docente(1, "Luis", "Martinez", "docente", 15000,"Licenciado en Naturales");
        Console.WriteLine($"ID: {docente.Id} \nNombre: {docente.Nombre} \nApellido: {docente.Apellido}\nTitulo: {docente.Titulo}\n");
        Console.ReadKey();

        
    }
}