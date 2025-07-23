using BibliotecaAPI.Entidades;

namespace BibliotecaAPI
{
    public class RepositorioValoresOracle : IRepositorioValores
    {
        public IEnumerable<Valor> ObtenerValores()
        {
            return new List<Valor>
            {
                new Valor{ Id = 3, Nombre="Valor Oracle 1"},
                new Valor{ Id = 4, Nombre="Valor Oracle 2"},
                new Valor{ Id = 5, Nombre="Valor Oracle 3"}
            };
        }
    }
}
