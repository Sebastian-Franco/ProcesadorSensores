using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ProcesadorStreamSensores.Domain;
using ProcesadorStreamSensores.Infrastructure.Json;

namespace ProcesadorStreamSensores.Application
{
    public class MotorProcesamientoSensores
    {
        private readonly JsonSensorReader _reader;
        private readonly EstadisticasSensores _aggregator;
        private readonly IEnumerable<ISensor> _writers;

        public MotorProcesamientoSensores(
            JsonSensorReader reader,
            EstadisticasSensores aggregator,
            IEnumerable<ISensor> writers)
        {
            _reader = reader;
            _aggregator = aggregator;
            _writers = writers;
        }

        public async Task RunAsync(
            string inputJsonPath,
            string summaryJsonPath,
            CancellationToken cancellationToken = default)
        {
            
            var channels = new Dictionary<ISensor, Channel<Sensor>>();
            foreach (var writer in _writers)
            {
                var channel = Channel.CreateBounded<Sensor>(
                    new BoundedChannelOptions(1000)
                    {
                        SingleWriter = true,
                        SingleReader = true
                    });

                channels[writer] = channel;
            }

            var writerTasks = new List<Task>();
            foreach (var writer in _writers)
            {
                var channel = channels[writer];

                var task = Task.Run(async () =>
                {
                    try
                    {
                        await writer.InicializarAsync(cancellationToken);

                        await foreach (var sensor in channel.Reader.ReadAllAsync(cancellationToken))
                        {
                            await writer.RegistrarSensorAsync(sensor, cancellationToken);
                        }

                        await writer.FinalizarAsync(cancellationToken);
                    }
                    finally
                    {
                        await writer.DisposeAsync();
                    }
                }, cancellationToken);

                writerTasks.Add(task);
            }

            await foreach (var sensor in _reader.ReadSensorsAsync(inputJsonPath, cancellationToken))
            {
                _aggregator.ProcesarSensor(sensor);

                foreach (var kvp in channels)
                {
                    await kvp.Value.Writer.WriteAsync(sensor, cancellationToken);
                }
            }

            foreach (var kvp in channels)
            {
                kvp.Value.Writer.Complete();
            }

            await Task.WhenAll(writerTasks);

            await GenerateSummaryAsync(summaryJsonPath, cancellationToken);
        }

        private async Task GenerateSummaryAsync(
            string summaryJsonPath,
            CancellationToken cancellationToken)
        {
            var summary = _aggregator.ConstruirResumen();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(summary, options);

            var dir = Path.GetDirectoryName(summaryJsonPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await File.WriteAllTextAsync(summaryJsonPath, json, cancellationToken);
        }
    }
}
