using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : Singleton<CanvasController>
{
    public void ButtonRestartPressed()
    {
        GameManager.Instance.RestartLevel();
    }
}
