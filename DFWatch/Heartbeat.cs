// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

internal static class Heartbeat
{
    private static Timer heartbeatTimer;
    private static readonly Logger log = LogManager.GetLogger("logTemp");

    public static void StartHeartbeat()
    {
        if (UserSettings.Setting.Heartbeat)
        {
            TimeSpan interval = TimeSpan.FromMinutes(15);
            heartbeatTimer = new Timer(interval.TotalMilliseconds)
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
        log.Info("15 minute heartbeat");
    }
}
