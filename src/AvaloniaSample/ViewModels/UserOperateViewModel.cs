using Avalonia.Controls;
using DryIoc;
using Prism.Dialogs;
using Prism.Ioc;
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

        private Person? _selectPeople;
        public Person? SelectPeople
        {
            get => _selectPeople;
            set => this.RaiseAndSetIfChanged(ref _selectPeople, value);
        }

        public ObservableCollection<Person> Peoples { get; }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
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
            AddCommand = ReactiveCommand.CreateFromTask(AddClick);
            EditCommand = ReactiveCommand.CreateFromTask<Person>(EditClick);
            DeleteCommand = new DelegateCommand<Person>(DeleteClick);
        }

        private async Task AddClick()
        {
            var parameters = new DialogParameters
            {
                { "data", new Person() }
            };
            var result = await _dialogService.ShowDialogAsync("UserAddView", parameters);
            if (result.Result == ButtonResult.OK)
            {
                var newData = result.Parameters.GetValue<Person>("data");
                if (newData != null && !string.IsNullOrWhiteSpace(newData.Name))
                {
                    Peoples.Add(newData);
                }
            }
        }

        private async Task EditClick(Person data)
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
            if (index >= 0)
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
