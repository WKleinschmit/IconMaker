using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        private static readonly HashSet<Task> taskCache = new HashSet<Task>();
        private static readonly object taskCacheLock = new object();

        private static void CacheTask(Task t)
        {
            lock (taskCacheLock)
                taskCache.Add(t);
        }

        private static void UncacheTask(Task t)
        {
            lock (taskCacheLock)
                taskCache.Remove(t);
        }

        internal static void WaitForIdle()
        {
            Task[] tasks;
            lock (taskCacheLock)
                tasks = taskCache.ToArray();
            Task.WaitAll(tasks);
        }

        internal static async Task DispatchAction(Action action)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(action);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
        }

        internal static async Task DispatchAction<T>(Action<T> action)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(action);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
        }

        internal static async Task DispatchAction<T1, T2>(Action<T1, T2> action)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(action);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
        }

        internal static async Task<TResult> DispatchFunc<TResult>(Func<TResult> func)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
            return (TResult)x.Result;
        }

        internal static async Task<TResult> DispatchFunc<T1, TResult>(Func<T1, TResult> func, T1 arg1)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func, arg1);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
            return (TResult)x.Result;
        }

        internal static async Task<TResult> DispatchFunc<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 arg1, T2 arg2)
        {
            DispatcherOperation x = Current.Dispatcher.BeginInvoke(func, arg1, arg2);
            CacheTask(x.Task);
            await x.Task;
            UncacheTask(x.Task);
            return (TResult)x.Result;
        }
    }

    internal static class Global
    {
        public static Uri MakePackUri(string relativeFile)
        {
            string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString);
        }
        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName != null)
                    return _assemblyShortName;

                Assembly a = typeof(Global).Assembly;
                _assemblyShortName = a.ToString().Split(',')[0];
                return _assemblyShortName;
            }
        }

        private static string _assemblyShortName;
    }
}
