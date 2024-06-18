using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  public static class ProviderUtility_UGUI_Toggle
  {
    public static Type InputFieldType => typeof(Toggle);

    public static bool UpdateInputElement(RectTransform elem, string settingIdentifier,
                                              SettingsCollection settings, List<object> values,
                                              bool initialize, Toggle.ToggleTransition transition)
    {
      if (elem == null) { return false; }

      var field = elem.GetComponentInChildren<Toggle>(true);
      if (field == null) { return false; }

      var dropdown = elem.GetComponentInChildren<TMP_Dropdown>();
      // If the toggle is part of a dropdown be don't use it.
      // This will then likely use the fallback initialization for the dropdown instead.
      if (dropdown != null && field.transform.IsChildOf(dropdown.transform)) { return false; }
      
      // TODO Setup defaults

      if (initialize)
      {
        field.toggleTransition = transition;
      }

      if (ProviderUtility.GetValueFromValues<bool>(values, out var value))
      {
        field.SetIsOnWithoutNotify(value);
      }
      else if (values?.Count > 0)
      {
        var settingsHolder = settings != null ? settings.GetSettingHolder(settingIdentifier) : null;
        var option = settingsHolder?.GetOptionForKey(values[0]);
        if (option != null)
        {
          if (option.Value.ToLowerInvariant() == "true" ||
              option.Value.ToLowerInvariant() == "1")
          {
            field.SetIsOnWithoutNotify(true);
          }
          else if (option.Value.ToLowerInvariant() == "false" ||
                   option.Value.ToLowerInvariant() == "0")
          {
            field.SetIsOnWithoutNotify(false);
          }
          else
          {
            var index = settingsHolder.Options.IndexOf(option);
            if (index == 0) { field.SetIsOnWithoutNotify(false); }
            else if (index == 1) { field.SetIsOnWithoutNotify(true); }
          }
        }
        //var stringValue = settingsHolder.GetOptionValueForKey(values[0]);

        //if (stringValue == "0") { stringValue = "False"; }
        //else if (stringValue == "1") { stringValue = "True"; }
        //else if (stringValue.ToLowerInvariant() == "false") { stringValue = "False"; }
        //else if (stringValue.ToLowerInvariant() == "true") { stringValue = "True"; }

        //if (bool.TryParse(stringValue, out value))
        //{
        //  field.SetIsOnWithoutNotify(value);
        //}
      }

      if (initialize)
      {
        //if (settings != null)
        //{
        //  settings.ApplySettingChange(settingIdentifier, false, false, field.isOn);
        //}

        field.onValueChanged.AddListener((value) =>
        {
          if (settings != null)
          {
            settings.ApplySettingChange(settingIdentifier, false, false, value);
          }
        });
      }
      return true;
    }
  }
}
