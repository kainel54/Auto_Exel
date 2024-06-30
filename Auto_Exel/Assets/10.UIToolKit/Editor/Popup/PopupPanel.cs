using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct PopupString
{
    public string title;
    public string content;
    public string confirmText;
    public string cancelText;
}

public class PopupPanel
{
    private VisualElement _root;
    private Button _confirmButton, _cancelButton;

    public PopupPanel(VisualElement root, PopupString str, Action ConfirmAction, Action CancelAction)
    {
        _root = root;
        _root.Q<Label>("Title").text = str.title;
        _root.Q<Label>("ContentLabel").text = str.content;

        _confirmButton = _root.Q<Button>("ConfirmBtn");
        _cancelButton = _root.Q<Button>("CancelBtn");

        _confirmButton.text = str.confirmText;
        _cancelButton.text = str.cancelText;

        _confirmButton.clicked += () =>
        {
            ConfirmAction?.Invoke();
            CloseWindow();
        };

        _cancelButton.clicked += () =>
        {
            CancelAction?.Invoke();
            CloseWindow();
        };
    }

    private void CloseWindow()
    {
        _root.RemoveFromHierarchy();
    }
}
