using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DG.TwitterClient.WpfHost.Helpers
{
    public static class VisualHelper
    {
        public static FrameworkElement FindElementByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0)
            {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        public static FrameworkElement FindElementByNameFromRow(string elementName, ListView listView, int rowIndex)
        {
            var lvi = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.Items[rowIndex]);

            return VisualHelper.FindElementByName(elementName, lvi);
        }

        public static ListViewItem GetListViewItem(ListView listView, int rowIndex)
        {
            return (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.Items[rowIndex]);
        }

        public static void Rebind(ListView listView, int rowIndex, DependencyProperty property)
        {
            var lvi = VisualHelper.GetListViewItem(listView, rowIndex);
            var be = lvi.GetBindingExpression(property);

            if (be != null)
            {
                be.UpdateTarget();
            }
        }

        public static void BindTarget(ListView listView, DependencyProperty property)
        {
            for (int rowIndex = 0; rowIndex < listView.Items.Count; rowIndex++)
            {
                var lvi = VisualHelper.GetListViewItem(listView, rowIndex);

                if (lvi != null)
                {
                    var be = lvi.GetBindingExpression(property);

                    if (be != null)
                    {
                        be.UpdateTarget();
                    }
                }
            }
        }

        public static object FindParent(object child, Type parentType)
        {
            if (child != null)
            {
                var childType = child.GetType();

                if (childType.Equals(parentType))
                {
                    return child;
                }

                var p = childType.GetProperty("Parent");

                return FindParent(p.GetValue(child, null), parentType);
            }

            return null;
        }

        public static object GetDataContext(object element)
        {
            if (element != null)
            {
                var p = element.GetType().GetProperty("DataContext");

                return p.GetValue(element, null);
            }

            return null;
        }
    }
}
