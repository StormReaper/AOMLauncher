using System;
using System.Reflection;
using System.Windows;

namespace AOMLauncher
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            LblVersion.Text = "Version " + version;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
