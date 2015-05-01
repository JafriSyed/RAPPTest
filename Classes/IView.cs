using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAPPTest
{
    public interface IView
    {
        void RegisterChangeRequestListener<T>(String propertyName,
            EventHandler<PropertyChangeRequestEventArgs<T>> handler);

        void UnRegisterChangeRequestListener<T>(String propertyName,
            EventHandler<PropertyChangeRequestEventArgs<T>> handler);
    }
}
