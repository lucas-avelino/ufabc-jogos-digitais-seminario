using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{

    [SerializeField] private ChatBox _chatBox;
    Queue<ChatMessage> _currentDialogs = new Queue<ChatMessage>(); 
   
    public Action _onDialogComplete;
    bool _isChatBoxVisible = false;
    bool _isDialogActive => _isChatBoxVisible;

    void Start()
    {
        _chatBox.OnChatBoxNext += OnChatBoxNext;
    }

    private void OnPointOfInterestClicked(PointOfInterest pointOfInterest)
    {
        if (pointOfInterest.Talker != null && !_isDialogActive)
        {
            RegisterDialog(pointOfInterest.Talker.GetCurrentDialog());
        }
    }

    void OnChatBoxNext()
    {
        if (_currentDialogs.Count > 0)
        {
            ChatMessage nextDialog = _currentDialogs.Dequeue();
            _chatBox.Show(nextDialog.Text, nextDialog.Title);
            _isChatBoxVisible = true;
        }
        else
        {
            _chatBox.Hide();
            _isChatBoxVisible = false;
            _onDialogComplete?.Invoke();
        }
    }

    public void RegisterDialog(List<ChatMessage> dialogs)
    {
        _currentDialogs = new Queue<ChatMessage>(dialogs);
        OnChatBoxNext();
    }

    public void CancelDialogs()
    {
        _currentDialogs.Clear();
        _chatBox.Hide();
        _isChatBoxVisible = false;
    }

}
