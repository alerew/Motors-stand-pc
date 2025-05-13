using Motors_stand.Infrastructure;
using Motors_stand.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using Motors_stand.Models;
using OxyPlot;

namespace Motors_stand.ViewModel
{
    class MainWindowVM : BaseVM
    {
        public StandParser parser;
        SaveData saveData;

        #region Values

        #region PortNames
        public string[] PortNames
        {
            get => parser.PortNames;
        }
        #endregion

        #region IsOpen
        public bool IsOpen
        {
            get => parser.IsOpen;
        }
        #endregion

        #region SelectedPort
        public string SelectedPort
        {
            set
            {
                parser.Port = value;
                OnPropertyChanged("SelectedPort");
            }
            get => parser.Port;
        }
        #endregion

        #region Thrust
        public String Thrust
        {
            get => $"Тяга: {parser.Thrust} г";
        }
        #endregion

        #region Amperage
        public String Amperage
        {
            get => $"Сила тока: {parser.Amperage} А";
        }
        #endregion

        #region Voltage
        public String Voltage
        {
            get => $"Напряжение: {parser.Voltage} В";
        }
        #endregion

        #region Vibration
        public String Vibration
        {
            get => $"Вибрация: {parser.Vibration}";
        }
        #endregion

        #region Temp
        public String Temp
        {
            get => $"Температура: {parser.Temp}°C";
        }
        #endregion

        #region RPM
        public String RPM
        {
            get => $"Скорость вращения: {parser.RPM}";
        }
        #endregion

        #region MotorValue
        public byte MotorValue
        {
            get => parser.MotorValue;
            set
            {
                //parser.MotorValue = value;
                CheckConnection(parser.SetMotorValue(value));
                Set();
            }
        }
        #endregion

        #region Message
        public String Message
        {
            get => parser.Message;
        }
        #endregion

        #region Motor
        public bool Motor => parser.Motor;
        #endregion

        #region Mode
        public bool Mode => parser.Mode;
        #endregion

        #region Efficiency
        public string Efficiency => $"Эффективность: {parser.Efficiency} г/Вт";
        #endregion

        #region Power
        public string Power => $"Мощность: {parser.Power} Вт";
        #endregion

        #region Volume
        public String Volume
        {
            get => $"Громкость: {parser.Volume} дБ";
        }
        #endregion

        #region Plot
        private Plotter plotter { get; set; }
        public PlotModel Plot => plotter.Plot;
        #endregion

        #region Loading
        private bool loading = false;

        public bool Loading
        {
            get => loading;
            set => Set(ref loading, value);
        }
        #endregion

        #endregion

        #region Commands

        #region OpenPortCommand
        public ICommand OpenPortCommand { get; }
        private bool CanOpenPortCommandExecute(Object p) => true;
        private void OnOpenPortCommandExecuted(Object p)
        {
            Loading = true;
            if (!parser.Open())
            {
                SystemSounds.Asterisk.Play();
                MessageBox.Show("Ошибка подключения");
            }
            OnPropertyChanged("IsOpen");
            parser.Tare();
            parser.UpdateSettings(Properties.Settings.Default.NumberOfBlades, Properties.Settings.Default.SensorsInterval, Properties.Settings.Default.SendInterval, Properties.Settings.Default.LCDInterval);
            plotter.Clear();
            saveData.Clear();
            Loading = false;
        }
        #endregion

        #region ClosePortCommand
        public ICommand ClosePortCommand { get; }
        private bool CanClosePortCommandExecute(Object p) => true;
        private void OnClosePortCommandExecuted(Object p)
        {
            Loading = true;
            parser.Close();
            OnPropertyChanged("IsOpen");
            Loading = false;
        }
        #endregion

        #region RefreshPortCommand
        public ICommand RefreshPortCommand { get; }
        private bool CanRefreshPortCommandExecute(Object p) => true;
        private void OnRefreshPortCommandExecuted(Object p)
        {
            OnPropertyChanged("PortNames");
        }
        #endregion

        #region TareCommand
        public ICommand TareCommand { get; }
        private bool CanTareCommandExecute(Object p) => true;
        private void OnTareCommandExecuted(Object p)
        {
            Loading = true;
            CheckConnection(parser.Tare());
            plotter.Clear();
            saveData.Clear();
            Loading = false;
        }
        #endregion

        #region OnOffMotorCommand
        public ICommand OnOffMotorCommand { get; }
        private bool CanOnOffMotorCommandExecute(Object p) => true;
        private void OnOffMotorCommandExecuted(Object p)
        {
            Loading = true;
            //parser.Motor = !parser.Motor;
            CheckConnection(parser.SetMotor(!parser.Motor));
            if (parser.Motor)
            {
                parser.Tare();
                saveData.Clear();
                plotter.Clear();
            }
            Loading = false;
        }
        #endregion

        #region AutoModeCommand
        public ICommand AutoModeCommand { get; }
        private bool CanAutoModeCommandExecute(Object p) => true;
        private void OnAutoModeCommandExecuted(Object p)
        {
            Loading  = true;
            if (!parser.Mode)
                parser.Motor = false;
            //parser.Mode = !parser.Mode;
            CheckConnection(parser.SetMode(!parser.Mode));
            Loading = false;
        }
        #endregion

        #region SaveCommand
        public ICommand SaveCommand { get; }
        private bool CanSaveCommandExecute(Object p) => true;
        private void OnSaveCommandExecuted(Object p)
        {
            switch (saveData.DialogSave())
            {
                case 3:
                    plotter.SavePDF(saveData.Path, Properties.Settings.Default.WidthPlot, Properties.Settings.Default.HeightPlot);
                    break;
                case 4:
                    plotter.SaveSVG(saveData.Path, Properties.Settings.Default.WidthPlot, Properties.Settings.Default.HeightPlot);
                    break;
            }
        }
        #endregion

        #region VisibleCommand
        public ICommand VisibleCommand { get; }
        private bool CanVisibleExecute(Object p) => true;
        private void OnVisibleCommandExecuted(Object p)
        {
            switch ((string)p)
            {
                case "Thrust":
                    plotter.IsVisibleThrust = !plotter.IsVisibleThrust;
                    break;
                case "Amperage":
                    plotter.IsVisibleAmperage = !plotter.IsVisibleAmperage;
                    break;
                case "Voltage":
                    plotter.IsVisibleVoltage = !plotter.IsVisibleVoltage;
                    break;
                case "Power":
                    plotter.IsVisiblePower = !plotter.IsVisiblePower;
                    break;
                case "Vibration":
                    plotter.IsVisibleVibration = !plotter.IsVisibleVibration;
                    break;
                case "Temp":
                    plotter.IsVisibleTemp = !plotter.IsVisibleTemp;
                    break;
                case "RPM":
                    plotter.IsVisibleRPM = !plotter.IsVisibleRPM;
                    break;
                case "Efficiency":
                    plotter.IsVisibleEfficiency = !plotter.IsVisibleEfficiency;
                    break;
                case "Volume":
                    plotter.IsVisibleVolume = !plotter.IsVisibleVolume;
                    break;
            }
        }
        #endregion

        #endregion

        public MainWindowVM()
        {
            parser = new StandParser();
            parser.DataReceived += Parser_DataReceived;
            App.Current.Exit += (object sender, ExitEventArgs e) => parser.Close();

            plotter = new Plotter();
            saveData = new SaveData();


            #region Commands
            OpenPortCommand = new RelayCommand(OnOpenPortCommandExecuted, CanOpenPortCommandExecute);
            ClosePortCommand = new RelayCommand(OnClosePortCommandExecuted, CanClosePortCommandExecute);
            RefreshPortCommand = new RelayCommand(OnRefreshPortCommandExecuted, CanRefreshPortCommandExecute);
            TareCommand = new RelayCommand(OnTareCommandExecuted, CanTareCommandExecute);
            OnOffMotorCommand = new RelayCommand(OnOffMotorCommandExecuted, CanOnOffMotorCommandExecute);
            AutoModeCommand = new RelayCommand(OnAutoModeCommandExecuted, CanAutoModeCommandExecute);
            SaveCommand = new RelayCommand(OnSaveCommandExecuted, CanSaveCommandExecute);
            VisibleCommand = new RelayCommand(OnVisibleCommandExecuted, CanVisibleExecute);
            #endregion
        }
        private void Parser_DataReceived(object sender, EventArgs e)
        {
            OnPropertyChanged("Thrust");
            OnPropertyChanged("Amperage");
            OnPropertyChanged("Voltage");
            OnPropertyChanged("Vibration");
            OnPropertyChanged("Temp");
            OnPropertyChanged("RPM");
            OnPropertyChanged("MotorValue");
            OnPropertyChanged("Message");
            OnPropertyChanged("Motor");
            OnPropertyChanged("Mode");
            OnPropertyChanged("Efficiency");
            OnPropertyChanged("Power");
            OnPropertyChanged("Volume");

            plotter.AddData(parser.characteristics, Properties.Settings.Default.SendInterval);
            saveData.AddData(parser.characteristics, Properties.Settings.Default.SendInterval);
        }
        private void CheckConnection(bool IsOpen)
        {
            if (!IsOpen)
            {
                SystemSounds.Asterisk.Play();
                MessageBox.Show("Нет соединения!");
                OnPropertyChanged("IsOpen");
            }
        }
    }
}
