using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;
namespace HelpDesk.API.Application.Mappers
{
 public static class TecnicoMapper
 {
  public static Tecnico ToTecnicoEntity(this TecnicoRequestDto obj) => new() { IdTecnico=obj.IdTecnico, Nome=obj.Nome, Email=obj.Email, Especialidade=obj.Especialidade, Ativo=obj.Ativo };
  public static void MapToExisting(this TecnicoRequestDto obj, Tecnico entity) { entity.Nome=obj.Nome; entity.Email=obj.Email; entity.Especialidade=obj.Especialidade; entity.Ativo=obj.Ativo; }
 }
}
