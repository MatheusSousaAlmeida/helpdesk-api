using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Mappers
{
    public static class ChamadoMapper
    {
        public static Chamado ToChamadoEntity(this ChamadoRequestDto obj)
        {
            return new Chamado
            {
                IdChamado = obj.IdChamado,
                IdUsuario = obj.IdUsuario,
                IdTecnico = obj.IdTecnico,
                Titulo = obj.Titulo,
                Descricao = obj.Descricao,
                Prioridade = obj.Prioridade,
                Status = obj.Status ?? string.Empty
            };
        }

        public static void MapToExisting(this ChamadoRequestDto obj, Chamado entity)
        {
            entity.IdUsuario = obj.IdUsuario;
            entity.IdTecnico = obj.IdTecnico;
            entity.Titulo = obj.Titulo;
            entity.Descricao = obj.Descricao;
            entity.Prioridade = obj.Prioridade;

            if (!string.IsNullOrWhiteSpace(obj.Status))
                entity.Status = obj.Status;
        }
    }
}
