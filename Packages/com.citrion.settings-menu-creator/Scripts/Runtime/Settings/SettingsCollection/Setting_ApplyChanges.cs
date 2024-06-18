using CitrioN.Common;

namespace CitrioN.SettingsMenuCreator
{
  public class Setting_ApplyChanges : Setting_SettingsCollection
  {
    public override string RuntimeName => "Apply";

    public override object ApplySettingChange(SettingsCollection settings, params object[] args)
    {
      settings.ApplyPendingSettingsChanges();
      return base.ApplySettingChange(settings, null);
    }
  }
}