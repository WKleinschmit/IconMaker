﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace IconMaker.wpf
{
    public class TextBlockExtension
    {
        public static bool GetRemoveEmptyRuns(DependencyObject obj)
        {
            return (bool)obj.GetValue(RemoveEmptyRunsProperty);
        }

        public static void SetRemoveEmptyRuns(DependencyObject obj, bool value)
        {
            obj.SetValue(RemoveEmptyRunsProperty, value);

            if (!value) return;

            if (obj is TextBlock tb)
            {
                tb.Loaded += Tb_Loaded;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static readonly DependencyProperty RemoveEmptyRunsProperty =
            DependencyProperty.RegisterAttached("RemoveEmptyRuns", typeof(bool),
                typeof(TextBlock), new PropertyMetadata(false));



        public static bool GetPreserveSpace(DependencyObject obj)
        {
            return (bool)obj.GetValue(PreserveSpaceProperty);
        }

        public static void SetPreserveSpace(DependencyObject obj, bool value)
        {
            obj.SetValue(PreserveSpaceProperty, value);
        }

        public static readonly DependencyProperty PreserveSpaceProperty =
            DependencyProperty.RegisterAttached("PreserveSpace", typeof(bool),
                typeof(Run), new PropertyMetadata(false));


        private static void Tb_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is TextBlock tb)) return;
            tb.Loaded -= Tb_Loaded;

            List<Inline> spaces = tb.Inlines.Where(a => a is Run run
                                               && run.Text == " "
                                               && !GetPreserveSpace(a)).ToList();
            spaces.ForEach(s => tb.Inlines.Remove(s));
        }
    }
}
