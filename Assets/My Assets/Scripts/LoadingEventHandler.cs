using System;
using System.Collections;
using UnityEngine;

public class LoadingEventHandler : MonoBehaviour
{
    public Action BackgroundEndingEvent;

    public void BackgroundEndingHandler()
    {
        if(BackgroundEndingEvent != null)
            BackgroundEndingEvent.Invoke();
    }
}

