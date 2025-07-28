using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
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
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<LibroDTO>> Get()
        {
            var libros = await context.Libros.ToListAsync();
            var librosDTO = mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }
        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        public async Task<ActionResult<LibroConAutorDTO>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound("No se encontro el libro que buscaba");
            }
            var LibroDTO = mapper.Map<LibroConAutorDTO>(libro);

            return LibroDTO;
        }
        [HttpPost]
        public async Task<ActionResult> Post(CreacionLibroDTO creacionLibroDTO)
        {
            var libro = mapper.Map<Libro>(creacionLibroDTO);
            var existe = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existe)
            {
                ModelState.AddModelError(nameof(libro.AutorId), $"El autor de id {libro.AutorId} no existe");
                return ValidationProblem();
            }
            context.Add(libro);
            await context.SaveChangesAsync();
            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id}, libroDTO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Libro>> Put(int id, CreacionLibroDTO creacionLibroDTO)
        {
            var libro = mapper.Map<Libro>(creacionLibroDTO);
            libro.Id = id;
            var existe = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            if (!existe)
            {
                return BadRequest($"El autor de id {libro.Id} no existe");
            }
            context.Update(libro);
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Libro>> Delete(int id) 
        {
            var libroEliminar = await context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (libroEliminar == 0) 
            {
                return NotFound("No se encontro el libro que desea eliminar.");
            }

            return NoContent();            
        }
    }
}
