using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Interlude NPC HTML subset used by Talking Island datapack pages:
/// font colors, br/br1, anchors, buttons, images, and basic tables.
/// </summary>
public sealed class NpcHtmlSubset
{
    public string Title = "";
    public readonly List<NpcHtmlBlock> Blocks = new List<NpcHtmlBlock>();

    public static NpcHtmlSubset Parse(string html)
    {
        var doc = new NpcHtmlSubset();
        if (string.IsNullOrEmpty(html))
            return doc;

        html = DecodeEntities(html);
        var stack = new Stack<NpcHtmlBlock>();
        var root = new NpcHtmlBlock { Tag = "root" };
        stack.Push(root);
        int i = 0;
        var text = new StringBuilder();
        while (i < html.Length)
        {
            if (html[i] != '<')
            {
                text.Append(html[i]);
                i++;
                continue;
            }

            Flush(stack, text);
            int end = html.IndexOf('>', i + 1);
            if (end < 0)
                break;
            string raw = html.Substring(i + 1, end - i - 1).Trim();
            i = end + 1;
            if (raw.Length == 0)
                continue;
            if (raw.StartsWith("!--", StringComparison.Ordinal))
                continue;
            bool close = raw[0] == '/';
            string body = close ? raw.Substring(1).Trim() : raw;
            if (body.EndsWith("/", StringComparison.Ordinal))
                body = body.Substring(0, body.Length - 1).Trim();
            string name = TagName(body);
            if (name.Length == 0)
                continue;
            if (close)
            {
                while (stack.Count > 1 && !string.Equals(stack.Peek().Tag, name, StringComparison.OrdinalIgnoreCase))
                    stack.Pop();
                if (stack.Count > 1 && string.Equals(stack.Peek().Tag, name, StringComparison.OrdinalIgnoreCase))
                {
                    NpcHtmlBlock done = stack.Pop();
                    if (name == "title")
                        doc.Title = done.Text ?? "";
                }

                continue;
            }

            var block = new NpcHtmlBlock { Tag = name };
            ReadAttrs(body, block);
            if (name == "br" || name == "br1" || name == "img" || name == "hr")
            {
                stack.Peek().Children.Add(block);
                continue;
            }

            stack.Peek().Children.Add(block);
            stack.Push(block);
        }

        Flush(stack, text);
        doc.Blocks.AddRange(root.Children);
        return doc;
    }

    public List<NpcHtmlBlock> Find(string tag)
    {
        var found = new List<NpcHtmlBlock>();
        Collect(Blocks, tag, found);
        return found;
    }

    static void Collect(List<NpcHtmlBlock> nodes, string tag, List<NpcHtmlBlock> found)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            NpcHtmlBlock node = nodes[i];
            if (string.Equals(node.Tag, tag, StringComparison.OrdinalIgnoreCase))
                found.Add(node);
            if (node.Children != null && node.Children.Count > 0)
                Collect(node.Children, tag, found);
        }
    }

    static void Flush(Stack<NpcHtmlBlock> stack, StringBuilder text)
    {
        if (text.Length == 0 || stack.Count == 0)
            return;
        string value = text.ToString();
        text.Length = 0;
        if (string.IsNullOrWhiteSpace(value) && value.IndexOf('\n') >= 0 && value.Trim().Length == 0)
            return;
        var block = new NpcHtmlBlock { Tag = "text", Text = value };
        NpcHtmlBlock parent = stack.Peek();
        parent.Children.Add(block);
        if (string.IsNullOrEmpty(parent.Text))
            parent.Text = value.Trim();
        else
            parent.Text = (parent.Text + value).Trim();
    }

    static string TagName(string raw)
    {
        int n = 0;
        while (n < raw.Length && !char.IsWhiteSpace(raw[n]) && raw[n] != '=')
            n++;
        return n == 0 ? "" : raw.Substring(0, n).ToLowerInvariant();
    }

    static void ReadAttrs(string raw, NpcHtmlBlock block)
    {
        int i = TagName(raw).Length;
        while (i < raw.Length)
        {
            while (i < raw.Length && char.IsWhiteSpace(raw[i]))
                i++;
            if (i >= raw.Length)
                break;
            int start = i;
            while (i < raw.Length && raw[i] != '=' && !char.IsWhiteSpace(raw[i]))
                i++;
            string key = raw.Substring(start, i - start).ToLowerInvariant();
            while (i < raw.Length && char.IsWhiteSpace(raw[i]))
                i++;
            if (i >= raw.Length || raw[i] != '=')
                continue;
            i++;
            while (i < raw.Length && char.IsWhiteSpace(raw[i]))
                i++;
            if (i >= raw.Length)
                break;
            string value;
            if (raw[i] == '"' || raw[i] == '\'')
            {
                char quote = raw[i++];
                int v = i;
                while (i < raw.Length && raw[i] != quote)
                    i++;
                value = raw.Substring(v, i - v);
                if (i < raw.Length)
                    i++;
            }
            else
            {
                int v = i;
                while (i < raw.Length && !char.IsWhiteSpace(raw[i]))
                    i++;
                value = raw.Substring(v, i - v);
            }

            if (key == "action")
                block.Action = value;
            else if (key == "color")
                block.Color = value;
            else if (key == "src")
                block.Src = value;
            else if (key == "value")
                block.Value = value;
            else if (key == "width")
                block.Width = value;
            else if (key == "height")
                block.Height = value;
        }
    }

    public static string DecodeEntities(string html)
    {
        if (string.IsNullOrEmpty(html) || html.IndexOf('&') < 0)
            return html ?? "";
        return html.Replace("&nbsp;", " ")
            .Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace("&quot;", "\"")
            .Replace("&amp;", "&");
    }
}

public sealed class NpcHtmlBlock
{
    public string Tag = "";
    public string Text = "";
    public string Action = "";
    public string Color = "";
    public string Src = "";
    public string Value = "";
    public string Width = "";
    public string Height = "";
    public readonly List<NpcHtmlBlock> Children = new List<NpcHtmlBlock>();
}
