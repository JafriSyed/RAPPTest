using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RAPPTest.Utilities
{
    public static class Utilities
    {
        public static ObservableCollection<int> GetTimerList()
        {
            ObservableCollection<int> timerList = new ObservableCollection<int>();
            for (int i = 1; i <= 10; i++)
            {
                timerList.Add(30 * i);
            }
            return timerList;
        }
    }
}
