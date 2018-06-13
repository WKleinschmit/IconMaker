using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IconMaker
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        internal static string IconLibPath { get; }

        static App()
        {
            IconLibPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IconLib");
        }

        public App()
        {
        }

        internal static async Task DispatchAction(Action action)
        {
            await Current.Dispatcher.BeginInvoke(action);
        }

        internal static async Task DispatchAction<T>(Action<T> action)
        {
            await Current.Dispatcher.BeginInvoke(action);
        }

        internal static async Task DispatchAction<T1, T2>(Action<T1, T2> action)
        {
            await Current.Dispatcher.BeginInvoke(action);
        }

        internal static async Task<TResult> DispatchFunc<TResult>(Func<TResult> func)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func);
            await x.Task;
            return (TResult)x.Result;
        }

        internal static async Task<TResult> DispatchFunc<T1, TResult>(Func<T1, TResult> func, T1 arg1)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func, arg1);
            await x.Task;
            return (TResult)x.Result;
        }

        internal static async Task<TResult> DispatchFunc<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func, arg1, arg2);
            await x.Task;
            return (TResult)x.Result;
        }
    }
}
