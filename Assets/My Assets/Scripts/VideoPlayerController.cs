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
            return _trackedTargetIndex >= 0 && _trackedTargets.Count > 0 ? _trackedTargets[_trackedTargetIndex] : null;
        }
    }

    private VideoPlayer _videoPlayer;
    private Image _playButtonImage;
    private Image _fullScreenButtonImage;
    private List<TrackableEventHandler> _trackedTargets;
    private int _trackedTargetIndex = -1;
    
    private bool _isInFullScreenMod;

    public void PlayButtonPressed()
    {
        TrackedTarget.ChildVideoContainer.SetMaterialTexture(_videoPlayer.targetTexture);
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
        _videoPlayer.loopPointReached += LoopPointReached;
        _trackedTargets = new List<TrackableEventHandler>();
        _playButtonImage = PlayButton.GetComponentsInChildren<Image>()[1];
        _fullScreenButtonImage = FullScreenButton.GetComponentsInChildren<Image>()[1];
        HideUIElements();
    }

    private void LoopPointReached(VideoPlayer source)
    {
        source.Stop();
        TrackedTarget.ChildVideoContainer.ResetMaterialTexture();
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

                if (!TrackedTarget)
                {
                    _trackedTargetIndex = 0;
                    Debug.LogErrorFormat("_trackedTarget now is {0}", sender.name);
                }

                if (TrackedTarget == sender)
                    RenderTrackedTarget(true);

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
            RenderTrackedTarget(false);
            
            _trackedTargets.Remove(sender);
            Debug.LogWarningFormat("{0} removed", sender.name);

            if (_trackedTargets.Count == 0)
                _trackedTargetIndex = -1;
            else if (_trackedTargets.Count == 1)
                SwitchButtons.SetActive(false);

        }
    }

    private void RenderTrackedTarget(bool condition)
    {
        var container = TrackedTarget.ChildVideoContainer;
        TrackedTarget.EnableComponents(condition);
        if (condition)
        {
            PlayerButtons.SetActive(true);
            _videoPlayer.clip = container.VideoClip;
        }
        else
        {
            PlayerButtons.SetActive(false);
            if (_videoPlayer.isPlaying)
                PlayButtonPressed();           
            LoopPointReached(_videoPlayer);
        }
    }
}
