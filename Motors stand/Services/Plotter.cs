using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Motors_stand.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace Motors_stand.Services
{
    internal class Plotter
    {
        private const int CountLine = 10;

        private enum SeriesName : int
        {
            Thrust,
            Amperage,
            Voltage,
            Power,
            Vibration,
            Temp,
            RPM,
            MotorValue,
            Efficiency,
            Volume
        }

        public PlotModel Plot { get; set; }
        LineSeries[] lineSeries;
        LinearAxis TimeAxis;
        LinearAxis ValueAxis;
        Legend legend;
        private string[] Names { get; } = new string[CountLine] { "Тяга (г)", "Сила тока (А)", "Напряжение (В)", "Мощность (Вт)", "Вибрация", "Температура (°C)", "Скорость вращения (об/мин)", "Управляющий сигнал", "Эффективность (г/Вт)", "Громкость (дБ)" };
        private string[] NamesEN { get; } = new string[CountLine] { "Thrust (g)", "Amperage (A)", "Voltage (V)", "Power (W)", "Vibration", "Temp (°C)", "RPM", "Signal", "Efficiency (g/W)", "Volume (dB)" };
        private OxyColor[] colors { get; } = new OxyColor[CountLine];

        private readonly OxyColor White = OxyColor.FromRgb(255, 255, 255);
        private readonly OxyColor Black = OxyColor.FromRgb(0, 0, 0);
        public Plotter()
        {
            Plot = new PlotModel() { PlotAreaBorderColor = White };
            TimeAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Время",
                TextColor = White,
                TicklineColor = White,
                TitleColor = White
            };
            ValueAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Значение",
                TextColor = White,
                TicklineColor = White,
                TitleColor = White
            };
            legend = new Legend()
            {
                LegendBackground = OxyColor.Parse("#20155E"),
                LegendOrientation = LegendOrientation.Horizontal,
                LegendTextColor = White,
                ShowInvisibleSeries = false,
            };

            Plot.Axes.Add(TimeAxis);
            Plot.Axes.Add(ValueAxis);
            Plot.Legends.Add(legend);

            GetColors();
            lineSeries = new LineSeries[CountLine];
            for (int i = 0; i < lineSeries.Length; i++)
            {
                lineSeries[i] = new LineSeries() { Title = Names[i], Color = colors[i] };
                Plot.Series.Add(lineSeries[i]);
            }
            IsVisibleAmperage = false;
            IsVisibleVoltage = false;
            IsVisiblePower = false;
            IsVisibleVibration = false;
            IsVisibleTemp = false;
            IsVisibleRPM = false;
            IsVisibleEfficiency = false;
            IsVisibleVolume = false;

        }
        private double index = 0;
        public void AddData(Characteristics c, int period = 300)
        {
            lineSeries[0].Points.Add(new DataPoint(index, c.Thrust));
            lineSeries[1].Points.Add(new DataPoint(index, c.Amperage));
            lineSeries[2].Points.Add(new DataPoint(index, c.Voltage));
            lineSeries[3].Points.Add(new DataPoint(index, c.Power));
            lineSeries[4].Points.Add(new DataPoint(index, c.Vibration));
            lineSeries[5].Points.Add(new DataPoint(index, c.Temp));
            lineSeries[6].Points.Add(new DataPoint(index, c.RPM));
            lineSeries[7].Points.Add(new DataPoint(index, c.MotorValue));
            lineSeries[8].Points.Add(new DataPoint(index, c.Efficiency));
            lineSeries[9].Points.Add(new DataPoint(index, c.Volume));
            Plot.InvalidatePlot(true);
            index += (period / 1000d);
        }
        public void Clear()
        {
            index = 0;
            foreach(var line in lineSeries)
                line.Points.Clear();
            Plot.InvalidatePlot(true);
        }
        public void SavePDF(string Path, int width = 3508, int height = 2480)
        {
            PrepareToPDF();
            using (FileStream writer = File.Create(Path))
            {
                PdfExporter.Export(Plot, writer, width, height);
            }
            PrepareToView();
        }
        public void SaveSVG(string Path, int width = 3508, int height = 2480)
        {
            PrepareToPDF();
            using(FileStream writer = File.Create(Path))
            {
                SvgExporter.Export(Plot, writer, width, height, true);
            }
            PrepareToView();
        }

        private void GetColors()
        {
            colors[0] = OxyColor.Parse("#FF0000");
            colors[1] = OxyColor.Parse("#FF8000");
            colors[2] = OxyColor.Parse("#FFFF00");
            colors[3] = OxyColor.Parse("#00FF00");
            colors[4] = OxyColor.Parse("#00FFFF");
            colors[5] = OxyColor.Parse("#0000FF");
            colors[6] = OxyColor.Parse("#FF00FF");
            colors[7] = OxyColor.Parse("#8673a1");
            colors[8] = OxyColor.Parse("#FFFFFF");
            colors[9] = OxyColor.Parse("#999999");
        }

        private void PrepareToPDF()
        {
            Plot.PlotAreaBorderColor = Black;

            lineSeries[8].Color = Black;

            TimeAxis.AxislineColor = Black;
            TimeAxis.TextColor = Black;
            TimeAxis.TicklineColor = Black;
            TimeAxis.TitleColor = Black;
            TimeAxis.Title = "Time";

            ValueAxis.AxislineColor = Black;
            ValueAxis.TextColor = Black;
            ValueAxis.TicklineColor = Black;
            ValueAxis.TitleColor = Black;
            ValueAxis.Title = "Value";

            legend.LegendFontSize = 24;
            legend.LegendBackground = White;
            legend.LegendTextColor = Black;
            legend.LegendBorder = Black;

            for (int i = 0; i < CountLine; i++)
            {
                lineSeries[i].Title = NamesEN[i];
            }
        }
        private void PrepareToView()
        {
            Plot.PlotAreaBorderColor = White;

            lineSeries[8].Color = White;

            TimeAxis.AxislineColor = White;
            TimeAxis.TextColor = White;
            TimeAxis.TicklineColor = White;
            TimeAxis.TitleColor = White;
            TimeAxis.Title = "Время";

            ValueAxis.AxislineColor = White;
            ValueAxis.TextColor = White;
            ValueAxis.TicklineColor = White;
            ValueAxis.TitleColor = White;
            ValueAxis.Title = "Значение";

            legend.LegendFontSize = double.NaN;
            legend.LegendBackground = OxyColor.Parse("#20155E");
            legend.LegendTextColor = White;
            legend.LegendBorder = OxyColor.FromAColor(0, White);

            for (int i = 0; i < CountLine; i++)
            {
                lineSeries[i].Title = Names[i];
            }
        }

        #region IsVisible
        public bool IsVisibleThrust
        {
            get => lineSeries[(int)SeriesName.Thrust].IsVisible;
            set
            {
                lineSeries[(int)SeriesName.Thrust].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleAmperage
        {
            get => lineSeries[(int)SeriesName.Amperage].IsVisible;
            set
            {
                lineSeries[(int)SeriesName.Amperage].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleVoltage
        {
            get => lineSeries[(int)SeriesName.Voltage].IsVisible;
            set {
                lineSeries[(int)SeriesName.Voltage].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisiblePower
        {
            get => lineSeries[(int)SeriesName.Power].IsVisible;
            set {
                lineSeries[(int)SeriesName.Power].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleVibration
        {
            get => lineSeries[(int)SeriesName.Vibration].IsVisible;
            set {
                lineSeries[(int)SeriesName.Vibration].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleTemp
        {
            get => lineSeries[(int)SeriesName.Temp].IsVisible;
            set {
                lineSeries[(int)SeriesName.Temp].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleRPM
        {
            get => lineSeries[(int)SeriesName.RPM].IsVisible;
            set {
                lineSeries[(int)SeriesName.RPM].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleMotorValue
        {
            get => lineSeries[(int)SeriesName.MotorValue].IsVisible;
            set {
                lineSeries[(int)SeriesName.MotorValue].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        public bool IsVisibleEfficiency
        {
            get => lineSeries[(int)SeriesName.Efficiency].IsVisible;
            set {
                lineSeries[(int)SeriesName.Efficiency].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }

        public bool IsVisibleVolume
        {
            get => lineSeries[(int)SeriesName.Volume].IsVisible;
            set
            {
                lineSeries[(int)SeriesName.Volume].IsVisible = value;
                Plot.InvalidatePlot(true);
            }
        }
        #endregion
    }
}
