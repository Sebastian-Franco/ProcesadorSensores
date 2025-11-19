using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcesadorStreamSensores
{
    public class ProcessingConfig
    {
        public string InputJsonPath { get; set; } = string.Empty;
        public string SummaryJsonPath { get; set; } = string.Empty;

        // Acá lo aclaro y luego lo pongo e git en el README -> si están vacíos o null, no se genera ese formato
        public string? CsvOutputPath { get; set; }
        public string? XmlOutputPath { get; set; }
    }
}
