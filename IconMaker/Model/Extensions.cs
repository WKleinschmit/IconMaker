using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IconMaker.Model
{
    internal static class Extensions
    {
        internal static async void InsertSorted<T>(this IList<T> list, T item)
        where T : IComparable<T>
        {
            int i;
            for (i = 0; i < list.Count; ++i)
            {
                if (list[i].CompareTo(item) > 0)
                    break;
            }

            await App.DispatchAction(()=> list.Insert(i, item));
        }
    }
}
