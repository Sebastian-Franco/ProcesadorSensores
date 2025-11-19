# Procesador de Sensores (C# / .NET 8)

Aplicación de consola que lee un archivo **JSON** de sensores, calcula estadísticas y opcionalmente genera archivos **CSV** y **XML** con los mismos datos de entrada.

---

## ¿Qué hace?

A partir de un JSON de sensores como:

json
{
  "index": 0,
  "id": "f4b5b928-949b-4d4a-a13b-a718a0507e3f",
  "isActive": true,
  "zone": "Z01",
  "value": "17999.31"
}
La app:

Lee el archivo en streaming (sensor por sensor, sin cargar todo en memoria).

Calcula:

Id del sensor con mayor valor.

Promedio global.

Promedio por zona.

Sensores activos por zona.

Genera:

Un JSON de resumen con esas estadísticas.

Opcionalmente, archivos CSV y/o XML con los datos originales.

Configuración (config.json)
La app se configura con un archivo config.json ubicado dentro del basepath del proyecto.

Ejemplo:

json

Copiar código
{

  "InputJsonPath": "C:\\Ruta\\sensores.json",
  
  "SummaryJsonPath": "C:\\Ruta\\resumen_sensores.json",
  
  "CsvOutputPath": "D:\\Salidas\\sensores.csv",
  
  "XmlOutputPath": "E:\\Backups\\sensores.xml"
          
}

Si CsvOutputPath está vacío → no se genera CSV.

Si XmlOutputPath está vacío → no se genera XML.

Si ambos están vacíos → se genera solo el resumen JSON.

Los nombres de archivo son libres: no hay nada “hardcodeado”.

Asegurarse de que config.json en el proyecto tenga:

Acción de compilación: Contenido

Copiar en el directorio de salida: Copiar si es posterior

Cómo ejecutar
Abrir la solución en Visual Studio.

Ajustar config.json con rutas válidas.

Establecer el proyecto de consola como Startup Project.

Ejecutar:

Desde Visual Studio: F5, o

Desde consola, en la carpeta del proyecto:

bash
Copiar código
dotnet run
Detalles técnicos (resumen)
Lectura en streaming con IAsyncEnumerable<Sensor> (no se carga el JSON completo en memoria).

Cálculo de estadísticas incremental en una clase de aplicación (EstadisticasSensores).

Escritura concurrente a CSV y XML usando Channel<Sensor> y Task:

Lectura y escritura trabajan en paralelo.

Cada formato escribe a su propio path (pueden ser discos / dispositivos distintos).

Arquitectura separada en capas:

Domain: modelo Sensor.

Application: estadísticas y motor de procesamiento.

Infrastructure: lector JSON + writers CSV/XML.

