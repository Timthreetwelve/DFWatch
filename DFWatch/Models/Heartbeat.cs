// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

//! Be mindful that Log messages that begin with "Heartbeat" are parsed differently in ColorConverter.cs

namespace DFWatch.Models;

internal static class Heartbeat
{
    #region Private fields
    private static System.Timers.Timer _heartbeatTimer;
    #endregion Private fields

    #region Start and stop the heartbeat timer
    /// <summary>
    /// Starts the heartbeat timer.
    /// </summary>
    public static void StartHeartbeat()
    {
        TimeSpan interval = TimeSpan.FromMinutes(15);
        _heartbeatTimer = new System.Timers.Timer(interval.TotalMilliseconds)
        {
            AutoReset = true
        };
        _heartbeatTimer.Elapsed += TimerElapsed;
        _heartbeatTimer.Start();
        NLogHelpers.Log.Info("Heartbeat timer started");
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Heartbeat Started");
    }

    /// <summary>
    /// Stops the heartbeat timer.
    /// </summary>
    public static void StopHeartbeat()
    {
        _heartbeatTimer.Stop();
        NLogHelpers.Log.Info("Heartbeat timer stopped");
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Heartbeat Stopped");
    }
    #endregion Start and stop the heartbeat timer

    #region Log the heartbeat message
    /// <summary>
    /// Writes the heartbeat message to the Log.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        NLogHelpers.Log.Info("Heartbeat every 15 minutes");
    }
    #endregion Log the heartbeat message
}
