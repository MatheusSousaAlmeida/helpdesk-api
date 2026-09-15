using System.Diagnostics;
using System.Diagnostics.Metrics;
namespace HelpDesk.API.Infrastructure.Observability
{
 public class ApiMetrics
 {
  public const string MeterName="HelpDesk.API";private readonly Meter _meter=new(MeterName,"1.0.0");private readonly Counter<long> _requests;private readonly Counter<long> _errors;private readonly Counter<long> _created;private readonly Histogram<double> _time;private long _totalRequests,_totalErrors,_totalCreated,_durationMicroseconds;
  public ApiMetrics(){_requests=_meter.CreateCounter<long>("helpdesk.api.requests","requests");_errors=_meter.CreateCounter<long>("helpdesk.api.errors","errors");_created=_meter.CreateCounter<long>("helpdesk.chamados.criados","chamados");_time=_meter.CreateHistogram<double>("helpdesk.api.response_time","ms");_meter.CreateObservableGauge("helpdesk.api.error_rate",()=>GetSnapshot().ErrorRatePercent,"%");_meter.CreateObservableGauge("helpdesk.api.average_response_time",()=>GetSnapshot().AverageResponseTimeMs,"ms");}
  public void Record(string method,string route,int status,double ms){var tags=new TagList{{"http.request.method",method},{"http.route",route},{"http.response.status_code",status}};_requests.Add(1,tags);_time.Record(ms,tags);Interlocked.Increment(ref _totalRequests);Interlocked.Add(ref _durationMicroseconds,(long)(ms*1000));if(status>=400){_errors.Add(1,tags);Interlocked.Increment(ref _totalErrors);}}
  public void RecordChamadoCriado(){_created.Add(1);Interlocked.Increment(ref _totalCreated);}
  public ApiMetricsSnapshot GetSnapshot(){var r=Interlocked.Read(ref _totalRequests);var e=Interlocked.Read(ref _totalErrors);var c=Interlocked.Read(ref _totalCreated);var d=Interlocked.Read(ref _durationMicroseconds);return new(r,e,c,Math.Round(r==0?0:e*100d/r,2),Math.Round(r==0?0:d/1000d/r,2));}
 }
 public record ApiMetricsSnapshot(long TotalRequests,long TotalErrors,long TotalChamadosCriados,double ErrorRatePercent,double AverageResponseTimeMs);
}
