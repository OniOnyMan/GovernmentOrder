using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class TestVideoController : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject Preview;

    [Space(10)]
    public Image PlayButtonImage;
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    public void PlayButtonPressed()
    {
        if (VideoPlayer.isPlaying)
        {
            VideoPlayer.Pause();
            PlayButtonImage.overrideSprite = PlaySprite;
        }
        else
        {
            if (Preview.activeInHierarchy)
            {
                var background = VideoPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
                background.SetTrigger("Running");
                background.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Running");
                Preview.SetActive(false);
            }
            VideoPlayer.Play();
            PlayButtonImage.overrideSprite = PauseSprite;
        }
    }

    public void ResetButtonPressed()
    {
        VideoPlayer.Stop();
        PlayButtonPressed();
    }

    private void Start()
    {
        VideoPlayer.loopPointReached += EndPlayer;
        VideoPlayer.prepareCompleted += EndLoadingAnimation;
    }

    private void EndPlayer(VideoPlayer source)
    {
        source.Stop();
        PlayButtonImage.overrideSprite = PlaySprite;
        Preview.SetActive(true);
    }

    private void EndLoadingAnimation(VideoPlayer source)
    {
        if (source.isPrepared)
        {
            var background = VideoPlayer.transform.Find("LoadingBackground").GetComponent<Animator>();
            background.SetTrigger("Ending");
            background.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Ending");
        }
    }
}

