using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motors_stand.Models
{
    public class Characteristics
    {
        public double Time { get; set; }
        public int Thrust { get; set; }
        public double Amperage { get; set; }
        public double Voltage { get; set; }
        public double Power => Math.Round(Amperage * Voltage, 2);
        public int Vibration { get; set; }
        public byte Temp { get; set; }
        public int RPM { get; set; }
        public byte MotorValue { get; set; }
        public bool Motor { get; set; }
        public bool Mode { get; set; }
        public double Efficiency
        {
            get
            {
                if(Motor == false || Thrust == 0 || Power == 0) return 0;
                return Math.Round(Thrust / Power, 2);
            }
        }
        public int Volume { get; set; }
        public Characteristics Clone(int period = 1000)
        {
            Characteristics c = MemberwiseClone() as Characteristics;
            index++;
            c.Time = index * period / 1000;
            return c;
        }
        public static void ResetIndex()
        {
            index = 0;
        }
        private static int index = 0;
    }
}
