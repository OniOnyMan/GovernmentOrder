using System;
using UnityEngine;
using System.Collections;
using Vuforia;

public class TargetTrack : MonoBehaviour, ITrackableEventHandler
{
    private TrackableBehaviour mTrackableBehaviour;
    private bool isTracked = false;
    
    public bool IsTracked { get { return isTracked; } }

    public event Action<bool, GameObject> TrackStateChanged;

    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if (TrackStateChanged != null && !isTracked)
                TrackStateChanged.Invoke(isTracked = true, gameObject);
        }
        else
        {
            if (TrackStateChanged != null && isTracked)
                TrackStateChanged.Invoke(isTracked = false, gameObject);
        }
    }

    public void EnableComponents(bool condition)
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);
        if (condition)
        {
            foreach (var component in rendererComponents)
                component.enabled = true;
            foreach (var component in colliderComponents)
                component.enabled = true;
            foreach (var component in canvasComponents)
                component.enabled = true;
        }
        else
        {
            foreach (var component in rendererComponents)
                component.enabled = false;
            foreach (var component in colliderComponents)
                component.enabled = false;
            foreach (var component in canvasComponents)
                component.enabled = false;
        }
    }
}