using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet] 
        public async Task<IEnumerable<Autor>> Get() 
        {
            return await context.Autores.ToListAsync();
        }
        [HttpGet("{id:int}")] // api/autores/id
        public async Task<ActionResult<Autor>> Get([FromRoute]int id, [FromHeader]bool traerlibro) //ActionResult: Resultado de un accion, es un tipo de dato que podemos usar como dato de salida (codigo de estado o autor por ejemplo)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null) 
            {
                return NotFound();
            }
            if (traerlibro) 
            {                
                autor = await context.Autores
                    .Include(x => x.Libros)
                    .FirstOrDefaultAsync(x => x.Id == id);
                return autor;
            }

            return autor;
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor) 
        {
            context.Add(autor);
            await context.SaveChangesAsync(); //Permite lanzar el query de INSERT y no quedarme esperando la respuesta

            return Ok();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Autor autor) 
        {
            if (id != autor.Id) 
            {
                return BadRequest("Los ids deben de coincidir");
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados = await context.Autores.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0) 
            {
                return NotFound("No se encontro el registro a borrar");
            }
            
            return Ok();
        }
    }
}
