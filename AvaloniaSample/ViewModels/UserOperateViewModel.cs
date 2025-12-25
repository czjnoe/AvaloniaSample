using DryIoc;
using Prism.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.ViewModels
{
    public partial class UserOperateViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        public Person SelectPeople { get; set; }

        public ObservableCollection<Person> Peoples { get; }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand<Person> EditCommand { get; }
        public DelegateCommand<Person> DeleteCommand { get; }

        public UserOperateViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            var people = new List<Person>
            {
                new Person("Neil",  55),
                new Person("Buzz", 38),
                new Person("James", 44)
            };
            Peoples = new ObservableCollection<Person>(people);
            AddCommand = new DelegateCommand(AddClick);
            EditCommand = new DelegateCommand<Person>(EditClick);
            DeleteCommand = new DelegateCommand<Person>(DeleteClick);
        }

        private void AddClick()
        {
            Peoples.Add(new Person());
        }

        private async void EditClick(Person data)
        {
            var parameters = new DialogParameters
            {
                { "data", data }
            };
            var result = await _dialogService.ShowDialogAsync("UserEditView", parameters);
            if (result.Result == ButtonResult.OK)
            {
                var editedData = result.Parameters.GetValue<Person>("data");
                var index = Peoples.IndexOf(data);
                if (index >= 0)
                {
                    Peoples[index] = editedData;
                }
            }
        }

        private void DeleteClick(Person data)
        {
            var index = Peoples.IndexOf(data);
            if (index >= -1)
            {
                Peoples.RemoveAt(index);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
