// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

//! Be mindful that log messages that begin with "Heartbeat" are parsed differently in ColorConverter.cs

namespace DFWatch;

internal static class Heartbeat
{
    private static System.Timers.Timer heartbeatTimer;
    private static readonly Logger log = LogManager.GetLogger("logTemp");

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

    public static void StopHeartbeat()
    {
        heartbeatTimer.Stop();
        log.Info("Heartbeat timer stopped");
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        log.Info("Heartbeat every 15 minutes");
    }
}
