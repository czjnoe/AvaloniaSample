using AvaloniaSample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string Greeting { get; } = "Welcome to Avalonia!";


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

        public MainWindowViewModel()
        {
            var people = new List<Person>
            {
                new Person("Neil", "Armstrong",  55),
                new Person("Buzz", "Lightyear", 38),
                new Person("James", "Kirk", 44)
            };
            People = new ObservableCollection<Person>(people);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
