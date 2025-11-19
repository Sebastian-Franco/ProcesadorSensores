using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ProcesadorStreamSensores.Domain;

namespace ProcesadorStreamSensores.Infrastructure.Json
{
    public class JsonSensorReader
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public async IAsyncEnumerable<Sensor> ReadSensorsAsync(
            string filePath,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await using var stream = File.OpenRead(filePath);

            await foreach (var dto in JsonSerializer.DeserializeAsyncEnumerable<SensorInputDto>(
                               stream, _options, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (dto == null)
                    continue;

                if (!decimal.TryParse(dto.ValueRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                {
                    continue;
                }

                yield return new Sensor
                {
                    Index = dto.Index,
                    Id = dto.Id,
                    IsActive = dto.IsActive,
                    Zone = dto.Zone,
                    Value = value
                };
            }
        }
    }
}
