// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// Based on https://github.com/it3xl/WPF-app-Single-Instance-in-one-line-of-code

namespace DFWatch;

public static class SingleInstance
{
    #region Private fields
    private static bool _alreadyProcessedOnThisInstance;
    #endregion Private fields

    #region Create the application or exit if application exists
    /// <summary>Creates a single instance of the application.</summary>
    /// <param name="appName">Name of the application.</param>
    /// <param name="uniquePerUser">if set to <c>true</c> unique per user.</param>
    internal static void Create(string appName, bool uniquePerUser = true)
    {
        if (_alreadyProcessedOnThisInstance)
        {
            return;
        }
        _alreadyProcessedOnThisInstance = true;

        Application app = Application.Current;

        string eventName;
        const string uniqueID = "{1E970469-0510-45FD-B7A0-A43B78861BAB}";
        if (uniquePerUser)
        {
            eventName = $"{appName}-{uniqueID}-{Environment.UserName}";
        }
        else
        {
            eventName = $"{appName}-{uniqueID}";
        }

        if (EventWaitHandle.TryOpenExisting(eventName, out EventWaitHandle eventWaitHandle))
        {
            ActivateFirstInstanceWindow(eventWaitHandle);

            Environment.Exit(0);
        }

        RegisterFirstInstanceWindowActivation(app, eventName);
    }
    #endregion Create the application or exit if application exists

    #region Set the event
    /// <summary>Sets the event</summary>
    /// <param name="eventWaitHandle">The event wait handle.</param>
    private static void ActivateFirstInstanceWindow(EventWaitHandle eventWaitHandle)
    {
        _ = eventWaitHandle.Set();
    }
    #endregion Set the event 

    #region Create the event handle and register the instance
    /// <summary>Registers the first instance window activation.</summary>
    /// <param name="app">The application.</param>
    /// <param name="eventName">Name of the event.</param>
    private static void RegisterFirstInstanceWindowActivation(Application app, string eventName)
    {
        EventWaitHandle eventWaitHandle = new(
            false,
            EventResetMode.AutoReset,
            eventName);

        _ = ThreadPool.RegisterWaitForSingleObject(
                eventWaitHandle,
                WaitOrTimerCallback,
                app,
                Timeout.Infinite, false);
        eventWaitHandle.Close();
    }
    #endregion Create the event handle and register the instance

    #region Show the main window
    /// <summary>Shows the main window of the original instance</summary>
    private static void WaitOrTimerCallback(object state, bool timedOut)
    {
        Application app = (Application)state;
        _ = app.Dispatcher.BeginInvoke(new Action(() => MainWindowHelpers.ShowMainWindow()));
    }
    #endregion Show the main window
}
