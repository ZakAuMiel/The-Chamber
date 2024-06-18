using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.StyleProfileSystem
{
  [AddComponentMenu("CitrioN/Style Profile/Style Listener/Style Listener (Image - Color)")]
  public class StyleListener_Color_Image : StyleListener_Color
  {
    [Header("Image Color")]

    [SerializeField]
    [Tooltip("Reference to the image script for which to change the color")]
    protected Image image;

    [SerializeField]
    [Tooltip("Should the current alpha value of the image be kept?")]
    protected bool keepAlpha = false;

    [Range(-360f, 360f)]
    [SerializeField]
    [Tooltip("Optional hue offset to be applied to the provided color")]
    protected int hueOffset = 0;

    [Range(-1f, 1f)]
    [SerializeField]
    [Tooltip("Optional saturation offset to be applied to the provided color")]
    protected float saturationOffset = 0f;

    [Range(-1f, 1f)]
    [SerializeField]
    [Tooltip("Optional value offset to be applied to the provided color")]
    protected float valueOffset = 0f;

    protected void Reset()
    {
      CacheImage();
    }

    protected override void OnEnable()
    {
      base.OnEnable();

      // TODO fetch the actual color
    }

    protected override void Awake()
    {
      base.Awake();
      CacheImage();
    }

    private void CacheImage()
    {
      if (image == null)
      {
        image = GetComponent<Image>();
      }
    }

    protected override void ApplyChange(Color color)
    {
      base.ApplyChange(color);
      if (image != null)
      {
        if (hueOffset != 0 || saturationOffset != 0f || valueOffset != 0f)
        {
          Color.RGBToHSV(color, out var hue, out var saturation, out var value);

          if (hueOffset != 0)
          {
            hue = Mathf.Clamp01(hue + (hueOffset / 360f));
          }

          if (saturationOffset != 0f)
          {
            saturation = Mathf.Clamp01(saturation + saturationOffset);
          }

          if (valueOffset != 0f)
          {
            value = Mathf.Clamp01(value + valueOffset);
          }

          color = Color.HSVToRGB(hue, saturation, value);
        }

        if (keepAlpha)
        {
          color.a = image.color.a;
        }

        image.color = color;
      }
    }
  }
}