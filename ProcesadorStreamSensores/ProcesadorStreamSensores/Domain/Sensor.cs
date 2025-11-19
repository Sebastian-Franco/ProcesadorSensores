using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcesadorStreamSensores.Domain
{
    public class Sensor
    {
        public int Index { get; set; }
        public string Id { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Zone { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
