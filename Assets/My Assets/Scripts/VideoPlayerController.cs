using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (_trackedTargets.Count == 0)
                return _trackedTargetIndex = -1;
            if (_trackedTargetIndex < 0)
                return _trackedTargetIndex = _trackedTargets.Count + _trackedTargetIndex;
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
    private bool _isFullScreen;
    private bool _isExitDialog;

    public void TargetTrackeStateChanged(bool condition, TrackableEventHandler sender)
    {
        if (condition)
        {
            Debug.LogWarningFormat("{0} has catched", sender.gameObject.name);

            var existCondition = _trackedTargets.Exists(item => item.DublicatedTargets.FirstOrDefault(dublicate => dublicate == sender));
            Debug.LogWarningFormat("_trackedTarges.Exist {0} with result {1}", sender.name, existCondition);
            if (!existCondition)
            {
                if (TrackedTarget != sender) // TO DO: подумать про костыль
                {
                    _trackedTargets.Add(sender);
                    Debug.LogWarningFormat("{0} added with index {1}, summary {2}", sender.name, _trackedTargets.IndexOf(sender), _trackedTargets.Count);
                }
                else Debug.LogErrorFormat("{0} already exist, but woulde be rendered", sender.name);

                if (TrackedTarget == sender)
                    RenderTrackedTarget(true);
                PlayerButtonsSetActive(true);

                if (_trackedTargets.Count > 1 && !_isFullScreen)
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
                RenderTrackedTarget(false);

            if (_isFullScreen)
            {
                if (TrackedTarget != sender)
                {
                    _trackedTargets.Remove(sender);
                    Debug.LogWarningFormat("{0} removed", sender.name);
                }
            }
            else
            {
                _trackedTargets.Remove(sender);
                Debug.LogWarningFormat("{0} removed", sender.name);
            }

            if (_trackedTargets.Count > 0)
                RenderTrackedTarget(true);

            if (!_isFullScreen && !TrackedTarget)
                PlayerButtonsSetActive(false);

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
            if (!_videoPlayer.isPaused)
                TrackedTarget.LoadingAnimation.ShowLoadingScreen();
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
        if (_isFullScreen)
        {
            _isFullScreen = false;
            _fullScreenButtonImage.sprite = FullScreenSprite;
            FullScreenController.Instance.DisableFullScreen();

            if (TrackedTarget.IsTracked)
                container.gameObject.SetActive(true);
            else
            {
                LoopPointReached(_videoPlayer);
                PrepareCompleted(_videoPlayer);
                PlayerButtonsSetActive(false);
                Debug.LogWarningFormat("{0} removed", TrackedTarget.name);
                _trackedTargets.Remove(TrackedTarget);
            }

            if (_trackedTargets.Count > 1)
                SwitchButtons.SetActive(true);
        }
        else
        {
            _isFullScreen = true;
            _fullScreenButtonImage.sprite = SmallScreenSprite;
            SwitchButtons.SetActive(false);
            FullScreenController.Instance.EnableFullScreen(
                _videoPlayer.isPlaying ? (Texture)_videoPlayer.targetTexture 
                                       : container.PreviewSprite.texture);

        }
    }

    public void SwitchButtonPressed(int indexShifting)
    {
        RenderTrackedTarget(false);
        _trackedTargetIndex += indexShifting;
        RenderTrackedTarget(true);
    }

    public void SetExitDialogActive(bool state)
    {
        _isExitDialog = state;
        ExitDialog.SetActive(state);
    }

    public void QuitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }

    private void PlayerButtonsSetActive(bool condition)
    {
        PlayerButtons.SetActive(condition);
        if (!condition)
            _playButtonImage.sprite = PlaySprite;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.loopPointReached += LoopPointReached;
        _videoPlayer.prepareCompleted += PrepareCompleted;
        _trackedTargets = new List<TrackableEventHandler>();
        _playButtonImage = PlayButton.GetComponentsInChildren<Image>()[1];
        _fullScreenButtonImage = FullScreenButton.GetComponentsInChildren<Image>()[1];
        HideUIElements();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (_isFullScreen)
                FullScreenButtonPressed();
            else SetExitDialogActive(!_isExitDialog);
    }

    private void RenderTrackedTarget(bool condition)
    {
        var container = TrackedTarget.VideoContainer;
        TrackedTarget.EnableComponents(condition);
        if (condition)
        {
            _videoPlayer.clip = container.VideoClip;
        }
        else
        {
            if (!_isFullScreen)
            {
                LoopPointReached(_videoPlayer);
                PrepareCompleted(_videoPlayer);
            }
        }
    }

    private void HideUIElements()
    {
        PlayerButtonsSetActive(false);
        SwitchButtons.SetActive(false);
        ExitDialog.SetActive(false);
    }

    private void LoopPointReached(VideoPlayer source)
    {
        source.Stop();
        _playButtonImage.sprite = PlaySprite;
        TrackedTarget.VideoContainer.ResetMaterialTexture();
    }

    private void PrepareCompleted(VideoPlayer source)
    {
        if(TrackedTarget.IsTracked)
            TrackedTarget.LoadingAnimation.DisableLoadingScreen();
        else
            TrackedTarget.LoadingAnimation.HideLoadingScreen();
    }
}
