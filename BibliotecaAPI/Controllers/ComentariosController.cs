using AutoMapper;
using BibliotecaAPI.Datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    public class ComentariosController
    {
        [ApiController]
        [Route("api/libro/{libro:int}/comentarios")]
        public class ComentariosControllers: ControllerBase 
        {
            private readonly ApplicationDbContext context;
            private readonly IMapper mapper;

            public ComentariosControllers(ApplicationDbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            [HttpGet]
            public async Task<ActionResult<IEnumerable<ComentarioDTO>>> Get(int libroId) 
            {
                var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

                if (!existeLibro)
                {
                    return NotFound();
                }

                var comentarios = await context.Comentarios
                    .Where(x => x.LibroId == libroId)
                    .OrderByDescending(x => x.FechaPublicaion)
                    .ToListAsync();

                return mapper.Map<List<ComentarioDTO>>(comentarios);
            }
            [HttpGet("{id}", Name = "ObtenerComentario")]
            public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
            {
                var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
                if(comentario is null) 
                {
                    return NotFound();
                }
                return mapper.Map<ComentarioDTO>(comentario);
            }

            [HttpPost]
            public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
            {
                var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

                if (!existeLibro) { return NotFound(); }

                var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
                comentario.LibroId = libroId;
                comentario.FechaPublicaion = DateTime.UtcNow;
                context.Add(comentario);
                await context.SaveChangesAsync();

                var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

                return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroId }, comentarioDTO);
            }
        }
    }
}
