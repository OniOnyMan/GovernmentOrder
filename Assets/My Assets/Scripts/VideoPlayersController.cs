using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayersController : MonoBehaviour {

    private GameObject _playButton;
    private Image _playButtonImage;
    private GameObject _nextButton;
    private GameObject _previousButton;
    private GameObject _resetButton;
    private GameObject _fullScreenButton;
    private Image _fullScreenButtonImage;
    private GameObject _fullScreenPlayer;

    //private List<TargetTrack> _videoPlayerTargets;
    private List<TargetTrack> _trackedVideoPlayerTargets;
    private VideoPlayer _trackedVideoPlayer;
    private int _targetIndex = 0;
    private bool _isInFullScreenMod = false;

    public Sprite PlaySprite;
    public Sprite PauseSprite;
    public Sprite FullScreenSprite;
    public Sprite SmallScreenSprite;

    void Awake()
    {
        GetUIElements();
        /*_videoPlayerTargets =*/ GetVideoPlayerTargets();
        _trackedVideoPlayerTargets = new List<TargetTrack>();
        _playButtonImage = _playButton.GetComponentsInChildren<Image>()[1];
        _fullScreenButtonImage = _fullScreenButton.GetComponentsInChildren<Image>()[1];
    }

    private void GetUIElements()
    {
        _playButton = GameObject.FindGameObjectWithTag("PlayButton");
        _nextButton = GameObject.FindGameObjectWithTag("NextButton");
        _previousButton = GameObject.FindGameObjectWithTag("PreviousButton");
        _resetButton = GameObject.FindGameObjectWithTag("ResetButton");
        _fullScreenButton = GameObject.FindGameObjectWithTag("FullScreenButton");
        _fullScreenPlayer = GameObject.FindGameObjectWithTag("FullScreenPlayer");
        ShowPlayerButtons(false);
        ShowSwitchButtons(false);
        _fullScreenPlayer.SetActive(false);
    }

    private void ShowPlayerButtons(bool condition)
    {
        _playButton.SetActive(condition);
        _resetButton.SetActive(condition);
        _fullScreenButton.SetActive(condition);
    }

    private void ShowSwitchButtons(bool condition)
    {
        _nextButton.SetActive(condition);
        _previousButton.SetActive(condition);
    }
    
    private List<TargetTrack> GetVideoPlayerTargets()
    {
        var targets = new List<TargetTrack>();
        targets.AddRange((GameObject.FindGameObjectsWithTag("Target")).Select(obj => obj.GetComponent<TargetTrack>()).ToList());
        foreach (var target in targets)
        {
            target.TrackStateChanged += StopPlayerOnLost;
            target.TrackStateChanged += GetTrackedTarget;
            target.TrackStateChanged += RendNextVideoScreenIfTracked;
            target.EnableComponents(true);//TODO: fix
            target.EnableComponents(false);
        }
        return targets;
    }

    private void GetTrackedTarget(bool condition, GameObject sender)
    {
        var target = sender.GetComponent<TargetTrack>();
        if (condition)
        {
            _trackedVideoPlayerTargets.Add(target);
            Debug.Log(sender.gameObject.name + " is tracked");
                      //+ " on " + _trackedVideoPlayerTargets.IndexOf(target) + " index");
            if (_trackedVideoPlayer == null //TODO: check
                || _trackedVideoPlayer == sender.GetComponentInChildren<VideoPlayer>())
            {
                RendVideoScreen(true, target);
            }
            if (_trackedVideoPlayerTargets.Count > 1 && !_isInFullScreenMod)
                ShowSwitchButtons(true);
            if (!_isInFullScreenMod)
                _fullScreenPlayer.transform.Find("PreviewImage").gameObject.SetActive(true);
        }
        else
        {
            _trackedVideoPlayerTargets.Remove(target);
            Debug.Log(sender.gameObject.name + " has lost");
            RendVideoScreen(false, target);
            if (!_isInFullScreenMod)
            {
                if (_trackedVideoPlayerTargets.Count == 0)
                    _trackedVideoPlayer = null;
                if (_trackedVideoPlayerTargets.Count == 1)
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
            _fullScreenPlayer.transform.GetChild(0).GetComponent<RawImage>().texture = _trackedVideoPlayer.targetTexture;
            _trackedVideoPlayer.loopPointReached += EndPlayer;
            _trackedVideoPlayer.prepareCompleted += EndLoadingAnimation;
            ShowPlayerButtons(true);
        }
        else
        {
            if (_trackedVideoPlayerTargets.Count == 0 && !_isInFullScreenMod)
                ShowPlayerButtons(false);
        }
    }
    
    private void EndPlayer(VideoPlayer source)
    {
        source.Stop();
        _playButtonImage.overrideSprite = PlaySprite;
        _trackedVideoPlayer.transform.Find("PreviewImage").gameObject.SetActive(true);
        if (_isInFullScreenMod)
            _fullScreenPlayer.transform.Find("PreviewImage").gameObject.SetActive(true);
    }

    private void RendNextVideoScreenIfTracked(bool condition, GameObject sender)
    {
        var temp = sender.GetComponentInChildren<VideoPlayer>();
        if (!condition && _trackedVideoPlayer == temp 
            && _trackedVideoPlayerTargets.Count > 0 && !_isInFullScreenMod)
        {
            _trackedVideoPlayer.loopPointReached -= EndPlayer;
            _trackedVideoPlayer.prepareCompleted -= EndLoadingAnimation;
            while (_targetIndex < 0)
                _targetIndex += _trackedVideoPlayerTargets.Count;
            while (_targetIndex >= _trackedVideoPlayerTargets.Count)
                _targetIndex -= _trackedVideoPlayerTargets.Count;
            RendVideoScreen(true, _trackedVideoPlayerTargets[_targetIndex]);
        }
    }
    
    public void PlayButtonPressed()
    {
        var preview = _trackedVideoPlayer.transform.Find("PreviewImage").gameObject;
        var previewFull = _fullScreenPlayer.transform.Find("PreviewImage").gameObject;
        if (_trackedVideoPlayer.isPlaying)
        {
            _trackedVideoPlayer.Pause();
            _playButtonImage.overrideSprite = PlaySprite;
        }
        else
        {
            if (preview.activeInHierarchy)
            {
                var background = _trackedVideoPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
                background.SetTrigger("Running");
                background.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Running");
                preview.SetActive(false);
            }
            if (previewFull.activeInHierarchy)
            {
                var backgroundUI = _fullScreenPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
                backgroundUI.SetTrigger("Running");
                backgroundUI.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Running");
                previewFull.SetActive(false);
            }
            _trackedVideoPlayer.Play();
            _playButtonImage.overrideSprite = PauseSprite;
        }
    }

    private void EndLoadingAnimation(VideoPlayer source)
    {
        if (source.isPrepared)
        {
            var background = _trackedVideoPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
            background.SetTrigger("Ending");
            background.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Ending");
            if (_isInFullScreenMod)
            {
                var backgroundUI = _fullScreenPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
                backgroundUI.SetTrigger("Ending");
                backgroundUI.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Ending");
            }
        }
    }

    public void StopPlayerOnLost(bool condition, GameObject sender)
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
        var previewFull = _fullScreenPlayer.transform.Find("PreviewImage").gameObject;
        if (isPrevious)
        {
            if (_targetIndex - 1 >= 0)
                _targetIndex--;
            else
                _targetIndex = _trackedVideoPlayerTargets.Count-1;
        }
        else
        {
            if (_targetIndex + 1 < _trackedVideoPlayerTargets.Count)
                _targetIndex++;
            else
                _targetIndex = 0;
        }
        previewFull.SetActive(true);
        StopPlayerOnLost(false, _trackedVideoPlayerTargets[temp].gameObject);
        RendVideoScreen(false, _trackedVideoPlayerTargets[temp]);
        RendNextVideoScreenIfTracked(false, _trackedVideoPlayerTargets[temp].gameObject);
    }

    public void ResetButtonPressed()
    {
        _trackedVideoPlayer.Stop();
        _playButtonImage.overrideSprite = PlaySprite;
        PlayButtonPressed();
    }

    public void FullScreenButtonPressed()
    {
        if (_isInFullScreenMod)
        {
            _isInFullScreenMod = false;
            _fullScreenPlayer.SetActive(false);
            _fullScreenButtonImage.overrideSprite = FullScreenSprite;
            if(_trackedVideoPlayerTargets.Count > 1)
                ShowSwitchButtons(true);
            if (_trackedVideoPlayerTargets.Count == 0)
            {
                ShowPlayerButtons(false);
                EndPlayer(_trackedVideoPlayer);
                _trackedVideoPlayer = null;
            }
            if (_trackedVideoPlayerTargets.Count == 1)
            {
                var trackedVideoPlayerRealTime = _trackedVideoPlayerTargets[0].GetComponentInChildren<VideoPlayer>();
                if (_trackedVideoPlayer != trackedVideoPlayerRealTime)
                {
                    _trackedVideoPlayer.Stop();
                    _trackedVideoPlayer.loopPointReached -= EndPlayer;
                    _trackedVideoPlayer.prepareCompleted -= EndLoadingAnimation;
                    _playButtonImage.overrideSprite = PlaySprite;
                    _trackedVideoPlayer.transform.Find("PreviewImage").gameObject.SetActive(true);
                    _fullScreenPlayer.transform.Find("PreviewImage").gameObject.SetActive(true);
                    RendVideoScreen(true, _trackedVideoPlayerTargets[0]);
                }
            }
        }
        else
        {
            _isInFullScreenMod = true;
            _fullScreenPlayer.SetActive(true);
            _fullScreenButtonImage.overrideSprite = SmallScreenSprite;
            ShowSwitchButtons(false);
        }
    }
}
