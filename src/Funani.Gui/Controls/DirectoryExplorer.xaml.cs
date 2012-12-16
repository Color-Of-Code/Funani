namespace Funani.Gui.Controls
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;

	/// <summary>
	/// Interaction logic for FileExplorer.xaml
	/// </summary>
	public partial class DirectoryExplorer : UserControl
	{
		public DirectoryExplorer()
		{
			InitializeComponent();
		}

		public void SelectPath(string path)
		{
			SelectedPath = path;
		}
		
		private object dummyNode = null;

		public string SelectedPath 
		{
			get { return (string)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}
		
		public static readonly DependencyProperty SelectedPathProperty = 
  			DependencyProperty.Register("SelectedPath", typeof(string), typeof(DirectoryExplorer), 
  			new PropertyMetadata(string.Empty));
  

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (string s in Directory.GetLogicalDrives())
			{
				TreeViewItem item = new TreeViewItem();
				item.Header = s;
				item.Tag = new DirectoryInfo(s);
				item.FontWeight = FontWeights.Normal;
				item.Items.Add(dummyNode);
				item.Expanded += new RoutedEventHandler(folder_Expanded);
				foldersItem.Items.Add(item);
			}
		}

		private void folder_Expanded(object sender, RoutedEventArgs e)
		{
			TreeViewItem item = (TreeViewItem)sender;
			if (item.Items.Count == 1 && item.Items[0] == dummyNode)
			{
				item.Items.Clear();
				try
				{
					var di = item.Tag as DirectoryInfo;
					foreach (var d in di.GetDirectories())
					{
						if ((d.Attributes & FileAttributes.Hidden) != 0)
							continue;
						
						TreeViewItem subitem = new TreeViewItem();
						subitem.Header = d.Name;
						subitem.Tag = d;
						subitem.FontWeight = FontWeights.Normal;
						subitem.Items.Add(dummyNode);
						subitem.Expanded += new RoutedEventHandler(folder_Expanded);
						item.Items.Add(subitem);
					}
				}
				catch (Exception) { }
			}
		}

		private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeView tree = (TreeView)sender;
			TreeViewItem item = (TreeViewItem)tree.SelectedItem;
			var di = item.Tag as DirectoryInfo;
			if (di != null)
				SelectedPath = di.FullName;
		}
	}
}