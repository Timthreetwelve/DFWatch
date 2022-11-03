// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Pages;
/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : Page
{
    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    public MainPage()
    {
        InitializeComponent();
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Update property when enter is pressed
        if (e.Key == Key.Enter)
        {
            // https://stackoverflow.com/a/13289118
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
            binding?.UpdateSource();
        }
    }

    #region Folder pickers
    private void BtnDestSourcePicker_Click(object sender, RoutedEventArgs e)
    {
        using System.Windows.Forms.FolderBrowserDialog dialog = new();
        dialog.ShowNewFolderButton = true;
        if (!string.IsNullOrEmpty(tbxSource.Text))
        {
            dialog.SelectedPath = Path.GetDirectoryName(tbxSource.Text);
        }
        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK)
        {
            UserSettings.Setting.SourceFolder = dialog.SelectedPath;
        }
    }
    private void BtnDestFolderPicker_Click(object sender, RoutedEventArgs e)
    {
        using System.Windows.Forms.FolderBrowserDialog dialog = new();
        dialog.ShowNewFolderButton = false;
        if (!string.IsNullOrEmpty(tbxDestination.Text))
        {
            dialog.SelectedPath = Path.GetDirectoryName(tbxDestination.Text);
        }
        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK)
        {
            UserSettings.Setting.DesitinationFolder = dialog.SelectedPath;
        }
    }
    #endregion Folder pickers

    private void Tbx1_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddToList();
        }
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }

    private void Tbx1_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        string invalid = new string(Path.GetInvalidFileNameChars()) + ",;";
        invalid = invalid.Replace("*", "").Replace("?", "");
        if (invalid.Contains(e.Text[0]))
        {
            e.Handled = true;
        }
        if (e.Text == "." && tbx1.Text.Length > 0)
        {
            e.Handled = true;
        }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        AddToList();
    }

    private void BtnDel_Click(object sender, RoutedEventArgs e)
    {
        DeleteFromList();
    }

    #region Add extensions to the list
    private void AddToList()
    {
        if (!string.IsNullOrWhiteSpace(tbx1.Text))
        {
            FileExt newitem = new() { FileExtension = tbx1.Text };
            log.Debug($"Adding {newitem.FileExtension} to extension list");
            (Application.Current.MainWindow as MainWindow)?.DisappearingMessage($"{newitem.FileExtension} has been added");
            FileExt.ExtensionList.Add(newitem.FileExtension);
            SortExtList();
            int idx = lbxExtensions.Items.IndexOf(newitem.FileExtension);
            lbxExtensions.SelectedItem = lbxExtensions.Items[idx];
            lbxExtensions.ScrollIntoView(lbxExtensions.Items[idx]);
            tbx1.Clear();
            tbx1.Focus();
        }
    }
    #endregion Add extensions to the list

    #region Delete extensions from the list
    private void DeleteFromList()
    {
        System.Collections.IList tempCollection = lbxExtensions.SelectedItems;
        for (int i = tempCollection.Count; i > 0; i--)
        {
            log.Debug($"Deleting {tempCollection[i - 1]} from extension list");
            (Application.Current.MainWindow as MainWindow)?.DisappearingMessage($"{tempCollection[i - 1]} has been removed");
            FileExt.ExtensionList.Remove(tempCollection[i - 1].ToString());
        }
    }
    #endregion Delete extensions from the list

    #region Remove duplicates and sort the extension list
    private static void SortExtList()
    {
        List<string> x = FileExt.ExtensionList.Distinct().ToList();
        x.Sort();
        FileExt.ExtensionList.Clear();
        foreach (string item in x)
        {
            FileExt.ExtensionList.Add(item);
        }
    }
    #endregion Remove duplicates and sort the extension list

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        FileExt.ExtensionList = UserSettings.Setting.ExtensionList;
        lbxExtensions.ItemsSource = FileExt.ExtensionList;
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        Watch.StopWatcher();
    }

    private void BtnStart_Click_1(object sender, RoutedEventArgs e)
    {
        Watch.StartWatcher();
    }

    private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = !Watch.watcher.EnableRaisingEvents;
    }

    private void StartWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StartWatcher();
    }

    private void StopCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Watch.watcher.EnableRaisingEvents;
    }

    private void StopWatching_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Watch.StopWatcher();
    }
}
