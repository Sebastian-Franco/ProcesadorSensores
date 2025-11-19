using System.Threading;
using System.Threading.Tasks;
using ProcesadorStreamSensores.Domain;

namespace ProcesadorStreamSensores.Application
{
    public interface ISensor : IAsyncDisposable
    {
        Task InicializarAsync(CancellationToken cancellationToken = default);
        Task RegistrarSensorAsync(Sensor sensor, CancellationToken cancellationToken = default);
        Task FinalizarAsync(CancellationToken cancellationToken = default);
    }
}
