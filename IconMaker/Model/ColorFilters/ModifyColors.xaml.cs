using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IconMaker.Model.ColorFilters
{
    /// <summary>
    /// Interaction logic for ModifyColors.xaml
    /// </summary>
    public partial class ModifyColors : Window
    {
        public ModifyColors()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ColorMap ColorMap { get; } = new ColorMap();

        public void SetEntries(IList selectedColorMapEntries)
        {
            foreach (ColorMapEntry entry in selectedColorMapEntries)
            {
                ColorMap.Add(entry);
            }
        }
    }
}
