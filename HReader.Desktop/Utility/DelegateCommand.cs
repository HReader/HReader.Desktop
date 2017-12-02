using System;
using System.Windows.Input;

namespace HReader.Utility
{
    internal class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            execute();
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        public void TriggerCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
