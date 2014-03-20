using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Storage;
using KeePass.I18n;
using KeePass.IO.Data;
using KeePass.Sources.WebDav.Api;
using KeePass.Storage;
using KeePass.Utils;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace KeePass.Sources.SdCard
{
    public partial class SdCardList
    {
        // A collection of database files (.KDBX files) for binding to the ListBox.
        public ObservableCollection<ExternalStorageFile> Files { get; set; }

        public SdCardList()
        {
            InitializeComponent();
            AppButton(0).Text = Strings.Refresh;
            Files = new ObservableCollection<ExternalStorageFile>();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;
            RefreshList();
        }

        private async void RefreshList()
        {
            // Clear the collection bound to the page.
            Files.Clear();

            // Connect to the current SD card.
            ExternalStorageDevice _sdCard = (await ExternalStorage.GetExternalStorageDevicesAsync()).FirstOrDefault();

            // If the SD card is present, add KDBX files to the Files collection.
            if (_sdCard != null)
            {
                try
                {
                    // Look for a folder on the SD card named Files.
                    ExternalStorageFolder dbFolder = await _sdCard.GetFolderAsync("CodeSafe");

                    // Get all files from the CodeSafe folder.
                    IEnumerable<ExternalStorageFile> routeFiles = await dbFolder.GetFilesAsync();

                    // Add each KDBX file to the Files collection.
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        if (esf.Path.EndsWith(".kdbx"))
                        {
                            Files.Add(esf);
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    // No CodeSafe folder is present.
                    MessageBox.Show("The CodeSafe folder is missing on your SD card. Add a CodeSafe folder containing at least one .kdbx file and try again.");
                }
            }
            else
            {
                // No SD card is present.
                MessageBox.Show("The SD card is mssing. Insert an SD card that has a CodeSafe folder containing at least one .kdbx file and try again.");
            }
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private async Task LoadDatabaseFile(ExternalStorageFile file)
        {
            Stream stream = await file.OpenForReadAsync();

            var info = new DatabaseInfo();

            info.SetDatabase(stream, new DatabaseDetails
            {
                Name = file.Name.RemoveKdbx(),
                Type = SourceTypes.Updatable,
                Source = DatabaseUpdater.SDCARD_UPDATER,
            });
        }

        private async void lstBrowse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedItem != null)
            {
                try
                {
                    ExternalStorageFile esf = (ExternalStorageFile)lb.SelectedItem;
                    await LoadDatabaseFile(esf);
                }
                catch (FileNotFoundException)
                {
                    // The route is not present on the SD card.
                    MessageBox.Show("That file is missing on your SD card.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Error opening file on your SD card.");
                }

                Dispatcher.BeginInvoke(this.BackToDBs);
            }
        }
    }
}