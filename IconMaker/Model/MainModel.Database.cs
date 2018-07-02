using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;

namespace IconMaker.Model
{
    public partial class MainModel
    {
        public static readonly XNamespace NSIconMaker = "urn:IconMaker/Database";
        public static readonly XNamespace NSXAML = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public XElement LibrariesInDatabase { get; set; }

        private void DoDatabaseUpdate()
        {
            LibrariesInDatabase = new XElement(NSIconMaker + "Libraries");

            ProgressDialog.ProgressDialog.Current.Report("Counting icons...");

            int numIcons = CountIcons(App.IconLibPath);

            ProgressDialog.ProgressDialog.Current.Report("Scanning icons...", min: 0, max: numIcons, value: 0);

            ScanIcons(App.IconLibPath);

            foreach (XElement eltLibrary in LibrariesInDatabase.Elements(NSIconMaker + "Library"))
            {
                List<XElement> children = new List<XElement>(eltLibrary.Elements());
                eltLibrary.RemoveNodes();
                children.Sort(LibraryChildrenSorter);
                foreach (XElement child in children)
                    eltLibrary.Add(child);
            }

            string databaseXml = Path.Combine(App.IconLibPath, "database.xml");

            XDocument database = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                LibrariesInDatabase);
            using (XmlWriter xmlWriter = XmlWriter.Create(databaseXml, new XmlWriterSettings { Indent = true }))
                database.WriteTo(xmlWriter);
        }

        private int LibraryChildrenSorter(XElement x, XElement y)
        {
            if (x.Name != y.Name)
                return x.Name.LocalName == "Overlay" ? -1 : 1;

            if (x.Name.LocalName == "Overlay")
                return string.Compare(x.Attribute("title")?.Value, y.Attribute("title")?.Value, StringComparison.Ordinal);

            return string.Compare(x.Attribute("name")?.Value, y.Attribute("name")?.Value, StringComparison.Ordinal);
        }

        private void ScanIcons(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
                ScanIcons(directory);

            foreach (string file in Directory.GetFiles(path, "*.xaml"))
            {
                LoadIcon(file);
                ProgressDialog.ProgressDialog.Current.Report(value: double.PositiveInfinity);
            }
        }

        private void LoadIcon(string xamlFileName)
        {
            if (!File.Exists(xamlFileName))
                return;

            string relativeFileName = xamlFileName.Substring(App.IconLibPath.Length + 1);

            XmlDocument xaml = new XmlDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xaml.NameTable);
            nsmgr.AddNamespace("p", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            xaml.Load(xamlFileName);

            if (xaml.SelectSingleNode("//p:Canvas[@Uid = 'Icon']", nsmgr) == null)
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

            XElement eltXaml = XElement.Parse(xaml.DocumentElement.OuterXml);

            if (xaml.SelectSingleNode("//p:Canvas[@Uid = 'Overlay']", nsmgr) != null)
                AddOverlayToDatabase(relativeFileName, eltXaml);
            else if (xaml.SelectSingleNode("//p:Canvas[@Uid = 'Icon']", nsmgr) != null)
                AddIconToDatabase(relativeFileName, eltXaml);
        }

        private void AddIconToDatabase(string relativeFileName, XElement eltXaml)
        {
            string[] parts = relativeFileName.Split(@"\".ToCharArray(), 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return;

            string libraryName = parts[0];
            XElement library = FindLibraryInDatabase(libraryName);

            string categoryName = parts[1];
            XElement category = FindCategoryInDatabase(library, categoryName);

            string iconName = new string(parts[2].TakeWhile(c => c != '.').ToArray());
            XElement icon = FindIconInDatabase(category, iconName);

            icon.Add(eltXaml);
        }

        private void AddOverlayToDatabase(string relativeFileName, XElement viewbox)
        {
            string[] parts = relativeFileName.Split(@"\".ToCharArray(), 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                return;

            string libraryName = parts[0];
            XElement library = FindLibraryInDatabase(libraryName);

            string iconName = new string(parts[2].TakeWhile(c => c != '.').ToArray());
            parts = iconName.Split("_".ToCharArray());
            if (parts.Length != 2)
                return;

            string overlayName = parts[0];
            XElement overlay = FindOverlayInDatabase(library, overlayName);

            overlay.Add(new XElement(NSIconMaker + parts[1], viewbox));
        }

        private XElement FindLibraryInDatabase(string libraryName)
        {
            XElement library = LibrariesInDatabase
                .Elements(NSIconMaker + "Library")
                .FirstOrDefault(e => e.Attribute("name")?.Value == libraryName);
            if (library != null)
                return library;

            library = new XElement(NSIconMaker + "Library",
                new XAttribute("name", libraryName));
            LibrariesInDatabase.Add(library);
            return library;
        }

        private static XElement FindOverlayInDatabase(XContainer library, string overlayName)
        {
            XElement overlay = library
                .Elements(NSIconMaker + "Overlay")
                .FirstOrDefault(e => e.Attribute("title")?.Value == overlayName);
            if (overlay != null)
                return overlay;

            overlay = new XElement(NSIconMaker + "Overlay",
                new XAttribute("title", overlayName));
            library.Add(overlay);
            return overlay;
        }

        private static XElement FindCategoryInDatabase(XContainer library, string categoryName)
        {
            XElement category = library
                .Elements(NSIconMaker + "Category")
                .FirstOrDefault(e => e.Attribute("name")?.Value == categoryName);
            if (category != null)
                return category;

            category = new XElement(NSIconMaker + "Category",
                new XAttribute("name", categoryName));
            library.Add(category);
            return category;
        }

        private static XElement FindIconInDatabase(XContainer category, string iconName)
        {
            XElement icon = category
                .Elements(NSIconMaker + "Icon")
                .FirstOrDefault(e => e.Attribute("title")?.Value == iconName);
            if (icon != null)
                return icon;

            icon = new XElement(NSIconMaker + "Icon",
                new XAttribute("title", iconName));
            category.Add(icon);
            return icon;
        }

        private static bool FixIconXaml(XmlNode xaml, XmlNamespaceManager nsmgr)
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

        private static int CountIcons(string path)
        {
            int count = Directory.GetDirectories(path).Sum(CountIcons);
            return count + Directory.GetFiles(path, "*.xaml").Length;
        }
    }
}