using System;
using System.Windows;
using System.Windows.Navigation;
using KeePass.I18n;
using KeePass.Storage;
using KeePass.Utils;
using Microsoft.Phone.Controls;

namespace KeePass.Sources
{
    public partial class Download
    {
        private string _folder;

        public Download()
        {
            InitializeComponent();
            AppMenu(0).Text = Strings.Download_Sample;
        }

        protected override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;

            lnkDemo.Visibility = Visibility.Collapsed;

            ApplicationBar.IsVisible = true;
            _folder = NavigationContext.QueryString["folder"];
        }

        private void Navigate<T>()
            where T : PhoneApplicationPage
        {
            this.NavigateTo<T>("folder={0}", _folder);
        }

        private void lnkDemo_Click(object sender, EventArgs e)
        {
            var info = new DatabaseInfo();
            var demoDb = Application.GetResourceStream(
                new Uri("Sources/Sample.kdbx", UriKind.Relative));

            info.SetDatabase(demoDb.Stream, new DatabaseDetails
            {
                Source = "Sample",
                Name = "Sample Database",
                Type = SourceTypes.OneTime,
            });

            MessageBox.Show(
                Properties.Resources.DemoDbText,
                Properties.Resources.DemoDbTitle,
                MessageBoxButton.OK);

            this.BackToDBs();
        }

        private void lnkDropBox_Click(object sender, RoutedEventArgs e)
        {
            Navigate<DropBox.DropBoxAuth>();
        }

        private void lnkSkyDrive_Click(object sender, RoutedEventArgs e)
        {
            Navigate<SkyDrive.LiveAuth>();
        }

        private void lnkWebDav_Click(object sender, RoutedEventArgs e)
        {
            Navigate<WebDav.WebDavDownload>();
        }

        private void lnkWeb_Click(object sender, RoutedEventArgs e)
        {
            Navigate<Web.WebDownload>();
        }

        private void lnkSdCard_Click(object sender, RoutedEventArgs e)
        {
            Navigate<SdCard.SdCardList>();
        }
    }
}
