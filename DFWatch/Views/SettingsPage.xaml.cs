// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.Views;
/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    #region TextBox events
    /// <summary>Handles the PreviewKeyDown event of the Tbx1 control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
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

    /// <summary>Handles the PreviewTextInput event of the Tbx1 control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="TextCompositionEventArgs" /> instance containing the event data.</param>
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
    /// <summary>Handles the KeyDown event of the TextBox control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
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
    #endregion TextBox events

    #region Button events
    /// <summary>Handles the Click event of the BtnAdd control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        AddToList();
    }

    /// <summary>Handles the Click event of the BtnDel control. </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnDel_Click(object sender, RoutedEventArgs e)
    {
        DeleteFromList();
    }

    /// <summary>Handles the Click event of the BtnSave control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        ConfigHelpers.SaveSettings();
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Settings Saved");
    }

    /// <summary>Handles the Click event of the BtnOpen control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        string settingsFile = ConfigHelpers.SettingsFileName;
        TextFileViewer.ViewTextFile(settingsFile);
    }
    #endregion Button events

    #region Folder pickers
    /// <summary>Handles the Click event of the BtnDestSourcePicker control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnDestSourcePicker_Click(object sender, RoutedEventArgs e)
    {
        VistaFolderBrowserDialog dialog = new()
        {
            ShowNewFolderButton = true,
            Description = "Select a folder",
            UseDescriptionForTitle = true
        };

        if (!string.IsNullOrEmpty(tbxSource.Text))
        {
            dialog.SelectedPath = Path.GetDirectoryName(tbxSource.Text);
        }
        if (dialog.ShowDialog() == true)
        {
            UserSettings.Setting.SourceFolder = dialog.SelectedPath;
        }
    }

    /// <summary>Handles the Click event of the BtnDestFolderPicker control.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
    private void BtnDestFolderPicker_Click(object sender, RoutedEventArgs e)
    {
        VistaFolderBrowserDialog dialog = new()
        {
            ShowNewFolderButton = false,
            Description = "Select a folder",
            UseDescriptionForTitle = true
        };
        if (!string.IsNullOrEmpty(tbxDestination.Text))
        {
            dialog.SelectedPath = Path.GetDirectoryName(tbxDestination.Text);
        }
        if (dialog.ShowDialog() == true)
        {
            UserSettings.Setting.DestinationFolder = dialog.SelectedPath;
        }
    }
    #endregion Folder pickers

    #region Add extensions to the list
    /// <summary>Adds to the extension list.</summary>
    private void AddToList()
    {
        if (!string.IsNullOrWhiteSpace(tbx1.Text))
        {
            FileExt newitem = new() { FileExtension = tbx1.Text.ToLower() };
            NLogHelpers.Log.Debug($"Adding {newitem.FileExtension} to extension list");
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
    /// <summary>Deletes from the extension list.</summary>
    private void DeleteFromList()
    {
        System.Collections.IList tempCollection = lbxExtensions.SelectedItems;
        for (int i = tempCollection.Count; i > 0; i--)
        {
            NLogHelpers.Log.Debug($"Deleting {tempCollection[i - 1]} from extension list");
            (Application.Current.MainWindow as MainWindow)?.DisappearingMessage($"{tempCollection[i - 1]} has been removed");
            FileExt.ExtensionList.Remove(tempCollection[i - 1].ToString());
        }
    }
    #endregion Delete extensions from the list

    #region Remove duplicates and sort the extension list
    /// <summary>Removes duplicate entries and sorts the extension list.</summary>
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

    #region Loaded Event
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        FileExt.ExtensionList = UserSettings.Setting.ExtensionList;
        lbxExtensions.ItemsSource = FileExt.ExtensionList;
    }
    #endregion Loaded Event
}
