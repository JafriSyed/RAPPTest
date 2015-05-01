using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAPPTest
{
    public abstract class View : IView
    {
        private ChangeRequestEvents changeRequestEvents;

        public View()
        {
            this.changeRequestEvents = null;
        }

        public ChangeRequestEvents ChangeRequestEvents
        {
            get { return changeRequestEvents; }
            set { changeRequestEvents = value; }
        }

        public void RegisterChangeRequestListener<T>(string propertyName, EventHandler<PropertyChangeRequestEventArgs<T>> handler)
        {
            changeRequestEvents.RegisterListener<T>(propertyName, handler);
        }

        public void UnRegisterChangeRequestListener<T>(string propertyName, EventHandler<PropertyChangeRequestEventArgs<T>> handler)
        {
            changeRequestEvents.UnRegisterListener<T>(propertyName, handler);
        }
    }
}
