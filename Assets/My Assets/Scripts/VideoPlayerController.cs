using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    public GameObject PlayerButtons;
    public GameObject SwitchButtons;
    public GameObject FullScreen;
    public GameObject ExitDialog;
    [Space(10)]
    public GameObject PlayButton;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    [Space(10)]
    public GameObject FullScreenButton;
    public Sprite FullScreenSprite;
    public Sprite SmallScreenSprite;

    public TrackableEventHandler TrackedTarget
    {
        get
        {
            return _trackedTargets.Count > 0 ? _trackedTargets[_trackedTargetIndex] : null;
        }
    }

    public static VideoPlayerController Instance { get; private set; }

    private VideoPlayer _videoPlayer;
    private Image _playButtonImage;
    private Image _fullScreenButtonImage;
    private List<TrackableEventHandler> _trackedTargets;
    private int _trackedTargetIndex = 0;
    private bool _isInFullScreenMod;

    public void PlayButtonPressed()
    {
        if (_videoPlayer.isPlaying)
        {
            _playButtonImage.sprite = PlaySprite;
            _videoPlayer.Pause();
        }
        else
        {
            _playButtonImage.sprite = PauseSprite;
            _videoPlayer.Play();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _trackedTargets = new List<TrackableEventHandler>();
        _playButtonImage = PlayButton.GetComponentsInChildren<Image>()[1];
        _fullScreenButtonImage = FullScreenButton.GetComponentsInChildren<Image>()[1];
        HideUIElements();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //    if (_isInFullScreenMod)
        //        FullScreenButtonPressed();
        //    else SetExitDialogActive(!_isExitDialog);
    }

    private void HideUIElements()
    {
        PlayerButtons.SetActive(false);
        SwitchButtons.SetActive(false);
        FullScreen.SetActive(false);
        ExitDialog.SetActive(false);
    }

    public void NewTargetTracked(bool condition, TrackableEventHandler sender)
    {
        if (condition)
        {
            Debug.Log(sender.gameObject.name + " is tracked");

            if (!_trackedTargets.Exists(item => item.DublicatedTarget == sender))
            {
                _trackedTargets.Add(sender);
                Debug.LogErrorFormat("Nope, {0} already exist", sender.name);
            }

            if (!TrackedTarget || TrackedTarget == sender)
                RenderTrackedTarget(true, sender);

            if (_trackedTargets.Count > 1 && !_isInFullScreenMod)
                SwitchButtons.SetActive(true);
        }
        else
        {
            Debug.Log(sender.gameObject.name + " has lost");
            _trackedTargets.Remove(sender);
            RenderTrackedTarget(false, sender);
            Debug.LogErrorFormat("TrackedTarget is {0}", TrackedTarget ? TrackedTarget.name : "null");
            if (!_isInFullScreenMod && _trackedTargets.Count == 1)
                SwitchButtons.SetActive(false);
        }
    }

    private void RenderTrackedTarget(bool condition, TrackableEventHandler target)
    {
        var container = target.ChildVideoContainer;
        target.EnableComponents(condition);
        if (condition)
        {
            PlayerButtons.SetActive(true);
            _videoPlayer.clip = container.VideoClip;
        }
        else
        {
            PlayerButtons.SetActive(false);
        }
    }
}
