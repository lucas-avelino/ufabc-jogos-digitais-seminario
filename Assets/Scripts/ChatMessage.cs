using System;
using Unity.VisualScripting;

[Serializable]
public class ChatMessage
{
    public string Title;
    public string Text;

    public ChatMessage(string text, string title = "")
    {
        Text = text;
        Title = title;
    }
}