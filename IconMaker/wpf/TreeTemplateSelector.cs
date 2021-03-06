﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IconMaker.wpf
{
    public class TreeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && container is FrameworkElement fe)
            {
                return fe.TryFindResource(item.GetType()) as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
