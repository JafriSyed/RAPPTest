using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAPPTest
{
    public class PropertyChangeRequestEventArgs<T> : EventArgs
    {
        public PropertyChangeRequestEventArgs(T pRequestedValue)
        {
            RequestedValue = pRequestedValue;
        }

        public T RequestedValue { get; set; }
    }

    public class ChangeRequestEvents
    {
        public ChangeRequestEvents(Object sender)
        {
            m_pool = new Dictionary<Type, Dictionary<String, Object>>();
            m_sender = sender;
        }

        private Dictionary<Type, Dictionary<String, Object>> m_pool;
        private Object m_sender;

        public void RegisterListener<T>(String propertyName, EventHandler<PropertyChangeRequestEventArgs<T>> handler)
        {
            Type t = typeof(T);

            if (!m_pool.ContainsKey(t))
                m_pool.Add(t, new Dictionary<String, Object>());

            Dictionary<String, Object> events = m_pool[t];

            if (!events.ContainsKey(propertyName))
                events.Add(propertyName, new EventHolder<T>());

            EventHolder<T> holder = events[propertyName] as EventHolder<T>;

            holder.OnEvent += handler;
        }

        public void UnRegisterListener<T>(String propertyName, EventHandler<PropertyChangeRequestEventArgs<T>> handler)
        {
            Type t = typeof(T);

            if (!m_pool.ContainsKey(t))
                return;

            Dictionary<String, Object> events = m_pool[t];

            if (!events.ContainsKey(propertyName))
                return;

            EventHolder<T> holder = events[propertyName] as EventHolder<T>;

            holder.OnEvent -= handler;
        }

        public void Fire<T>(String propertyName, T requestedValue)
        {
            Type t = typeof(T);

            if (!m_pool.ContainsKey(t))
                return;

            Dictionary<String, Object> events = m_pool[t];

            if (!events.ContainsKey(propertyName))
                return;

            EventHolder<T> holder = events[propertyName] as EventHolder<T>;

            holder.Fire(m_sender, requestedValue);
        }

        private class EventHolder<T>
        {
            private event EventHandler<PropertyChangeRequestEventArgs<T>> m_onEvent;

            public void Fire(Object sender, T requestedValue)
            {
                if (null != m_onEvent)
                    m_onEvent(sender, new PropertyChangeRequestEventArgs<T>(requestedValue));
            }

            public event EventHandler<PropertyChangeRequestEventArgs<T>> OnEvent
            {
                add { m_onEvent += value; }
                remove { m_onEvent -= value; }
            }
        }

    }
}
