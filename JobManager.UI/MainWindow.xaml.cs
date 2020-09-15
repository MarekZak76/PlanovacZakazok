using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace JobManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        JobsPage jobsPage;
        OfficesPage officesPage;

        public MainWindow()
        {
            InitializeComponent();

            jobsPage = new JobsPage();
            panelContent.Content = jobsPage;
            officesPage = new OfficesPage();

            sbUser.Content = Environment.UserName;
            sbComputer.Content = Environment.MachineName;
        }

        protected async override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            await OfficeList.Instance.LoadDataAsync();
            await JobList.Instance.LoadDataAsync();
            (officesPage.DataContext as OfficeWithJobsViewModel).UpdateMapAsync();
        }
        private async void Refresh(object sender, RoutedEventArgs e)
        {
            OfficeList.Instance.DataClient.ErrorMessage = string.Empty;
            OfficeList.Instance.Clear();

            JobList.Instance.DataClient.ErrorMessage = string.Empty;
            JobList.Instance.Clear();

            await OfficeList.Instance.LoadDataAsync();
            JobList.Instance.LoadDataAsync();

        }
        private void Show_JobsPage(object sender, RoutedEventArgs e)
        {
            panelContent.Content = jobsPage;
        }
        private void Show_OfficesPage(object sender, RoutedEventArgs e)
        {
            panelContent.Content = officesPage;
        }
        private void Shutdown(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }       
    }
}
