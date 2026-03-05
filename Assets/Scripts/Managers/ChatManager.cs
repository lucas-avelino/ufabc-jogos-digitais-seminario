using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{

    [SerializeField] private ChatBox _chatBox;
    [SerializeField] private GameObject _world;
     
    private Talker[] _talkers;
    Queue<string> _currentDialogs = new Queue<string>(); 
   
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
            string nextDialog = _currentDialogs.Dequeue();
            _chatBox.Show(nextDialog);
        }
        else
        {
            _chatBox.Hide();
            _onDialogComplete?.Invoke();
        }
    }

    public void RegisterDialog(List<string> dialogs)
    {
        _currentDialogs = new Queue<string>(dialogs);
        OnChatBoxNext();
    }

}
