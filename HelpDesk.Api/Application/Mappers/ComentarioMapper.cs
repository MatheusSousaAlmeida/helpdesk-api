using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Mappers
{
    public static class ComentarioMapper
    {
        public static Comentario ToComentarioEntity(this ComentarioRequestDto obj)
        {
            return new Comentario
            {
                IdComentario = obj.IdComentario,
                IdChamado = obj.IdChamado,
                Autor = obj.Autor,
                Texto = obj.Texto
            };
        }

        public static void MapToExisting(this ComentarioRequestDto obj, Comentario entity)
        {
            entity.IdChamado = obj.IdChamado;
            entity.Autor = obj.Autor;
            entity.Texto = obj.Texto;
        }
    }
}
