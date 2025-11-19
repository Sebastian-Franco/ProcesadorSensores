using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProcesadorStreamSensores.Application;
using ProcesadorStreamSensores.Domain;

namespace ProcesadorStreamSensores.Infrastructure.Outputs
{
    public class CsvSensorOutputWriter : ISensor
    {
        private readonly string _filePath;
        private StreamWriter _writer;

        public CsvSensorOutputWriter(string filePath)
        {
            _filePath = filePath;
        }

        public async Task InicializarAsync(CancellationToken cancellationToken = default)
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _writer = new StreamWriter(_filePath, append: false, Encoding.UTF8);

            // Encabezado CSV
            await _writer.WriteLineAsync("index;id;isActive;zone;value");
        }

        public async Task RegistrarSensorAsync(Sensor sensor, CancellationToken cancellationToken = default)
        {
            if (_writer == null)
                throw new InvalidOperationException("CSV writer no inicializado.");

            var line = $"{sensor.Index};{sensor.Id};{sensor.IsActive};{sensor.Zone};{sensor.Value}";
            await _writer.WriteLineAsync(line);
        }

        public async Task FinalizarAsync(CancellationToken cancellationToken = default)
        {
            if (_writer != null)
            {
                await _writer.FlushAsync();
                _writer.Dispose();
                _writer = null;
            }
        }

        public ValueTask DisposeAsync()
        {
            _writer?.Dispose();
            _writer = null;
            return ValueTask.CompletedTask;
        }
    }
}
