using UnityEngine;
using System.Collections;
using System;

public class LoadingAnimator : MonoBehaviour
{
    private Animator _backgroundAnimator;
    private Animator _ringAnimator;

    public Animator BackgroundAnimator
    {
        get
        {
            if (_backgroundAnimator == null)
                _backgroundAnimator = transform.Find("LoadingBackground").GetComponent<Animator>();
            return _backgroundAnimator;
        }
    }

    public Animator RingAnimator
    {
        get
        {
            if (_ringAnimator == null)
                _ringAnimator = BackgroundAnimator.transform.GetChild(0).GetComponent<Animator>();
            return _ringAnimator;
        }
    }

    public void SetRunning()
    {
        if(!BackgroundAnimator.gameObject.activeInHierarchy)
            BackgroundAnimator.gameObject.SetActive(true);
        BackgroundAnimator.SetTrigger("Running");
        RingAnimator.SetTrigger("Running");
    }
    public void SetEnding()
    {
        if (BackgroundAnimator.gameObject.activeInHierarchy)
        {
            BackgroundAnimator.SetTrigger("Ending");
            RingAnimator.SetTrigger("Ending");
        }
    }

    private void Start()
    {
        BackgroundAnimator.gameObject.SetActive(false);
        var temp = RingAnimator;
        BackgroundAnimator.GetComponent<LoadingEventHandler>().BackgroundEndingEvent += OnEndingEvent;
    }

    private void OnEndingEvent()
    {
        BackgroundAnimator.gameObject.SetActive(false);
    }
}
