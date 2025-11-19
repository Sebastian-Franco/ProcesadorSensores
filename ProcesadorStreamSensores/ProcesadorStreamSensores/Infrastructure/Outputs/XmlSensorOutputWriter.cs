using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using ProcesadorStreamSensores.Application;
using ProcesadorStreamSensores.Domain;

namespace ProcesadorStreamSensores.Infrastructure.Outputs
{
    public class XmlSensorOutputWriter : ISensor
    {
        private readonly string _filePath;
        private XmlWriter _xmlWriter;

        public XmlSensorOutputWriter(string filePath)
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

            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = true
            };

            _xmlWriter = XmlWriter.Create(_filePath, settings);

            await _xmlWriter.WriteStartDocumentAsync();
            await _xmlWriter.WriteStartElementAsync(null, "Sensores", null);
        }

        public async Task RegistrarSensorAsync(Sensor sensor, CancellationToken cancellationToken = default)
        {
            if (_xmlWriter == null)
                throw new InvalidOperationException("XML writer no inicializado.");

            await _xmlWriter.WriteStartElementAsync(null, "Sensor", null);

            await _xmlWriter.WriteElementStringAsync(null, "Index", null, sensor.Index.ToString());
            await _xmlWriter.WriteElementStringAsync(null, "Id", null, sensor.Id);
            await _xmlWriter.WriteElementStringAsync(null, "IsActive", null, sensor.IsActive.ToString());
            await _xmlWriter.WriteElementStringAsync(null, "Zone", null, sensor.Zone);
            await _xmlWriter.WriteElementStringAsync(null, "Value", null, sensor.Value.ToString());

            await _xmlWriter.WriteEndElementAsync(); 
        }

        public async Task FinalizarAsync(CancellationToken cancellationToken = default)
        {
            if (_xmlWriter != null)
            {
                await _xmlWriter.WriteEndElementAsync();   
                await _xmlWriter.WriteEndDocumentAsync();
                await _xmlWriter.FlushAsync();
                _xmlWriter.Dispose();
                _xmlWriter = null;
            }
        }

        public ValueTask DisposeAsync()
        {
            _xmlWriter?.Dispose();
            _xmlWriter = null;
            return ValueTask.CompletedTask;
        }
    }
}
