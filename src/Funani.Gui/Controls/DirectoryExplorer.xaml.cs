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

            directories.DataContext = new DirectoryTreeViewModel();
		}

		public void SelectPath(string path)
		{
            var tree = directories.DataContext as DirectoryTreeViewModel;
            SelectedPath = path;
            tree.ExpandAndSelect(new DirectoryInfo(path));
		}

		public string SelectedPath 
		{
			get { return (string)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}
		
		public static readonly DependencyProperty SelectedPathProperty = 
  			DependencyProperty.Register("SelectedPath", typeof(string), typeof(DirectoryExplorer), 
  			new PropertyMetadata(string.Empty));
  

		private void directories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeView tree = (TreeView)sender;
            DirectoryViewModel item = (DirectoryViewModel)tree.SelectedItem;
			var di = item.DirectoryInfo;
			if (di != null)
				SelectedPath = di.FullName;
		}
	}
}