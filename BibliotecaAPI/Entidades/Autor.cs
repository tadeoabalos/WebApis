using BibliotecaAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos.")]
        //[PrimeraLetraMay]
        public required string Nombre { get; set; }
        public List<Libro> Libros { get; set; } = new List<Libro>(); // Relacion uno a muchos, un autor puede tener muchos libros

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre)) 
            {
                var primeraLetra = Nombre[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper()) 
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula - por modelo.",
                        new string[] { nameof(Nombre) });
                }
            }
        }

        //[Range(18, 100)]
        //public int Edad { get; set; }

        //[CreditCard]
        //public  string? Tarjeta { get; set; }

        //[Url]
        //public string? url { get; set; }
    }
}
