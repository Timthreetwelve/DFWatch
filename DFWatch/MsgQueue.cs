// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace DFWatch;
internal static class MsgQueue
{
    public static ObservableQueue<string> MessageQueue = new();
}
