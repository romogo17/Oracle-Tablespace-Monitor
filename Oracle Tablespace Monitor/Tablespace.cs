using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oracle_Tablespace_Monitor
{
    public class Tablespace
    {
        public bool Visible { get; set; }
        public string Name { get; set; }
        public double Max { get; set; }
        public double Free { get; set; }
        public double Used { get; set; }
        public double RateOfGrowthInBytes { get; set; }
        public double DaysToHwm { get; set; }
        public double DaysToMax { get; set; }
    }
}
