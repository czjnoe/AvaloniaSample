using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Models
{
    public class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private int _ageInYears;

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int AgeInYears
        {
            get => _ageInYears;
            set
            {
                if (_ageInYears != value)
                {
                    _ageInYears = value;
                    OnPropertyChanged();
                }
            }
        }

        public Person(string firstName, string lastName, int ageInYears)
        {
            FirstName = firstName;
            LastName = lastName;
            AgeInYears = ageInYears;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
