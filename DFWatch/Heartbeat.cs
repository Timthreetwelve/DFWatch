// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

//! Be mindful that log messages that begin with "Heartbeat" are parsed differently in ColorConverter.cs

namespace DFWatch;

internal static class Heartbeat
{
    #region Private fields
    private static System.Timers.Timer _heartbeatTimer;
    private static readonly Logger _log = LogManager.GetLogger("logTemp");
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
        _log.Info("Heartbeat timer started");
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Heartbeat Started");
    }

    /// <summary>
    /// Stops the heartbeat timer.
    /// </summary>
    public static void StopHeartbeat()
    {
        _heartbeatTimer.Stop();
        _log.Info("Heartbeat timer stopped");
        (Application.Current.MainWindow as MainWindow)?.DisappearingMessage("Heartbeat Stopped");
    }
    #endregion Start and stop the heartbeat timer

    #region Log the heartbeat message
    /// <summary>
    /// Writes the heartbeat message to the log.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        _log.Info("Heartbeat every 15 minutes");
    }
    #endregion Log the heartbeat message
}
