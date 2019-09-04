using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayersqwerty : MonoBehaviour
{
    public GameObject PlayButton;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    [Space(10)]
    public GameObject FullScreenButton;
    public Sprite FullScreenSprite;
    public Sprite SmallScreenSprite;
    [Space(10)]
    public GameObject NextButton;
    public GameObject PreviousButton;
    public GameObject ResetButton;
    public GameObject ExitDialog;
    public GameObject FullScreenPlayer;

    private Image _playButtonImage;
    private Image _fullScreenButtonImage;
    //private List<TargetTrack> _videoPlayerTargets;
    private List<TrackableEventHandler> _videoTargetContainer;
    private VideoPlayer _trackedVideoPlayer;
    private int _targetIndex = 0;
    private bool _isInFullScreenMod = false;
    private bool _isExitDialog = false;

    public GameObject FullScreenPreview
    {
        get
        {
            return FullScreenPlayer.transform.Find("PreviewImage").gameObject;
        }
    }

    public GameObject TrackedVideoPreview
    {
        get
        {
            return _trackedVideoPlayer == null ? null : _trackedVideoPlayer.transform.Find("PreviewImage").gameObject;
        }
    }

    public void SetExitDialogActive(bool state)
    {
        _isExitDialog = state;
        ExitDialog.SetActive(state);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void PlayButtonPressed()
    {
        if (_trackedVideoPlayer.isPlaying)
        {
            _trackedVideoPlayer.Pause();
            _playButtonImage.overrideSprite = PlaySprite;
        }
        else
        {
            if (TrackedVideoPreview.activeInHierarchy)
            {
                _trackedVideoPlayer.GetComponent<LoadingAnimator123>().SetRunning();
                TrackedVideoPreview.SetActive(false);
            }
            if (FullScreenPreview.activeInHierarchy)
            {
                FullScreenPlayer.GetComponent<LoadingAnimator123>().SetRunning();
                FullScreenPreview.SetActive(false);
            }
            _trackedVideoPlayer.Play();
            _playButtonImage.overrideSprite = PauseSprite;
        }
    }

    public void StopPlayerOnLost(bool condition, TrackableEventHandler sender)
    {
        var player = sender.GetComponentInChildren<VideoPlayer>();
        var preview = player.transform.Find("PreviewImage").gameObject;
        if (!condition && !_isInFullScreenMod)
        {
            player.Stop();
            preview.SetActive(true);
            if (player == _trackedVideoPlayer)
                _playButtonImage.overrideSprite = PlaySprite;
        }
    }

    public void NextButtonPressed(bool isPrevious)
    {
        var temp = _targetIndex;
        if (isPrevious)
        {
            if (_targetIndex - 1 >= 0)
                _targetIndex--;
            else
                _targetIndex = _videoTargetContainer.Count - 1;
        }
        else
        {
            if (_targetIndex + 1 < _videoTargetContainer.Count)
                _targetIndex++;
            else
                _targetIndex = 0;
        }
        FullScreenPreview.SetActive(true);
        //StopPlayerOnLost(false, _videoTargetContainer[temp].gameObject);
        //RendVideoScreen(false, _videoTargetContainer[temp]);
        //RendNextVideoScreenIfTracked(false, _videoTargetContainer[temp].gameObject);
    }

    public void ResetButtonPressed()
    {
        _trackedVideoPlayer.Stop();
        //_playButtonImage.overrideSprite = PlaySprite;
        PlayButtonPressed();
    }

    public void FullScreenButtonPressed()
    {
        if (_isInFullScreenMod)
        {
            _isInFullScreenMod = false;
            _trackedVideoPlayer.transform.GetChild(0).gameObject.SetActive(true);
            FullScreenPlayer.SetActive(false);
            _fullScreenButtonImage.overrideSprite = FullScreenSprite;
            if (_videoTargetContainer.Count > 1)
                ShowSwitchButtons(true);
            if (_videoTargetContainer.Count == 0)
            {
                ShowPlayerButtons(false);
                EndPlayer(_trackedVideoPlayer);
                _trackedVideoPlayer = null;
            }
            //if (_trackedVideoPlayer != null && !GetVideoPlayersFromTargetTracks(_videoTargetContainer).Contains(_trackedVideoPlayer))
            //{
            //    _trackedVideoPlayer.Stop();
            //    _trackedVideoPlayer.loopPointReached -= EndPlayer;
            //    _trackedVideoPlayer.prepareCompleted -= EndLoadingAnimation;
            //    _playButtonImage.overrideSprite = PlaySprite;
            //    TrackedVideoPreview.SetActive(true);
            //    FullScreenPreview.SetActive(true);
            //    RendVideoScreen(true, _videoTargetContainer[0]);
            //}
        }
        else
        {
            FullScreenPreview.GetComponent<RawImage>().texture = TrackedVideoPreview.GetComponent<MeshRenderer>().material.mainTexture;
            FullScreenPreview.gameObject.SetActive(TrackedVideoPreview.activeInHierarchy);
            _isInFullScreenMod = true;
            FullScreenPlayer.SetActive(true);
            _trackedVideoPlayer.transform.GetChild(0).gameObject.SetActive(false);
            _fullScreenButtonImage.overrideSprite = SmallScreenSprite;
            ShowSwitchButtons(false);
        }
    }

    public VideoPlayer[] GetVideoPlayersFromTargetTracks(List<TargetTrack> sourse)
    {
        var result = new List<VideoPlayer>();
        foreach (var target in sourse)
        {
            result.Add(target.GetComponentInChildren<VideoPlayer>());
        }
        return result.ToArray();
    }

    private void Awake()
    {
        GetUIElements();
        /*_videoPlayerTargets =*/ GetVideoPlayerTargets();
        _videoTargetContainer = new List<TrackableEventHandler>();
        _playButtonImage = PlayButton.GetComponentsInChildren<Image>()[1];
        _fullScreenButtonImage = FullScreenButton.GetComponentsInChildren<Image>()[1];
        ExitDialog = GameObject.FindGameObjectWithTag("ExitDialog");
        ExitDialog.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (_isInFullScreenMod)
                FullScreenButtonPressed();
            else SetExitDialogActive(!_isExitDialog);
    }

    private void GetUIElements()
    {
        //PlayButton = GameObject.FindGameObjectWithTag("PlayButton");
        //NextButton = GameObject.FindGameObjectWithTag("NextButton");
        //PreviousButton = GameObject.FindGameObjectWithTag("PreviousButton");
        //ResetButton = GameObject.FindGameObjectWithTag("ResetButton");
        //FullScreenButton = GameObject.FindGameObjectWithTag("FullScreenButton");
        //FullScreenPlayer = GameObject.FindGameObjectWithTag("FullScreenPlayer");
        ShowPlayerButtons(false);
        ShowSwitchButtons(false);
        FullScreenPlayer.SetActive(false);
    }

    private void ShowPlayerButtons(bool condition)
    {
        PlayButton.SetActive(condition);
        ResetButton.SetActive(condition);
        FullScreenButton.SetActive(condition);
    }

    private void ShowSwitchButtons(bool condition)
    {
        NextButton.SetActive(condition);
        PreviousButton.SetActive(condition);
    }
    
    private List<TrackableEventHandler> GetVideoPlayerTargets()
    {
        var targets = new List<TrackableEventHandler>();
        targets.AddRange((GameObject.FindGameObjectsWithTag("Target"))
            .Select(obj => obj.GetComponent<TrackableEventHandler>()).ToList());
        foreach (var target in targets)
        {
            //target.OnTracked += StopPlayerOnLost;
            //target.OnTracked += GetTrackedTarget;
            //target.OnTracked += RendNextVideoScreenIfTracked;
            //target.EnableComponents(true);//TODO: fix
            //target.EnableComponents(false);
        }
        return targets;
    }

    private void GetTrackedTarget(bool condition, TrackableEventHandler sender)
    {
        var target = sender.GetComponent<TargetTrack>();
        if (condition)
        {
            //if(!_videoTargetContainer.Exists(x => x.DublicatedTarget == target))
            //    _videoTargetContainer.Add(target);

            Debug.Log(sender.gameObject.name + " is tracked");
                      //+ " on " + _trackedVideoPlayerTargets.IndexOf(target) + " index");
            if (_trackedVideoPlayer == null //TODO: check
                || _trackedVideoPlayer == sender.GetComponentInChildren<VideoPlayer>())
            {
                RendVideoScreen(true, target);
            }
            if (_videoTargetContainer.Count > 1 && !_isInFullScreenMod)
                ShowSwitchButtons(true);
            if (!_isInFullScreenMod)
                FullScreenPreview.SetActive(true);
        }
        else
        {
            //_videoTargetContainer.Remove(target);
            Debug.Log(sender.gameObject.name + " has lost");
            RendVideoScreen(false, target);
            if (!_isInFullScreenMod)
            {
                if (_videoTargetContainer.Count == 0)
                    _trackedVideoPlayer = null;
                if (_videoTargetContainer.Count == 1)
                    ShowSwitchButtons(false);
            }
        }
    }

    private void RendVideoScreen(bool condition, TargetTrack target)
    {
        target.EnableComponents(condition);
        if (condition)
        {
            _trackedVideoPlayer = target.GetComponentInChildren<VideoPlayer>();
            FullScreenPlayer.transform.GetChild(0).GetComponent<RawImage>().texture = _trackedVideoPlayer.targetTexture;
            _trackedVideoPlayer.loopPointReached += EndPlayer;
            _trackedVideoPlayer.prepareCompleted += EndLoadingAnimation;
            ShowPlayerButtons(true);
        }
        else
        {
            if (_videoTargetContainer.Count == 0 && !_isInFullScreenMod)
                ShowPlayerButtons(false);
        }
    }
    
    private void EndPlayer(VideoPlayer source)
    {
        source.Stop();
        _playButtonImage.overrideSprite = PlaySprite;
        TrackedVideoPreview.SetActive(true);
        if (_isInFullScreenMod)
            FullScreenPreview.gameObject.SetActive(true);
    }

    private void RendNextVideoScreenIfTracked(bool condition, TrackableEventHandler sender)
    {
        var temp = sender.GetComponentInChildren<VideoPlayer>();
        if (!condition && _trackedVideoPlayer == temp 
            && _videoTargetContainer.Count > 0 && !_isInFullScreenMod)
        {
            _trackedVideoPlayer.loopPointReached -= EndPlayer;
            _trackedVideoPlayer.prepareCompleted -= EndLoadingAnimation;
            while (_targetIndex < 0)
                _targetIndex += _videoTargetContainer.Count;
            while (_targetIndex >= _videoTargetContainer.Count)
                _targetIndex -= _videoTargetContainer.Count;
            //RendVideoScreen(true, _videoTargetContainer[_targetIndex]);
        }
    }

    private void EndLoadingAnimation(VideoPlayer source)
    {
        _trackedVideoPlayer.GetComponent<LoadingAnimator123>().SetEnding();
        if (_isInFullScreenMod)
        {
            FullScreenPlayer.GetComponent<LoadingAnimator123>().SetEnding();
        }
    }
}
