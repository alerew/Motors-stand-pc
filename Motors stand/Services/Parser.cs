using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Motors_stand.Services
{
    public class Parser
    {
        public SerialPort Serial;
        public List<string> Data = new List<string>();
        public Parser()
        {
            Serial = new SerialPort();
            Serial.BaudRate = 9600;
            Serial.DataReceived += Serial_DataReceived;
            Serial.StopBits = StopBits.One;
        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Read())
            {
                if (Splitter() != 0)
                {
                    if (Parse())
                        DataReceived?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool Open()
        {
            Close();

            try
            {
                Serial.PortName = Port;
                Serial.Open();
            }
            catch { return false; }
            return true;
        }
        public void Close()
        {
            if (Serial.IsOpen)
            {
                Serial.Close();
            }
        }
        public byte Splitter()
        {
            byte count = 0;
            Data.Clear();
            bool startParse = false;
            string temp = "";
            foreach (char c in Message)
            {
                if (c == 'd' && !startParse)
                {
                    startParse = true;
                }
                else if (startParse)
                {
                    if (c == ',')
                    {
                        Data.Add(temp);
                        temp = "";
                        count++;
                    }
                    else if (c == ';')
                    {
                        Data.Add(temp);
                        return ++count;
                    }
                    else
                    {
                        temp += c;
                    }
                }
            }
            return 0;
        }
        public virtual bool Parse() { return true; }
        public bool Read()
        {
            try
            {
                Message = Serial.ReadLine();
                return true;
            }
            catch { return false; }
        }
        public bool Write(string str)
        {
            if (Serial.IsOpen)
            {
                Serial.Write($"{str}\n");
                return true;
            }
            return false;
        }

        public String Message { get; set; }
        public string Port { get; set; }
        public string[] PortNames => SerialPort.GetPortNames();
        public bool IsOpen => Serial.IsOpen;

        public event EventHandler DataReceived;

    }
}
