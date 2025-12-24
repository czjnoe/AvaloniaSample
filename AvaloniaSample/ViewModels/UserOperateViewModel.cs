using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.ViewModels
{
    public partial class UserOperateViewModel : ViewModelBase
    {
        private ObservableCollection<Person> _people;

        public ObservableCollection<Person> People
        {
            get => _people;
            set
            {
                _people = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand SaveCommand { get; }

        public UserOperateViewModel()
        {
            var people = new List<Person>
            {
                new Person("Neil", "Armstrong",  55),
                new Person("Buzz", "Lightyear", 38),
                new Person("James", "Kirk", 44)
            };
            People = new ObservableCollection<Person>(people);
            AddCommand = new DelegateCommand(AddClick);
            SaveCommand = new DelegateCommand(SaveClick);
            EditCommand = new DelegateCommand(EditClick);
        }

        private void SaveClick()
        {

        }

        private void AddClick()
        {
            People.Add(new Person("New", "Person", 25));
        }

        private void EditClick()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
