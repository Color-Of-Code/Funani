
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

namespace Funani.Gui.Controls
{
	/// <summary>
	/// Interaction logic for FileView.xaml
	/// </summary>
	public partial class FileView : UserControl
	{
		public FileView()
		{
			InitializeComponent();
			
			Files = new ObservableCollection<FileInfo>();
			DataContext = this;
		}

		public string SelectedPath 
		{
			get { return (string)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}
		
		public void ReloadFiles()
		{
			Files.Clear();
			DirectoryInfo di = new DirectoryInfo(SelectedPath);
			try
			{
				var files = di.GetFiles();
				foreach (var file in files)
					Files.Add(file);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Could not get files", MessageBoxButton.OK);
			}
		}
		
		public ObservableCollection<FileInfo> Files
		{
			get;
			private set;
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