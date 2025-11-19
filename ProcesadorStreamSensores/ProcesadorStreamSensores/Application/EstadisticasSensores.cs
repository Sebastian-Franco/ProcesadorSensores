using ProcesadorStreamSensores.Domain;

namespace ProcesadorStreamSensores.Application
{ 
    public class EstadisticasSensores
    {
        private decimal _sumaGlobal;
        private long _contadorGlobal;

        private string? _maxSensorId;
        private decimal _maxValorSensor = decimal.MinValue;

        private readonly Dictionary<string, (decimal Sum, long Count)> _estadisticaZona = new();
        private readonly Dictionary<string, long> _activosPorZona = new();

        public void ProcesarSensor(Sensor sensor)
        {
            _sumaGlobal += sensor.Value;
            _contadorGlobal++;

            if (sensor.Value > _maxValorSensor)
            {
                _maxValorSensor = sensor.Value;
                _maxSensorId = sensor.Id;
            }

            if (_estadisticaZona.TryGetValue(sensor.Zone, out var current))
            {
                _estadisticaZona[sensor.Zone] = (current.Sum + sensor.Value, current.Count + 1);
            }
            else
            {
                _estadisticaZona[sensor.Zone] = (sensor.Value, 1);
            }

            if (sensor.IsActive)
            {
                if (_activosPorZona.TryGetValue(sensor.Zone, out var activeCount))
                {
                    _activosPorZona[sensor.Zone] = activeCount + 1;
                }
                else
                {
                    _activosPorZona[sensor.Zone] = 1;
                }
            }
        }

        public ResumenEstadisticas ConstruirResumen()
        {
            var valorMedioGlobal = _contadorGlobal == 0
               ? 0
               : Math.Round(_sumaGlobal / _contadorGlobal, 2);

            var valorMedioPorZona = _estadisticaZona
                .Select(kvp => new valorMedioZona
                {
                    Zona = kvp.Key,
                    ValorMedio = kvp.Value.Count == 0
                        ? 0
                        : Math.Round(kvp.Value.Sum / kvp.Value.Count, 2)
                })
                .ToList();

            var activosPorZona = _activosPorZona
                .Select(kvp => new ActivosPorZona
                {
                    Zona = kvp.Key,
                    Cantidad = kvp.Value
                })
                .ToList();

            return new ResumenEstadisticas
            {
                MaxSensorId = _maxSensorId,
                MaxSensorValue = _maxValorSensor,
                valorMedioGlobal = valorMedioGlobal,
                valorMedioPorZona = valorMedioPorZona,
                SensoresActivosPorZona = activosPorZona
            };
        }
    }

    public class ResumenEstadisticas
    {
        public string? MaxSensorId { get; set; }
        public decimal MaxSensorValue { get; set; }
        public decimal valorMedioGlobal { get; set; }
        public List<valorMedioZona> valorMedioPorZona { get; set; } = new();
        public List<ActivosPorZona> SensoresActivosPorZona { get; set; } = new();
    }

    public class valorMedioZona
    {
        public string Zona { get; set; } = string.Empty;
        public decimal ValorMedio { get; set; }
    }

    public class ActivosPorZona
    {
        public string Zona { get; set; } = string.Empty;
        public long Cantidad { get; set; }
    }
}