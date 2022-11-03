// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Dialogs
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();

            txtBuildDate.Text = $"{BuildInfo.BuildDateUtc:f}  (UTC)";
        }

        #region License click
        /// <summary>
        /// Handles the Click event of the BtnLicense control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs" /> instance containing the event data.
        /// </param>
        private void BtnLicense_Click(object sender, RoutedEventArgs e)
        {
            string dir = AppInfo.AppDirectory;
            TextFileViewer.ViewTextFile(Path.Combine(dir, "License.txt"));
        }
        #endregion License click

        #region URL click
        /// <summary>
        /// Called when [navigate].
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="RequestNavigateEventArgs" /> instance containing the event data.
        /// </param>
        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process p = new();
            p.StartInfo.FileName = e.Uri.AbsoluteUri;
            p.StartInfo.UseShellExecute = true;
            p.Start();
            e.Handled = true;
        }
        #endregion URL click

        #region ReadMe click
        /// <summary>
        /// Handles the Click event of the BtnReadMe control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="RoutedEventArgs" /> instance containing the event data.
        /// </param>
        private void BtnReadMe_Click(object sender, RoutedEventArgs e)
        {
            string dir = AppInfo.AppDirectory;
            TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
        }
        #endregion ReadMe click
    }
}
