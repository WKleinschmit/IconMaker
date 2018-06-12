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
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static string IconLibPath { get; }

        static App()
        {
            IconLibPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IconLib");
        }

        public App()
        {
            Stream iconsZipStream = GetType().Assembly.GetManifestResourceStream("IconMaker.Icons.zip");
            if (iconsZipStream == null)
                throw new ApplicationException();

            ZipArchive iconsZip = new ZipArchive(iconsZipStream);
            {
                foreach (ZipArchiveEntry entry in iconsZip.Entries)
                {
                    string fullName = Path.Combine(IconLibPath, entry.FullName);
                    if (entry.Length == 0)
                        Directory.CreateDirectory(fullName);
                    else
                    {
                        byte[] buffer = new byte[40960];
                        using (FileStream fileStream = File.Open(fullName, FileMode.Create))
                        {
                            using (Stream zipStream = entry.Open())
                            {
                                int count;
                                do
                                {
                                    count = zipStream.Read(buffer, 0, buffer.Length);
                                    fileStream.Write(buffer, 0, count);
                                } while (count > 0);
                            }
                        }
                    }
                }
            }
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
