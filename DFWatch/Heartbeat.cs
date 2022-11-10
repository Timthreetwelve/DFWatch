// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

//! Be mindful that log messages that begin with "Heartbeat" are parsed differently in ColorConverter.cs

namespace DFWatch;

internal static class Heartbeat
{
    #region Private fields
    private static System.Timers.Timer heartbeatTimer;
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion Private fields

    #region Start and stop the heartbeat timer
    /// <summary>
    /// Starts the heartbeat timer.
    /// </summary>
    public static void StartHeartbeat()
    {
        if (UserSettings.Setting.Heartbeat)
        {
            TimeSpan interval = TimeSpan.FromMinutes(15);
            heartbeatTimer = new System.Timers.Timer(interval.TotalMilliseconds)
            {
                AutoReset = true
            };
            heartbeatTimer.Elapsed += TimerElapsed;
            heartbeatTimer.Start();
            log.Info("Heartbeat timer started");
        }
    }

    /// <summary>
    /// Stops the heartbeat timer.
    /// </summary>
    public static void StopHeartbeat()
    {
        heartbeatTimer.Stop();
        log.Info("Heartbeat timer stopped");
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
        log.Info("Heartbeat every 15 minutes");
    }
    #endregion Log the heartbeat message
}
