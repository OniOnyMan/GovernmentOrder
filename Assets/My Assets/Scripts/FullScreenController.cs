using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenController : MonoBehaviour
{
    private RawImage _sceenImage;

    public static FullScreenController Instance { get; private set; }

    public RawImage ScreenImage
    {
        get
        {
            if (_sceenImage == null)
                _sceenImage = transform.GetChild(0).GetComponent<RawImage>();
            return _sceenImage;
        }
    }

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

    private void Awake()
    {
        Instance = this;
        DisableFullScreen();
    }
}
