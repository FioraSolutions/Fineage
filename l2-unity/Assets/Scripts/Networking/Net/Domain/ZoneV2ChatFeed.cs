using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds zone chat lines until <see cref="ChatWindow"/> has built its tabs.
/// The welcome <c>SystemMessage</c> is the last packet of enter world, before the game UI exists.
/// </summary>
public static class ZoneV2ChatFeed
{
    struct Line
    {
        public bool System;
        public string Name;
        public string Text;
        public int ChatType;
    }

    static readonly List<Line> Pending = new List<Line>();

    public static void Reset()
    {
        Pending.Clear();
    }

    public static void NoteSay(string name, string text, int chatType)
    {
        Pending.Add(new Line
        {
            System = false,
            Name = name ?? "",
            Text = text ?? "",
            ChatType = chatType
        });
        Flush();
    }

    public static void NoteSystem(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;
        Pending.Add(new Line { System = true, Text = text });
        Flush();
    }

    public static void Flush()
    {
        ChatWindow chat = ChatWindow.Instance;
        if (chat == null || !chat.Ready || Pending.Count == 0)
            return;

        try
        {
            for (int i = 0; i < Pending.Count; i++)
            {
                Line line = Pending[i];
                if (line.System)
                    chat.AppendSystemLine(line.Text);
                else
                    chat.ReceiveZoneSay(line.Name, line.Text, line.ChatType);
            }

            Pending.Clear();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("[ZoneV2] chat line skipped: " + ex.Message);
        }
    }
}
