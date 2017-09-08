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
        public double RateOfGrowthInMB { get; set; }
        public double DaysToHwm { get; set; }
        public double DaysToMax { get; set; }


        public void setRateOfGrothInBytes(decimal rogb, decimal hwm)
        {
            this.RateOfGrowthInMB = Convert.ToDouble(rogb)/1024/1024;
            this.DaysToMax = Math.Round((Max - Used) / RateOfGrowthInMB,2);
            this.DaysToHwm = Math.Round((Max*Convert.ToDouble(hwm) - Used) / RateOfGrowthInMB,2);

            //System.Console.WriteLine(Max + "-> "+ Used + "-> " + RateOfGrowthInMB  + "-> " + hwm);
        }

        public void updatesRatesWithNewHwm(decimal hwm)
        {
            this.DaysToHwm = Math.Round((Max * Convert.ToDouble(hwm) - Used) / RateOfGrowthInMB, 2);
        }
    }



}
