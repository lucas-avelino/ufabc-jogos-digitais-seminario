using System;
using Unity.VisualScripting;

[Serializable]
public class ChatMessage
{
    public string Text;
    public string Title;

    public ChatMessage(string text, string title = "")
    {
        Text = text;
        Title = title;
    }
}