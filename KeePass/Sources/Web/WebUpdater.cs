﻿using System;
using System.IO;
using System.Net;
using KeePass.IO.Utils;
using KeePass.Storage;

namespace KeePass.Sources.Web
{
    internal static class WebUpdater
    {
        public static void Update(DatabaseInfo info,
            Func<DatabaseInfo, bool> queryUpdate,
            ReportUpdateResult report)
        {
            if (info == null) throw new ArgumentNullException("info");
            if (queryUpdate == null) throw new ArgumentNullException("queryUpdate");
            if (report == null) throw new ArgumentNullException("report");

            var details = info.Details;
            var urlInfo = WebUtils.Deserialize(details.Url);
            var credentials = WebUtils.CreateCredentials(
                urlInfo.User, urlInfo.Password, urlInfo.Domain);

            WebUtils.Download(urlInfo.Url, credentials, (req, getResponse) =>
            {
                if (!queryUpdate(info))
                    return;

                HttpWebResponse res;

                try
                {
                    res = getResponse();
                }
                catch (WebException ex)
                {
                    report(info, SyncResults.Failed, ex.Message);
                    return;
                }

                using (var buffer = new MemoryStream())
                {
                    using (var stream = res.GetResponseStream())
                    {
                        BufferEx.CopyStream(stream, buffer);
                        buffer.Position = 0;
                    }

                    var check = DatabaseVerifier
                        .VerifyUnattened(buffer);

                    if (check.Result == VerifyResultTypes.Error)
                    {
                        report(info, SyncResults.Failed,
                            check.Message);

                        return;
                    }

                    info.SetDatabase(buffer, details);
                    report(info, SyncResults.Downloaded, null);
                }
            });
        }
    }
}