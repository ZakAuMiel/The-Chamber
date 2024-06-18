using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common
{
  [CreateAssetMenu(fileName = "ScriptableVisualTreeAsset_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/ScriptableVisualTreeAsset")]
  public class ScriptableVisualTreeAsset : ScriptableObject
  {
    public string displayName = string.Empty;

    public VisualTreeAsset uxml;

    public StyleSheet styleSheet;

    public ScriptableVisualTreeAssetController controller;

    public VisualElement GetVisualTree()
    {
      VisualElement elem = null;

      if (uxml != null)
      {
        elem = uxml.Instantiate();
      }

      if (styleSheet != null && elem != null)
      {
        elem.AddStyleSheet(styleSheet);
      }

      if (elem != null && controller != null)
      {
        controller.Setup(elem);
      }

      return elem;
    }

    public VisualElement AddVisualTree(VisualElement parent)
    {
      if (parent == null) { return null; }

      var elem = GetVisualTree();

      if (elem != null)
      {
        parent.Add(elem);
      }

      return elem;
    }
  }
}