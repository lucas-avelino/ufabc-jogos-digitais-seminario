using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{

    [SerializeField] private ChatBox _chatBox;
    [SerializeField] private GameObject _world;
     
    private Talker[] _talkers;
    Queue<ChatMessage> _currentDialogs = new Queue<ChatMessage>(); 
   
    public Action _onDialogComplete;

    void Start()
    {
        _talkers = _world.GetComponentsInChildren<Talker>();
        foreach (var talker in _talkers)
        {
            talker.OnTalkerClicked += RegisterDialog;
        }
        _chatBox.Hide();
        _chatBox.OnChatBoxNext += OnChatBoxNext;
    }

    void OnChatBoxNext()
    {
        if (_currentDialogs.Count > 0)
        {
            ChatMessage nextDialog = _currentDialogs.Dequeue();
            _chatBox.Show(nextDialog.Text, nextDialog.Title);
        }
        else
        {
            _chatBox.Hide();
            _onDialogComplete?.Invoke();
        }
    }

    public void RegisterDialog(List<ChatMessage> dialogs)
    {
        _currentDialogs = new Queue<ChatMessage>(dialogs);
        OnChatBoxNext();
    }

}
