using Motors_stand.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motors_stand.Services
{
    public class StandParser : Parser
    {
        public Characteristics characteristics { get; set; }
        public StandParser()
        {
            characteristics = new Characteristics();
        }

        public override bool Parse()
        {
            if (Data.Count >= 8)
            {
                characteristics.Thrust = Convert.ToInt32(Data[0]);
                characteristics.Amperage = Convert.ToDouble(Data[1], new System.Globalization.CultureInfo("en-US"));
                characteristics.Voltage = Convert.ToDouble(Data[2], new System.Globalization.CultureInfo("en-US"));
                characteristics.Vibration = Convert.ToInt32(Data[3]);
                characteristics.Temp = Convert.ToByte(Data[4]);
                characteristics.RPM = Convert.ToInt32(Data[5]);
                characteristics.MotorValue = Convert.ToByte(Data[6]);
                characteristics.Motor = Convert.ToByte(Data[7]) != 0;
                characteristics.Mode = Convert.ToByte(Data[8]) != 0;
                characteristics.Volume = Convert.ToInt32(Data[9]);
                return true;
            }
            return false;
        }

        public bool Tare()
        {
            return Write("c0;");
        }
        public bool SetMotorValue(byte value)
        {
            characteristics.MotorValue = value;
            return Write($"c1,{value};");
        }
        public bool SetMotor(bool value)
        {
            characteristics.Motor = value;
            return Write($"c2,{(value ? 1 : 0)};");
        }
        public bool SetMode(bool value)
        {
            characteristics.Mode = value;

            return Write($"c3,{(value ? 1 : 0)};");
        }

        public bool UpdateSettings(byte NumberOfBlades, ushort SensorsInterval, ushort SendInterval, ushort LCDInterval)
        {
            return Write($"c4,{NumberOfBlades};") && Write($"c5,{SensorsInterval};") && Write($"c6,{SendInterval};") && Write($"c7,{LCDInterval};");
        }

        public int Thrust => characteristics.Thrust;
        public double Amperage => characteristics.Amperage;
        public double Voltage => characteristics.Voltage;
        public int Vibration => characteristics.Vibration;
        public byte Temp => characteristics.Temp;
        public int RPM => characteristics.RPM;
        public byte MotorValue
        {
            get => characteristics.MotorValue;
            set
            {
                SetMotorValue(value);
            }
        }
        public bool Motor
        {
            get => characteristics.Motor;
            set
            {
                SetMotor(value);
            }
        }
        public bool Mode
        {
            get => characteristics.Mode;
            set
            {
                SetMode(value);
            }
        }
        public double Efficiency => characteristics.Efficiency;
        public double Power => characteristics.Power;
        public int Volume => characteristics.Volume;
    }
}
