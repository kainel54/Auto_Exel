using System.Collections.Generic;
using UnityEngine;

public class DialogSO : ScriptableObject
{
    public int dialogID;
    public string description;
    public List<ChatSO> chatList;
}
