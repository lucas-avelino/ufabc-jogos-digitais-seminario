using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : MonoBehaviour
{

    [SerializeField] private ChatBox _chatBox;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private ScreenFlashManager _screenFlashManager;
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
                var hasItem = _inventoryManager.HasItem(item[0]);
                if(hasItem)
                {
                    GoTo(item[1]);
                    RegisterDialog(_currentTalker);
                }
                
                return;
            }

            if (nextDialog.Title == "hasAll")
            {
                var items = nextDialog.Text.Split(' ');
                var hasAllItems = true;
                for (int i = 0; i < items.Length - 1; i++)
                {
                    if (!_inventoryManager.HasItem(items[i]))
                    {
                        hasAllItems = false;
                        break;
                    }
                }
                if(hasAllItems)
                {
                    GoTo(items[items.Length - 1]);
                    RegisterDialog(_currentTalker);
                }
                
                return;
            }

            if (nextDialog.Title == "take")
            {
                var item = nextDialog.Text;
                if (_inventoryManager.HasItem(item))
                {
                    _inventoryManager.RemoveItem(item);
                    OnChatBoxNext();
                }
                
                return;
            }

            if (nextDialog.Title == "give")
            {
                var item = nextDialog.Text;
                _inventoryManager.AddItem(item);
                OnChatBoxNext();
                return;
            }

            if (nextDialog.Title == "flash")
            {
                _screenFlashManager.Flash();
                OnChatBoxNext();
                return;
            }

            if (nextDialog.Title == "end")
            {
                var gm = GameObject.Find("GameManager");
                gm.GetComponent<GameManager>().EndGame();
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
