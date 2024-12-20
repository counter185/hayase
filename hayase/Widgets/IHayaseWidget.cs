using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hayase.Widgets
{
    internal interface IHayaseWidget
    {
        void Tick(ulong timeSinceActivation);

        void OnHayaseActivated();
    }
}
