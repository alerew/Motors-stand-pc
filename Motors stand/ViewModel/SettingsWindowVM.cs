using Motors_stand.Infrastructure;
using Motors_stand.Models;
using Motors_stand.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Motors_stand.ViewModel
{
    class SettingsWindowVM : BaseVM
    {
        public StandParser parser;

        #region Value

        public byte NumberOfBlades
        {
            get => Properties.Settings.Default.NumberOfBlades;
            set
            {
                Properties.Settings.Default.NumberOfBlades = value;
                Set();
            }
        }
        public UInt16 SensorsInterval
        {
            get => Properties.Settings.Default.SensorsInterval;
            set
            {
                Properties.Settings.Default.SensorsInterval = value;
                Set();
            }
        }
        public UInt16 SendInterval
        {
            get => Properties.Settings.Default.SendInterval;
            set
            {
                Properties.Settings.Default.SendInterval = value;
                Set();
            }
        }
        public UInt16 LCDInterval
        {
            get => Properties.Settings.Default.LCDInterval;
            set
            {
                Properties.Settings.Default.LCDInterval = value;
                Set();
            }
        }
        public int WidthPlot
        {
            get => Properties.Settings.Default.WidthPlot;
            set
            {
                Properties.Settings.Default.WidthPlot = value;
                Set();
            }
        }
        public int HeightPlot
        {
            get => Properties.Settings.Default.HeightPlot;
            set
            {
                Properties.Settings.Default.HeightPlot = value;
                Set();
            }
        }


        #endregion

        #region Commands
        public ICommand UpdateCommand { get; }
        private bool CanUpdateCommandExecute(Object p) => true;
        private void OnUpdateCommandExecuted(Object p)
        {
            Properties.Settings.Default.Save();
            if (!parser.UpdateSettings(Properties.Settings.Default.NumberOfBlades, Properties.Settings.Default.SensorsInterval, Properties.Settings.Default.SendInterval, Properties.Settings.Default.LCDInterval))
            {
                SystemSounds.Asterisk.Play();
                MessageBox.Show("Ошибка подключения");
            }
            else
            {
                MessageBox.Show("Настройки применены");
            }

        }
        #endregion

        public SettingsWindowVM()
        {
            parser = ((MainWindowVM)App.Current.MainWindow.DataContext).parser;

            UpdateCommand = new RelayCommand(OnUpdateCommandExecuted, CanUpdateCommandExecute);
        }
    }
}
