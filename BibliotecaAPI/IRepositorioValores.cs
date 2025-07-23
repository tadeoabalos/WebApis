using BibliotecaAPI.Entidades;

namespace BibliotecaAPI
{
    public interface IRepositorioValores
    {
        IEnumerable<Valor> ObtenerValores();
    }
}
