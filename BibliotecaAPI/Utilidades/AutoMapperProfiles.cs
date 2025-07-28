using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>()
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(autor => MapearNombreYApellidoAutor(autor)));

            CreateMap<Autor, AutorConLibrosDTO>()
                .ForMember(dto => dto.NombreCompleto,
                config => config.MapFrom(autor => MapearNombreYApellidoAutor(autor)));

            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorPatchDTO>().ReverseMap();
            CreateMap<Libro, LibroDTO>();

            CreateMap<CreacionLibroDTO, Libro>();
            CreateMap<Libro, LibroConAutorDTO>()
                .ForMember(dto => dto.AutorNombre,
                config => config.MapFrom(ent => MapearNombreYApellidoAutor(ent.Autor!)));

            CreateMap<ComentarioCreacionDTO,  Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();
        }

        private string MapearNombreYApellidoAutor(Autor autor) => $"{autor.Nombres} {autor.Apellidos}";
    }
}
