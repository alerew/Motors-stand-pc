using Motors_stand.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Motors_stand.ViewModel;

namespace Motors_stand.Infrastructure.Commands
{
    class SettingsCommand : BaseCommand
    {
        private SettingsWindow Window;
        public override bool CanExecute(object parameter) => Window == null;
        public override void Execute(object parameter)
        {
            var window = new SettingsWindow() { Owner = App.Current.MainWindow};
            Window = window;
            window.Closed += Window_Closed;
            window.DataContext = new SettingsWindowVM();

            window.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ((Window)sender).Closed -= Window_Closed;
            Window = null;
        }
    }
}
