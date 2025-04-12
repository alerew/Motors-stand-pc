using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Motors_stand.ViewModel
{
    public class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public bool Set<T>(ref T field, T value, [CallerMemberName] string Name = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(Name);
            return true;
        }
        public void Set([CallerMemberName] string Name = "")
        {
            OnPropertyChanged(Name);
        }
    }
}
