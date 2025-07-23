using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Libro>> Get()
        {
            return await context.Libros.ToListAsync();
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound("No se encontro el libro que buscaba");
            }
            return libro;
        }
        [HttpPost]
        public async Task<ActionResult> Post(Libro libro)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existe)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"El autor de id {libro.AutorId} no existe");
                return ValidationProblem();
            }

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Libro>> Put(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest("Los ids deben coincidir");
            }
            var existe = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existe)
            {
                return BadRequest($"El autor de id {libro.Id} no existe");
            }
            context.Update(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Libro>> Delete(int id) 
        {
            var libroEliminar = await context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (libroEliminar == 0) 
            {
                return NotFound("No se encontro el libro que desea eliminar.");
            }

            return Ok();            
        }
    }
}
