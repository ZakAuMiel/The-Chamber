using UnityEditor;
using UnityEngine;

namespace CitrioN.Common.Editor
{
#if !UNITY_2022_1_OR_NEWER
  [CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true, isFallback = true)]
#endif
  public class UIToolkitMonoBehaviourEditor : UIToolkitInspectorWindowEditor { }
}