using KeePass.Storage;
using Microsoft.Phone.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePass.Utils;
using System.Windows;

namespace KeePass.Sources.SdCard
{
    internal static class SdCardUpdater
    {
        public static async void Update(DatabaseInfo info,
            Func<DatabaseInfo, bool> queryUpdate,
            ReportUpdateResult report)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (queryUpdate == null) throw new ArgumentNullException("queryUpdate");
            if (report == null) throw new ArgumentNullException("report");

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

                    bool found = false;
                    foreach (ExternalStorageFile esf in routeFiles)
                    {
                        if (esf.Name.RemoveKdbx() == info.Details.Name)
                        {
                            found = true;
                            Stream stream = await esf.OpenForReadAsync();

                            var check = DatabaseVerifier
                                .VerifyUnattened(stream);

                            if (check.Result == VerifyResultTypes.Error)
                            {
                                report(info, SyncResults.Failed,
                                    check.Message);
                                return;
                            }

                            info.SetDatabase(stream, info.Details);
                            report(info, SyncResults.Downloaded, null);

                            break;
                        }
                    }

                    if (!found)
                        MessageBox.Show("The original database file is not found in CodeSafe folder on your SD card anymore. Did you delete it?");
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
    }
}
