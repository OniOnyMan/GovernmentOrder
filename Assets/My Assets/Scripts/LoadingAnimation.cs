using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingAnimation : MonoBehaviour
{
    public Transform Circle
    {
        get
        {
            if (!_circle)
                _circle = transform.GetChild(0);
            return _circle;
        }
    }

    public SpriteRenderer Sprite
    {
        get
        {
            if (!_sprite)
                _sprite = GetComponent<SpriteRenderer>();
            return _sprite;
        }
    }

    public SpriteRenderer CircleSprite
    {
        get
        {
            if (!_circleSprite)
                _circleSprite = Circle.GetComponent<SpriteRenderer>();
            return _circleSprite;
        }
    }

    private float _disableTime = 0.3f;
    private float _circleRotatingTime = 0.4f;
    private Transform _circle;
    private SpriteRenderer _sprite;
    private SpriteRenderer _circleSprite;
    private Color _spriteColor;
    private Color _circleSpriteColor;

    public void DisableLoadingScreen()
    {
        Sprite.DOColor(new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, 0), _disableTime).SetEase(Ease.Linear).OnComplete(() => Sprite.gameObject.SetActive(false));
        CircleSprite.DOColor(new Color(CircleSprite.color.r, CircleSprite.color.g, CircleSprite.color.b, 0), _disableTime).SetEase(Ease.Linear);
        FullScreenController.Instance.DisableLoadingScreen();
    }

    public void ShowLoadingScreen()
    {
        Sprite.color = _spriteColor;
        CircleSprite.color = _circleSpriteColor;
        Sprite.gameObject.SetActive(true);
        FullScreenController.Instance.ShowLoadingScreen();
    } 

    public void SetUp()
    {
        _spriteColor = Sprite.color;
        _circleSpriteColor = CircleSprite.color;
        Circle.DOLocalRotate(new Vector3(0, 0, 360), _circleRotatingTime, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }
}
