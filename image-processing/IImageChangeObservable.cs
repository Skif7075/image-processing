using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace image_processing
{
    interface IImageChangeObservable
    {
        void RegisterObserver(IObserver observer);
        void NotifyObservers();
    }
}
