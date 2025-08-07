using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BibliotecaAPI.DTOs
{
    public class ComentarioDTO
    {
        public Guid Id { get; set; }        
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicaion { get; set; }
        public required string UsuarioId { get; set; }
        public required string UsuarioEmail {  get; set; }
    }
}
