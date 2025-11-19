using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ProcesadorStreamSensores;
using ProcesadorStreamSensores.Application;
using ProcesadorStreamSensores.Infrastructure.Json;
using ProcesadorStreamSensores.Infrastructure.Outputs;


var basePath = AppContext.BaseDirectory;
var configPath = Path.Combine(basePath, "config.json");

if (!File.Exists(configPath))
{
    Console.WriteLine($"No se encontró el archivo de configuración: {configPath}");
    Console.WriteLine("Creá un config.json junto al ejecutable.");
    Console.ReadKey();
    return;
}

ProcessingConfig? config;
try
{
    var json = File.ReadAllText(configPath);
    config = JsonSerializer.Deserialize<ProcessingConfig>(json);

    if (config == null)
        throw new InvalidOperationException("No se pudo deserializar config.json.");
}
catch (Exception ex)
{
    Console.WriteLine("Error leyendo config.json:");
    Console.WriteLine(ex.Message);
    Console.ReadKey();
    return;
}


if (string.IsNullOrWhiteSpace(config.InputJsonPath))
{
    Console.WriteLine("InputJsonPath no está configurado en config.json.");
    Console.ReadKey();
    return;
}

if (string.IsNullOrEmpty(Path.GetFileName(config.InputJsonPath)))
{
    Console.WriteLine("InputJsonPath debe incluir el nombre del archivo (no solo la carpeta).");
    Console.ReadKey();
    return;
}

if (!File.Exists(config.InputJsonPath))
{
    Console.WriteLine($"El archivo de entrada no existe: {config.InputJsonPath}");
    Console.ReadKey();
    return;
}

if (string.IsNullOrWhiteSpace(config.SummaryJsonPath))
{
    Console.WriteLine("SummaryJsonPath no está configurado en config.json.");
    Console.ReadKey();
    return;
}

if (string.IsNullOrEmpty(Path.GetFileName(config.SummaryJsonPath)))
{
    Console.WriteLine("SummaryJsonPath debe incluir el nombre del archivo (no solo la carpeta).");
    Console.ReadKey();
    return;
}


var writers = new List<ISensor>();

if (!string.IsNullOrWhiteSpace(config.CsvOutputPath))
{
    if (string.IsNullOrEmpty(Path.GetFileName(config.CsvOutputPath)))
    {
        Console.WriteLine("CsvOutputPath debe incluir el nombre del archivo (no solo la carpeta).");
        Console.ReadKey();
        return;
    }

    writers.Add(new CsvSensorOutputWriter(config.CsvOutputPath));
}

if (!string.IsNullOrWhiteSpace(config.XmlOutputPath))
{
    if (string.IsNullOrEmpty(Path.GetFileName(config.XmlOutputPath)))
    {
        Console.WriteLine("XmlOutputPath debe incluir el nombre del archivo (no solo la carpeta).");
        Console.ReadKey();
        return;
    }

    writers.Add(new XmlSensorOutputWriter(config.XmlOutputPath));
}

if (writers.Count == 0)
{
    Console.WriteLine("No se configuró ningún archivo de salida CSV/XML.");
    Console.WriteLine("Se generará únicamente el resumen JSON.");
}


Console.WriteLine("Iniciando procesamiento de sensores...");

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

var reader = new JsonSensorReader();
var aggregator = new EstadisticasSensores();
var pipeline = new MotorProcesamientoSensores(reader, aggregator, writers);

try
{
    await pipeline.RunAsync(config.InputJsonPath, config.SummaryJsonPath, cancellationToken);

    Console.WriteLine();
    Console.WriteLine("Procesamiento finalizado correctamente.");
    Console.WriteLine($"Resumen: {config.SummaryJsonPath}");
    if (!string.IsNullOrWhiteSpace(config.CsvOutputPath))
        Console.WriteLine($"CSV: {config.CsvOutputPath}");
    if (!string.IsNullOrWhiteSpace(config.XmlOutputPath))
        Console.WriteLine($"XML: {config.XmlOutputPath}");
}
catch (Exception ex)
{
    Console.WriteLine("Se produjo un error:");
    Console.WriteLine(ex.Message);
}

Console.WriteLine();
Console.WriteLine("Presioná una tecla para salir...");
Console.ReadKey();
