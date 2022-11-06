// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        SingleInstanceCheck();
    }
    /// <summary>
    /// Make sure that only a single instance of the application can run.
    /// If another instance is started bring that window to the front.
    /// <para/>
    /// Adapted from https://stackoverflow.com/a/23730146/15237757
    /// </summary>
    public void SingleInstanceCheck()
    {
        string UniqueEventName = "{5C3AE823-1FF6-4472-9EB6-A4590731A2CD}" + AppInfo.AppName;
        string UniqueMutexName = AppInfo.AppName + "_Mutex";
        Mutex Mutex = new(true, UniqueMutexName, out bool isOnlyInstance);
        EventWaitHandle eventWaitHandle = new(false, EventResetMode.AutoReset, UniqueEventName);
        GC.KeepAlive(Mutex);

        if (isOnlyInstance)
        {
            // Spawn a thread which will be waiting for our event
            Thread thread = new(() =>
                {
                    while (eventWaitHandle.WaitOne())
                    {
                        _ = Current.Dispatcher.BeginInvoke(method: () => (Current.MainWindow as MainWindow)?.BringToForeground());
                    }
                })
            {
                // It is important mark it as background otherwise it will prevent app from exiting.
                IsBackground = true
            };
            thread.Start();
            return;
        }
        // Notify other instance so it could bring itself to foreground.
        eventWaitHandle.Set();

        // Terminate this instance.
        Shutdown();
    }
}
