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

        public DelegateCommand AddCommand { get; }
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
            AddCommand = new DelegateCommand(AddClick);
            EditCommand = ReactiveCommand.CreateFromTask<Person>(EditClick);
            DeleteCommand = new DelegateCommand<Person>(DeleteClick);
        }

        private void AddClick()
        {
            ContainerLocator.Container.Resolve<UserAddView>().ShowDialog(App.Instance.MainWindow as Window);
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
