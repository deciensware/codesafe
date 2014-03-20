using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using KeePass.Data;
using KeePass.I18n;
using KeePass.Storage;
using KeePass.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace KeePass
{
    public partial class WebView
    {
        private EntryBinding _entry;

        public WebView()
        {
            InitializeComponent();

            AppButton(0).Text = Strings.Refresh;
            AppButton(1).Text = Strings.WebView_GoBack;
            AppButton(2).Text = Strings.WebView_Username;
            AppButton(3).Text = Strings.WebView_Password;
        }

        protected override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled || _entry != null)
                return;

            var database = Cache.Database;
            if (database == null)
            {
                this.BackToDBs();
                return;
            }

            _entry = CurrentEntry.Entry;

            foreach (var field in _entry.GetFields().Take(3))
            {
                var local = field;

                var item = new ApplicationBarMenuItem(local.Name);
                item.Click += (s, _) => SetValue(local.Value);

                ApplicationBar.MenuItems.Add(item);
            }
        }

        private void SetValue(string value)
        {
            try
            {
                browser.Focus();

                value = value.Replace(@"\", @"\\");
                var script = string.Format(
                    "document.activeElement.value='{0}'", value);
                browser.InvokeScript("eval", script);
            }
            catch (Exception)
            {
                lblOverlay.Text = value;
                vwOverlay.Visibility = Visibility.Visible;
            }
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            progBusy.IsIndeterminate = false;
            progBusy.Visibility = Visibility.Collapsed;
        }

        private void browser_Loaded(object sender, RoutedEventArgs e)
        {
            var url = new Uri(
                NavigationContext.QueryString["url"],
                UriKind.Absolute);

            browser.Navigate(url);
        }

        private void browser_Navigated(object sender, NavigationEventArgs e)
        {
            progBusy.IsIndeterminate = true;
            progBusy.Visibility = Visibility.Visible;
        }

        private void cmdBack_Click(object sender, EventArgs e)
        {
            try
            {
                browser.InvokeScript("eval", "history.go(-1)");
            }
            catch {}
        }

        private void cmdPassword_Click(object sender, EventArgs e)
        {
            SetValue(_entry.Password);
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                browser.InvokeScript("eval", "history.go()");
            }
            catch
            {
                var uri = browser.Source;

                if (uri != null)
                    browser.Navigate(uri);
            }
        }

        private void cmdUser_Click(object sender, EventArgs e)
        {
            SetValue(_entry.UserName);
        }

        private void vwOverlay_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction != System.Windows.Controls.Orientation.Vertical)
                return;

            if (e.VerticalVelocity >= 0)
                return;

            e.Handled = true;
            vwOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
