using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace AvaloniaSample.ViewModels
{
    public partial class UserAddViewModel : ViewModelBase, IDialogAware
    {
        public Person CurrentData { get; private set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private int _age;
        public int Age
        {
            get => _age;
            set
            {
                _age = value;
                RaisePropertyChanged(nameof(Age));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public DialogCloseListener RequestClose { get; set; }

        public UserAddViewModel()
        {
            SaveCommand = new DelegateCommand(SaveClick);
            CancelCommand = new DelegateCommand(CancelClick);
            Name = string.Empty;
            Age = 0;
        }

        public void SaveClick()
        {
            CurrentData = new Person
            {
                Name = this.Name,
                AgeInYears = this.Age
            };
            var parameters = new DialogParameters
            {
                { "data", CurrentData }
            };
            RequestClose.Invoke(parameters, ButtonResult.OK);
        }

        public void CancelClick()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var data = parameters.GetValue<Person>("data");
            if (data != null)
            {
                CurrentData = data;
                Name = CurrentData.Name;
                Age = CurrentData.AgeInYears;
            }
            else
            {
                // 添加新用户时，使用空值
                Name = string.Empty;
                Age = 0;
            }
        }
    }
}
