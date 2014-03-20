using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using KeePass.I18n;
using KeePass.Utils;
using Microsoft.Phone.Shell;

namespace KeePass.Sources.WebDav
{
    public partial class WebDavDownload
    {
        private readonly ApplicationBarIconButton _cmdDownload;
        private string _folder;

        public WebDavDownload()
        {
            InitializeComponent();
            
            _cmdDownload = AppButton(0);
            _cmdDownload.Text = Strings.WebDavDownload_Open;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            txtPassword.Password = string.Empty;
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(
            bool cancelled, NavigationEventArgs e)
        {
            if (cancelled)
                return;

            _folder = NavigationContext.QueryString["folder"];
        }

        private void PerformDownload()
        {
            this.NavigateTo<WebList>(
                "user={0}&pass={1}&folder={2}&path={3}",
                txtUser.Text, txtPassword.Password,
                _folder, txtUrl.Text);
        }

        private void ValidateData()
        {
            var isValidUrl = UrlUtils
                .IsValidUrl(txtUrl.Text);

            var hasData = isValidUrl &&
                txtUser.Text.Length > 0;

            _cmdDownload.IsEnabled = hasData;
        }

        private void cmdBoxNet_Click(object sender, EventArgs e)
        {
            txtUrl.Text = "https://dav.box.com/dav";
            txtUser.Focus();
        }

        private void cmdDownload_Click(object sender, EventArgs e)
        {
            PerformDownload();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsEnter() && _cmdDownload.IsEnabled)
                PerformDownload();
        }

        private void txtUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsEnter())
                txtUser.Focus();
        }

        private void txtUrl_Loaded(object sender, RoutedEventArgs e)
        {
            txtUrl.Focus();
            txtUrl.SelectionStart = txtUrl.Text.Length;
        }

        private void txtUrl_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            ValidateData();
        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsEnter())
                txtPassword.Focus();
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateData();
        }
    }
}
