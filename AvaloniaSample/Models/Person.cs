using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Models
{
    public class Person
    {
        public string Name { get; set; }

        public int AgeInYears { get; set; }

        public Person()
        {

        }

        public Person(string name, int ageInYears)
        {
            Name = name;
            AgeInYears = ageInYears;
        }
    }
}
