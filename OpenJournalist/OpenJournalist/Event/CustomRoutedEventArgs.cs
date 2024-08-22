using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenJournalist.Event
{
    public class CustomRoutedEventArgs<T> : RoutedEventArgs
    {
        public T Data { get; private set; }

        public CustomRoutedEventArgs()
        {
        }

        public CustomRoutedEventArgs(T data)
        {
            this.Data = data;
        }

        public CustomRoutedEventArgs(RoutedEvent routedEvent, T data) : base(routedEvent)
        {
            this.Data = data;
        }

        public CustomRoutedEventArgs(RoutedEvent routedEvent, object source, T data) : base(routedEvent, source)
        {
            this.Data= data;
        }
    }
}
