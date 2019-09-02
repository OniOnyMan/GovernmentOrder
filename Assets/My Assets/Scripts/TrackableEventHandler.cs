using System;
using UnityEngine;
using Vuforia;

public class TrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    public TrackableEventHandler DublicatedTarget;
    public bool IsRenderAllow = true;
    public bool IsTracked = false;
    public event Action<bool, TrackableEventHandler> OnTracked;

    private TrackableBehaviour mTrackableBehaviour;
    private TrackableBehaviour.Status m_PreviousStatus;
    private TrackableBehaviour.Status m_NewStatus;
    private VideoContainer _childVideoScreen;

    public VideoContainer ChildVideoContainer
    {
        get
        {
            if (!_childVideoScreen)
                _childVideoScreen = transform.GetChild(0).GetComponent<VideoContainer>();
            return _childVideoScreen;
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

    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName +
                  " " + mTrackableBehaviour.CurrentStatus +
                  " -- " + mTrackableBehaviour.CurrentStatusInfo);

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            if (OnTracked != null && !IsTracked)
                OnTracked.Invoke(IsTracked = true, this);
        }
        else 
        {
            if (OnTracked != null && IsTracked)
                OnTracked.Invoke(IsTracked = false, this);
        }
    }

    private void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    private void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }
}
