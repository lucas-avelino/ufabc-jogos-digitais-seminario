using System;
using System.Collections.Generic;
using UnityEngine;

public class Talker : MonoBehaviour
{
    [SerializeField] private DictionaryWrapper<string, List<ChatMessage>> _dialogs;

    [SerializeField] string _currentDialogKey;

    public event Action<List<ChatMessage>> OnTalkerClicked;

    public void OnMouseDown()
    {
        Dictionary<string, List<ChatMessage>> dialogDict = _dialogs.ToDictionary();
        OnTalkerClicked?.Invoke(dialogDict[_currentDialogKey]);
    }

    [Serializable]
    public class KeyValuePair<K, V>
    {
        public K Keys;
        public V Values;
    }

    [Serializable]
    public class DictionaryWrapper<K, V>
    {
        public List<KeyValuePair<K, V>> Items;

        public Dictionary<K, V> ToDictionary()
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            foreach (var kvp in Items)
            {
                dict[kvp.Keys] = kvp.Values;
            }
            return dict;
        }
    }
}


