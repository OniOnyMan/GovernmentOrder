using UnityEngine;
using System.Collections;
using UnityEngine.Video;

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
        SetMaterialTexture(PreviewSprite.texture);
        transform.parent.GetComponent<TrackableEventHandler>().OnTracked += VideoPlayerController.Instance.NewTargetTracked;
    }

    public void SetMaterialTexture(Texture texture)
    {
        RenderedMaterial.SetTexture("_MainTex", texture);
        RenderedMaterial.SetTexture("_EmissionMap", texture);
    }
}
