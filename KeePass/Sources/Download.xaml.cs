using System;
using System.Windows;
using System.Windows.Navigation;
using KeePass.I18n;
using KeePass.Storage;
using KeePass.Utils;
using Microsoft.Phone.Controls;
using Windows.Phone.Storage.SharedAccess;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Threading.Tasks;
using System.IO;

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

        protected async override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;

            lnkDemo.Visibility = Visibility.Collapsed;

            ApplicationBar.IsVisible = true;
            _folder = NavigationContext.QueryString["folder"];

            if (NavigationContext.QueryString.ContainsKey("fileToken"))
            {
                string fileID = NavigationContext.QueryString["fileToken"];
                string incomingFileName = SharedStorageAccessManager.GetSharedFileName(fileID);
                string msg = "Please confirm opening database file '" + incomingFileName + "'";
                if (MessageBox.Show(msg, "Open database file", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    await loadExternalFile(fileID);
                }
            }
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

        private async Task loadExternalFile(string fileID)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.CreateDirectory("temp");
            }
            StorageFolder tempFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync("temp");
            // Get the file name.
            string incomingFileName = SharedStorageAccessManager.GetSharedFileName(fileID);
            var file = await SharedStorageAccessManager.CopySharedFileAsync(tempFolder, incomingFileName, NameCollisionOption.ReplaceExisting, fileID);
            var info = new DatabaseInfo();
            var randAccessStream = await file.OpenReadAsync();
            info.SetDatabase(randAccessStream.AsStream(), new DatabaseDetails
            {
                Source = "ExternalApp",
                Name = incomingFileName.RemoveKdbx(),
                Type = SourceTypes.OneTime,
            });
            this.NavigateTo<MainPage>();
            // Don't go back to load new DB
            NavigationService.RemoveBackEntry();
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
