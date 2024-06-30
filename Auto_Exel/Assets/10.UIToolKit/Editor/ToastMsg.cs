using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ToastMsg
{
    private VisualElement _root;

    private Label _msgLabel;
    private int _index;

    public void Initialize(VisualElement root)
    {
        _root = root;
        _msgLabel = root.Q<Label>("MsgLabel");
    }

    public async void ShowMessage(string msg, float time)
    {
        _msgLabel.text = msg;
        _root.AddToClassList("on");
        _index++;
        int myIndex = _index;
        await Task.Delay(Mathf.RoundToInt(time*1000));

        if (myIndex == _index)
        {
            _root.RemoveFromClassList("on");
        }
    }
}
