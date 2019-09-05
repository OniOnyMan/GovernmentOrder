using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenController : MonoBehaviour
{
    private RawImage _sceenImage;
    private Image _loadingImage;
    private Image _circleImage;
    private float _disableTime = 0.3f;
    private float _circleRotatingTime = 0.48f;
    private Color _spriteColor;
    private Color _circleSpriteColor;

    public static FullScreenController Instance { get; private set; }
    
    public void SetScreenTexture(Texture texture)
    {
        _sceenImage.texture = texture;
    }

    public void EnableFullScreen()
    {
        gameObject.SetActive(true);
    }

    public void EnableFullScreen(Texture texture)
    {
        SetScreenTexture(texture);
        EnableFullScreen();
    }

    public void DisableFullScreen()
    {
        gameObject.SetActive(false);
    }

    public void DisableLoadingScreen()
    {
        _loadingImage.DOColor(new Color(_loadingImage.color.r, _loadingImage.color.g, _loadingImage.color.b, 0), _disableTime).SetEase(Ease.Linear).OnComplete(() => _loadingImage.gameObject.SetActive(false));
        _circleImage.DOColor(new Color(_circleImage.color.r, _circleImage.color.g, _circleImage.color.b, 0), _disableTime).SetEase(Ease.Linear);
    }

    public void ShowLoadingScreen()
    {
        _loadingImage.color = _spriteColor;
        _circleImage.color = _circleSpriteColor;
        _loadingImage.gameObject.SetActive(true);
    }

    private void Awake()
    {
        Instance = this;
        SetUp();
        DisableFullScreen();
    }

    public void SetUp() {
        if (!_sceenImage)
            _sceenImage = transform.GetChild(0).GetComponent<RawImage>();
        if (!_loadingImage)
            _loadingImage = _sceenImage.transform.GetChild(0).GetComponent<Image>();
        if (!_circleImage)
            _circleImage = _loadingImage.transform.GetChild(0).GetComponent<Image>();
        _spriteColor = _loadingImage.color;
        _circleSpriteColor = _circleImage.color;
    }
}
