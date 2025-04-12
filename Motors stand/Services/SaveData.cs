using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motors_stand.Models;
using System.IO;
using System.Text.Json;

namespace Motors_stand.Services
{
    internal class SaveData
    {
        public string Path { get; set; }
        public int FilterIndex { get; set; }
        public List<Characteristics> ListCharacteristics { get; set; }
        public SaveData()
        {
            ListCharacteristics = new List<Characteristics>();
        }
        public void AddData(Characteristics c, int period = 1000)
        {
            if (c.Motor == true)
            {
                ListCharacteristics.Add(c.Clone(period));
            }
        }
        public void Clear()
        {
            ListCharacteristics.Clear();
            Characteristics.ResetIndex();
        }
        public bool Dialog()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Таблица (CSV)|*.csv|JSON|*.json|График (PDF)|*.pdf|График (SVG)|*.svg"
            };
            if (sfd.ShowDialog() == false) return false;
            Path = sfd.FileName;
            FilterIndex = sfd.FilterIndex;
            return true;
        }
        public void SaveCSV()
        {
            using(StreamWriter writer = File.CreateText(Path))
            {
                writer.WriteLine("Время (с);Тяга (г);Сила тока (А);Напряжение (В);Мощность (Вт);Вибрация;Температура (°C);RPM (об/мин);Управляющий сигнал;Эффективность (г/Вт);Громкость (дБ)");
                foreach (Characteristics c in ListCharacteristics)
                {
                    line = "";
                    AddValue(c.Time);
                    AddValue(c.Thrust);
                    AddValue(c.Amperage);
                    AddValue(c.Voltage);
                    AddValue(c.Power);
                    AddValue(c.Vibration);
                    AddValue(c.Temp);
                    AddValue(c.RPM);
                    AddValue(c.MotorValue);
                    AddValue(c.Efficiency);
                    AddValue(c.Volume, ' ');
                    writer.WriteLine(line);
                }
            }
        }
        public void SaveJSON()
        {
            using (FileStream writer = File.Create(Path))
            {
                JsonSerializer.Serialize(writer, ListCharacteristics);
            }
        }
        public int DialogSave()
        {
            if (Dialog())
            {
                if (FilterIndex == 1)
                    SaveCSV();
                else if (FilterIndex == 2)
                    SaveJSON();
                return FilterIndex;
            }
            return 0;
        }
        private string line;
        private void AddValue<T>(T value, char end = ';')
        {
            line += value;
            line += end;
        }
    }
}
