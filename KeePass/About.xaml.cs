using System;
using System.Windows;
using Microsoft.Phone.Tasks;
using System.Reflection;

namespace KeePass
{
    public partial class About
    {
        public About()
        {
            InitializeComponent();

            var version = new Version(Assembly.GetExecutingAssembly().FullName.Split('=')[1].Split(',')[0]);

            lblVersion.Text = string.Format(
                lblVersion.Text, string.Format("{0}.{1}.{2} rev{3}", version.Major, version.Minor, version.Build, version.Revision));
        }

        private void lnkPurchase_Click(
            object sender, RoutedEventArgs e)
        {
            new MarketplaceDetailTask().Show();
        }

        private void lnkReview_Click(
            object sender, RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }
    }
}