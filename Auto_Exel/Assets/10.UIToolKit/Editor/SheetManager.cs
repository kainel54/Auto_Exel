using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SheetManager
{

    protected SheetManagerEditor _manager;
    public virtual void Initialize(VisualElement root, SheetManagerEditor manager)
    {
        _manager = manager;
    }
    public abstract void UploadToSheet();
    public abstract void DownloadFromSheet();
}
