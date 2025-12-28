using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Interfaces
{
    public interface IAutoStartService
    {
        bool IsEnabled();
        void Enable();
        void Disable();
    }
}
