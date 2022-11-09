// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;

/// <summary>
/// Class for NLog helper methods
/// </summary>
internal static class NLHelpers
{
    #region Create the NLog configuration
    /// <summary>
    /// Configure NLog
    /// </summary>
    public static void NLogConfig()
    {
        LoggingConfiguration config = new();

        // create log file Target for NLog
        FileTarget logfile = new("logfile")
        {
            // filename
            FileName = "${basedir}Logs${dir-separator}${processname}.log",

            // create directories if needed
            CreateDirs = true,

            // archive parameters
            ArchiveFileName = "${basedir}Logs${dir-separator}${processname}.{##}.log",
            ArchiveNumbering = ArchiveNumberingMode.Sequence,
            ArchiveAboveSize = UserSettings.Setting.LogFileSize * 1024,
            MaxArchiveFiles = UserSettings.Setting.LogFileVersions,

            // message and footer layouts
            Footer = "${date:format=yyyy/MM/dd HH\\:mm\\:ss}",
            Layout = "${date:format=yyyy/MM/dd HH\\:mm\\:ss} " +
                         "${pad:padding=-5:inner=${level:uppercase=true}}  " +
                         "${message}${onexception:${newline}${exception:format=tostring}}"
        };

        // add the log file target
        config.AddTarget(logfile);

        // add the rule for the log file
        LoggingRule file = new("*", LogLevel.Debug, logfile)
        {
            RuleName = "LogToFile"
        };
        config.LoggingRules.Add(file);

        // create debugger target
        DebuggerTarget debugger = new("debugger")
        {
            Layout = "${processtime} >>> ${message} "
        };

        // add the target
        config.AddTarget(debugger);

        // add the rule
        LoggingRule bug = new("*", LogLevel.Debug, debugger);
        config.LoggingRules.Add(bug);

        // Method target
        MethodCallTarget method = new("methodCall")
        {
            ClassName = typeof(MainWindow).AssemblyQualifiedName,
            MethodName = "LogMethod"
        };
        method.Parameters.Add(new MethodCallParameter("${level:format=TriLetter:uppercase=true}"));
        method.Parameters.Add(new MethodCallParameter("${message}"));

        // add the target
        config.AddTarget(method);

        // add the rule
        LoggingRule meth = new("*", LogLevel.Debug, method)
        {
            RuleName = "LogToMethod"
        };
        config.LoggingRules.Add(meth);

        // add the configuration to NLog
        LogManager.Configuration = config;

        // Lastly, set the logging level based on setting
        SetLogToFileLevel(UserSettings.Setting.IncludeDebugInFile);
        SetLogToMethodLevel(UserSettings.Setting.IncludeDebugInGui);
    }
    #endregion Create the NLog configuration

    #region Set NLog logging level
    /// <summary>
    /// Set the NLog logging level to Debug or Info
    /// </summary>
    /// <param name="debug">If true set level to Debug, otherwise set to Info</param>
    public static void SetLogToFileLevel(bool debug)
    {
        LoggingConfiguration config = LogManager.Configuration;

        LoggingRule rule1 = config.FindRuleByName("LogToFile");
        if (rule1 != null)
        {
            if (debug)
            {
                rule1.SetLoggingLevels(LogLevel.Debug, LogLevel.Fatal);
            }
            else
            {
                rule1.SetLoggingLevels(LogLevel.Info, LogLevel.Fatal);
            }
        }
        LogManager.ReconfigExistingLoggers();
    }

    /// <summary>
    /// Set the NLog logging level to Debug or Info
    /// </summary>
    /// <param name="debug">If true set level to Debug, otherwise set to Info</param>
    public static void SetLogToMethodLevel(bool debug)
    {
        LoggingConfiguration config = LogManager.Configuration;

        LoggingRule rule2 = config.FindRuleByName("LogToMethod");
        if (rule2 != null)
        {
            if (debug)
            {
                rule2.SetLoggingLevels(LogLevel.Debug, LogLevel.Fatal);
            }
            else
            {
                rule2.SetLoggingLevels(LogLevel.Info, LogLevel.Fatal);
            }
        }
        LogManager.ReconfigExistingLoggers();
    }
    #endregion Set NLog logging level

    #region Get the log file name
    /// <summary>
    /// Gets the filename for the NLog log fie
    /// </summary>
    /// <returns></returns>
    public static string GetLogfileName()
    {
        LoggingConfiguration config = LogManager.Configuration;
        Target target = config.FindTargetByName("logfile");
        if (target is FileTarget ft)
        {
            return ft.FileName.Render(new LogEventInfo());
        }
        return string.Empty;
    }
    #endregion Get the log file name
}
