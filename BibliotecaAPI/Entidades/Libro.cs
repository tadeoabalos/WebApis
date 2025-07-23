using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        public required string Titulo { get; set; }
        // Aca indico que la tabla de libros va a tener una relacion con la tabla de autores
        // , y que el campo AutorId es el que contiene la clave foranea.
        public int AutorId { get; set; }
        // Propiedad de navegacion, para poder acceder al autor desde el libro
        public Autor? Autor { get; set; }
    }
}
