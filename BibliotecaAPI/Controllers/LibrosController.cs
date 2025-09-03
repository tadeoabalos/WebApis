using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros")]
    [Authorize(Policy = "esadmin")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        public const string cache = "libros-obtener";
        public LibrosController(ApplicationDbContext context, IMapper mapper, IOutputCacheStore outputCacheStore)
        { 
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }        

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<IEnumerable<LibroDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Libros.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var libros = await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
            var librosDTO = mapper.Map<IEnumerable<LibroDTO>>(libros);
            return librosDTO;
        }
        [HttpGet("{id:int}", Name = "ObtenerLibro")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<ActionResult<LibroConAutoresDTO>> Get(int id)
        {
            var libro = await context.Libros
                .Include(x => x.Autores)
                    .ThenInclude( x => x.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (libro is null)
            {
                return NotFound("No se encontro el libro que buscaba");
            }
            var LibroDTO = mapper.Map<LibroConAutoresDTO>(libro);

            return LibroDTO;
        }
        [HttpPost]
        public async Task<ActionResult> Post(CreacionLibroDTO creacionLibroDTO)
        {
            if (creacionLibroDTO.AutoresIds is null || creacionLibroDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(creacionLibroDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            var autoresIdsExisten = await context.Autores
                .Where(x => creacionLibroDTO.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();

            if(autoresIdsExisten.Count != creacionLibroDTO.AutoresIds.Count)
            {
                var autoresNoExisten = creacionLibroDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresNoExistenString = string.Join(",", autoresNoExisten);
                var mensajeDeError = $"Los siguientes autores no existen: {autoresNoExistenString}";
                ModelState.AddModelError(nameof(creacionLibroDTO.AutoresIds), mensajeDeError);
                return ValidationProblem();
            }

            var libro = mapper.Map<Libro>(creacionLibroDTO);
            AsignarOrdenAutores(libro);
            context.Add(libro);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
        }

        private void AsignarOrdenAutores(Libro libro) 
        {
            if(libro.Autores is not null)
            {
                for(int i = 0; i < libro.Autores.Count; i++)
                {
                    libro.Autores[i].Orden = i;
                }
            }
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Libro>> Put(int id, CreacionLibroDTO creacionLibroDTO)
        {
            if (creacionLibroDTO.AutoresIds is null || creacionLibroDTO.AutoresIds.Count == 0)
            {
                ModelState.AddModelError(nameof(creacionLibroDTO.AutoresIds), "No se puede crear un libro sin autores");
                return ValidationProblem();
            }

            var autoresIdsExisten = await context.Autores
                .Where(x => creacionLibroDTO.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();

            if (autoresIdsExisten.Count != creacionLibroDTO.AutoresIds.Count)
            {
                var autoresNoExisten = creacionLibroDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresNoExistenString = string.Join(",", autoresNoExisten);
                var mensajeDeError = $"Los siguientes autores no existen: {autoresNoExistenString}";
                ModelState.AddModelError(nameof(creacionLibroDTO.AutoresIds), mensajeDeError);
                return ValidationProblem();
            }

            var libroDB = await context.Libros
                            .Include(x => x.Autores)
                            .FirstOrDefaultAsync(x => x.Id == id);

           if(libroDB is null) 
            {
                return NotFound();
            }

            libroDB = mapper.Map(creacionLibroDTO, libroDB);
            AsignarOrdenAutores(libroDB);
            
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Libro>> Delete(int id) 
        {
            var libroEliminar = await context.Libros.Where(x => x.Id == id).ExecuteDeleteAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            if (libroEliminar == 0) 
            {
                return NotFound("No se encontro el libro que desea eliminar.");
            }

            return NoContent();            
        }
    }
}
