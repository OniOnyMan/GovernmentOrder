using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System;

public class VideoContainer : MonoBehaviour
{
    public VideoClip VideoClip;
    public Sprite PreviewSprite;

    public Material RenderedMaterial
    {
        get
        {
            if (!_renderedMaterial)
                _renderedMaterial = GetComponent<MeshRenderer>().material;
            return _renderedMaterial;
        }
    }

    private Material _renderedMaterial;

    private void Start()
    {
        ResetMaterialTexture();
        transform.parent.GetComponent<TrackableEventHandler>().OnTracked += VideoPlayerController.Instance.TargetTrackeStateChanged;
        //transform.parent.GetComponent<TrackableEventHandler>().OnTracked += NewMethod;
    }

    private void NewMethod(bool arg1, TrackableEventHandler arg2)
    {
        Debug.LogErrorFormat("[C] {0} is tracked with result {2}", arg2.name, arg1);
    }

    public void SetMaterialTexture(Texture texture)
    {
        RenderedMaterial.SetTexture("_MainTex", texture);
        RenderedMaterial.SetTexture("_EmissionMap", texture);
    }

    public void ResetMaterialTexture()
    {
        SetMaterialTexture(PreviewSprite.texture);
    }
}
