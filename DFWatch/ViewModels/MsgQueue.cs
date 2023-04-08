// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch.ViewModels;
/// <summary>
/// Observable message queue
/// </summary>
internal static class MsgQueue
{
    public static ObservableQueue<string> MessageQueue = new();
}
