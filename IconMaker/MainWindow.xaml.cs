using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using IconMaker.Model;

namespace IconMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainModel _model;

        public MainWindow()
        {
            InitializeComponent();

            if (!(DataContext is MainModel model))
                throw new ApplicationException();

            _model = model;
        }

        private bool isInitialized;

        private async void MainWindow_OnActivated(object sender, EventArgs e)
        {
            if (isInitialized)
                return;

            Ready.Visibility = Visibility.Collapsed;
            LoadProgress.Visibility = Visibility.Visible;
            int numIcons = await CountIcons(App.IconLibPath);
            ProgressBar.Maximum = numIcons;
            ProgressBar.Value = 0;
            try
            {
                await ScanIcons(App.IconLibPath);
            }
            finally
            {
                Ready.Visibility = Visibility.Visible;
                LoadProgress.Visibility = Visibility.Collapsed;
            }

            isInitialized = true;
        }

        private async Task ScanIcons(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
                await ScanIcons(directory);

            foreach (string file in Directory.GetFiles(path, "*.xaml"))
            {
                await LoadIcon(file);
                await App.DispatchAction(() => ++ProgressBar.Value);
            }
        }

        private async Task LoadIcon(object obj)
        {
            if (!(obj is string xamlFileName) || !File.Exists(xamlFileName))
                return;

            string relativeFileName = xamlFileName.Substring(App.IconLibPath.Length + 7);

            XmlDocument xaml = new XmlDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xaml.NameTable);
            nsmgr.AddNamespace("p", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            xaml.Load(xamlFileName);

            if (!(xaml.SelectSingleNode("//p:Canvas[@Uid = 'Icon']", nsmgr) is XmlElement iconCanvas))
            {
                if (!FixIconXaml(xaml, nsmgr))
                    return;

                using (XmlWriter xmlWriter = XmlWriter.Create(xamlFileName, new XmlWriterSettings
                {
                    Indent = true,
                    NewLineChars = "\r\n",
                    IndentChars = "  ",
                }))
                {
                    xaml.WriteTo(xmlWriter);
                }
            }

            Viewbox viewbox;
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings{CloseOutput = false}))
                    xaml.WriteTo(xmlWriter);
                ms.Seek(0, SeekOrigin.Begin);
                
                viewbox = await App.DispatchFunc((s) => XamlReader.Load(s) as Viewbox, ms);
            }
            if (viewbox == null)
                return;

            _model.AddIcon(relativeFileName, viewbox);
        }

        private bool FixIconXaml(XmlDocument xaml, XmlNamespaceManager nsmgr)
        {
            if (xaml.SelectSingleNode("/p:Viewbox/p:Canvas/p:Canvas/p:Canvas", nsmgr) is XmlElement iconCanvasInOverlay)
            {
                if (xaml.SelectSingleNode("/p:Viewbox/p:Canvas/p:Canvas[2]", nsmgr) is XmlElement overlayCanvasInOverlay)
                {
                    iconCanvasInOverlay.SetAttribute("Uid", "Icon");
                    overlayCanvasInOverlay.SetAttribute("Uid", "Overlay");
                    foreach (XmlElement xmlElement in iconCanvasInOverlay.ChildNodes.OfType<XmlElement>().ToArray())
                    {
                        if (xmlElement.Name == "Canvas.RenderTransform")
                            continue;
                        iconCanvasInOverlay.RemoveChild(xmlElement);
                    }

                    return true;
                }
            }

            if (xaml.SelectSingleNode("/p:Viewbox/p:Canvas/p:Canvas", nsmgr) is XmlElement iconCanvasInIcon)
            {
                iconCanvasInIcon.SetAttribute("Uid", "Icon");

                return true;
            }

            return false;
        }

        private Task<int> CountIcons(string path)
        {
            List<Task<int>> taskList = new List<Task<int>>();

            foreach (string directory in Directory.GetDirectories(path))
                taskList.Add(CountIcons(directory));

            Task.WaitAll(taskList.OfType<Task>().ToArray());

            int numIcons = taskList.Select(t => t.Result).Aggregate(0, (i1, i2) => i1 + i2);

            numIcons += Directory.GetFiles(path, "*.xaml").Length;

            return Task.FromResult(numIcons);
        }
    }
}
