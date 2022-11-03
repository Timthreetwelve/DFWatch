// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

public class FileExt : INotifyPropertyChanged
{
    #region Observable collection
    public static ObservableCollection<string> ExtensionList { get; set; }
    #endregion Observable collection

    #region Properties
    public string FileExtension
    {
        get { return fileExtension; }
        set
        {
            if (value != null)
            {
                if (!value.StartsWith("."))
                {
                    value = string.Concat(".", value);
                }
                fileExtension = value;
                OnPropertyChanged();
            }
        }
    }
    private string fileExtension;
    #endregion Properties

    #region Property changed
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Debug.WriteLine($"{propertyName} has changed");
    }
    #endregion Property changed
}
