using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{

    [SerializeField] private ChatBox _chatBox;
    [SerializeField] private InventoryManager _inventoryManager;
    Queue<ChatMessage> _currentDialogs = new Queue<ChatMessage>(); 
   
    public Action _onDialogComplete;
    bool _isChatBoxVisible = false;
    public bool IsDialogActive => _isChatBoxVisible;

    void Start()
    {
        _chatBox.OnChatBoxNext += OnChatBoxNext;
    }

    void OnChatBoxNext()
    {
        if (_currentDialogs.Count > 0)
        {
            ChatMessage nextDialog = _currentDialogs.Dequeue();
            if (nextDialog.Title == "goto")
            {
                GoTo(nextDialog.Text);
                return;
            }

            if (nextDialog.Title == "command")
            {
                var command = nextDialog.Text.Split(' ');
                GameObject.Find(command[0]).GetComponent<Talker>().SetCurrentDialogKey(command[1]);
    
                OnChatBoxNext();
                return;
            }

            if (nextDialog.Title == "has")
            {
                var item = nextDialog.Text.Split(' ');
                var hasItem = _inventoryManager.HasItem(item[0], int.Parse(item[1]));
                if(hasItem)
                    GoTo(item[1]);
                
                OnChatBoxNext();
                return;
            }

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

    void GoTo(string dialogKey)
    {
        _currentTalker.SetCurrentDialogKey(dialogKey);
        _chatBox.Hide();
        _isChatBoxVisible = false;
        _onDialogComplete?.Invoke();
        _currentDialogs.Clear();
    }
    Talker _currentTalker;
    public void RegisterDialog(Talker talker)
    {
        if (talker.HasDialog())
        {
            _currentTalker = talker;
            _currentDialogs = new Queue<ChatMessage>(talker.GetCurrentDialog());
            OnChatBoxNext();
        }
        else
        {
            Debug.LogWarning($"Talker '{talker.name}' does not have a dialog for the current dialog key.");
        }
    }

    public void CancelDialogs()
    {
        _currentDialogs.Clear();
        _chatBox.Hide();
        _isChatBoxVisible = false;
    }

}
