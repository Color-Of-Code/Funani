namespace Funani.Gui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Funani.Gui.Model;

    /// <summary>
	/// Interaction logic for FileView.xaml
	/// </summary>
	public partial class FileView : UserControl
	{
		public FileView()
		{
			InitializeComponent();
			
			DataContext = this;
		}

		public string SelectedPath 
		{
			get { return (string)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}
		
		public void ReloadFiles()
		{
            var provider = new FileViewModelProvider(SelectedPath);
            var items = new AsyncVirtualizingCollection<FileViewModel>(provider, 20, 10 * 1000);
            listControl.DataContext = items;
		}
		
		private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var fileView = d as FileView;
			fileView.ReloadFiles();
		}
		
		public static readonly DependencyProperty SelectedPathProperty = 
  			DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileView), 
  			new PropertyMetadata(string.Empty, OnSelectedPathChanged));
	}
}