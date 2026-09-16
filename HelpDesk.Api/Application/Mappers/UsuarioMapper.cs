using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Mappers
{
    public static class UsuarioMapper
    {
        public static Usuario ToUsuarioEntity(this UsuarioRequestDto obj)
        {
            return new Usuario
            {
                IdUsuario = obj.IdUsuario,
                Nome = obj.Nome,
                Email = obj.Email,
                Departamento = obj.Departamento,
                Ativo = obj.Ativo
            };
        }

        public static void MapToExisting(this UsuarioRequestDto obj, Usuario entity)
        {
            entity.Nome = obj.Nome;
            entity.Email = obj.Email;
            entity.Departamento = obj.Departamento;
            entity.Ativo = obj.Ativo;
        }
    }
}
