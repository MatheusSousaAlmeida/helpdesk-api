using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Application.UseCases;
using HelpDesk.API.Domain.Interfaces;
using HelpDesk.API.Infrastructure.Data;
using HelpDesk.API.Infrastructure.Data.Repositories;
using HelpDesk.API.Infrastructure.Observability;
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;
namespace HelpDesk.API.Infrastructure.IoC
{
 public static class Bootstrap
 {
  public static IServiceCollection AddHelpDeskInfrastructure(this IServiceCollection services,IConfiguration configuration)
  {
   var cs=configuration.GetConnectionString("OracleDbConnection")??string.Empty;
   services.AddDbContext<ApplicationContext>(o=>o.UseOracle(cs,x=>x.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion21)));
   services.AddTransient<IUsuarioRepository,UsuarioRepository>();services.AddTransient<ITecnicoRepository,TecnicoRepository>();services.AddTransient<IChamadoRepository,ChamadoRepository>();services.AddTransient<IComentarioRepository,ComentarioRepository>();
   services.AddTransient<IUsuarioUseCase,UsuarioUseCase>();services.AddTransient<ITecnicoUseCase,TecnicoUseCase>();services.AddTransient<IChamadoUseCase,ChamadoUseCase>();services.AddTransient<IComentarioUseCase,ComentarioUseCase>();
   services.AddSingleton<ApiMetrics>();return services;
  }
 }
}
