using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/valores")]
    public class ValoresController : ControllerBase
    {
        private readonly IRepositorioValores repositorioValores;

        public ValoresController(IRepositorioValores repositorioValores)
        {
            this.repositorioValores = repositorioValores;
        }

        [HttpGet]
        public IEnumerable<Valor> Get()
        {            
            return repositorioValores.ObtenerValores();
        }
    }
}
