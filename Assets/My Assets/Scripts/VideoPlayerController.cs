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
    public GameObject ExitDialog;
    [Space(10)]
    public GameObject PlayButton;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    [Space(10)]
    public GameObject FullScreenButton;
    public Sprite FullScreenSprite;
    public Sprite SmallScreenSprite;

    public static VideoPlayerController Instance { get; private set; }

    public TrackableEventHandler TrackedTarget
    {
        get
        {
            return TrackedTargetIndex < 0  ? null : _trackedTargets[TrackedTargetIndex];
        }
    }

    public int TrackedTargetIndex
    {
        get
        {
            if (_trackedTargetIndex < 0)
            {
                if (_trackedTargets.Count == 0)
                    return _trackedTargetIndex = -1;
                else return _trackedTargetIndex = 0;
            }
            else if (_trackedTargetIndex >= _trackedTargets.Count)
                return _trackedTargetIndex %= _trackedTargets.Count;
            else return _trackedTargetIndex;
        }
    }

    private VideoPlayer _videoPlayer;
    private Image _playButtonImage;
    private Image _fullScreenButtonImage;
    private List<TrackableEventHandler> _trackedTargets;
    private int _trackedTargetIndex = -1;
    private bool _isInFullScreenMod;

    public void TargetTrackeStateChanged(bool condition, TrackableEventHandler sender)
    {
        if (condition)
        {
            Debug.LogWarningFormat("{0} has catched", sender.gameObject.name);

            var existCondition = _trackedTargets.Exists(item => item.DublicatedTargets.FirstOrDefault(dublicate => dublicate == sender));
            Debug.LogWarningFormat("_trackedTarges.Exist {0} with result {1}", sender.name, existCondition);
            if (!existCondition)
            {
                _trackedTargets.Add(sender);
                Debug.LogWarningFormat("{0} added", sender.name);

                if (TrackedTarget == sender)
                    RenderTrackedTarget(true, sender);

                if (_trackedTargets.Count > 1 && !_isInFullScreenMod)
                    SwitchButtons.SetActive(true);
            }
            else
            {
                Debug.LogErrorFormat("Nope, {0} already exist", sender.name);
            }
        }
        else
        {
            Debug.LogWarningFormat("{0} has lost", sender.gameObject.name);
            if (TrackedTarget == sender)
                RenderTrackedTarget(false, sender);

            _trackedTargets.Remove(sender);
            Debug.LogWarningFormat("{0} removed", sender.name);

            if (_trackedTargets.Count > 0)
                RenderTrackedTarget(true, sender);

            if (_trackedTargets.Count <= 1)
                SwitchButtons.SetActive(false);
        }
    }

    public void PlayPauseButtonPressed()
    {
        TrackedTarget.VideoContainer.SetMaterialTexture(_videoPlayer.targetTexture);
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

    public void ResetButtonPressed()
    {
        _videoPlayer.Stop();
        PlayPauseButtonPressed();
    }

    public void FullScreenButtonPressed()
    {
        var container = TrackedTarget.VideoContainer;
        if (_isInFullScreenMod)
        {
            _fullScreenButtonImage.sprite = FullScreenSprite;
            container.gameObject.SetActive(true);
            FullScreenController.Instance.DisableFullScreen();
        }
        else
        {
            _fullScreenButtonImage.sprite = SmallScreenSprite;
            FullScreenController.Instance.EnableFullScreen(
                _videoPlayer.isPlaying ? (Texture)_videoPlayer.targetTexture 
                                       : container.PreviewSprite.texture);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += LoopPointReached;
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
        ExitDialog.SetActive(false);
    }

    private void LoopPointReached(VideoPlayer source)
    {
        source.Stop();
        TrackedTarget.VideoContainer.ResetMaterialTexture();
    }

    private void RenderTrackedTarget(bool condition, TrackableEventHandler target)
    {
        var container = target.VideoContainer;
        target.EnableComponents(condition);
        if (condition)
        {
            PlayerButtons.SetActive(true);
            _videoPlayer.clip = container.VideoClip;
        }
        else
        {
            PlayerButtons.SetActive(false);
            if (_videoPlayer.isPlaying)
                PlayPauseButtonPressed();           
            LoopPointReached(_videoPlayer);
        }
    }
}
