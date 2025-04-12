using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motors_stand.Infrastructure
{
    class RelayCommand : BaseCommand
    {
        private readonly Action<Object> _Execute;
        private readonly Func<Object, bool> _CanExecute;
        public RelayCommand(Action<Object> Execute, Func<Object, bool> CanExecute = null)
        {
            _Execute = Execute ?? throw new ArgumentNullException(nameof(Execute));
            _CanExecute = CanExecute;
        }
        public override bool CanExecute(Object parameter) => _CanExecute?.Invoke(parameter) ?? true;
        public override void Execute(Object parameter) => _Execute(parameter);
    }
}
