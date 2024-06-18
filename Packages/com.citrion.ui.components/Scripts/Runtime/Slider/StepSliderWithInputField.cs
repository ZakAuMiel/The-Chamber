using CitrioN.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.UI
{
  public class StepSliderWithInputField : StepSlider
  {
    [SerializeField]
    [Tooltip("Reference to the input field\n" +
             "to synchronize with the slider")]
    private TMP_InputField inputField;

    protected TMP_InputField InputField
    {
      get { return inputField; }
      set
      {
        inputField = value;
        SetupInputField();
      }
    }

    protected override void Start()
    {
      base.Start();
      SetupInputField();
    }

    protected void SetupInputField()
    {
      if (InputField != null)
      {
        InputField.characterValidation = TMP_InputField.CharacterValidation.Decimal;
      }
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      //onValueChanged.AddListener(OnSliderValueChanged);

      if (InputField != null)
      {
        //inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        InputField.onSubmit.AddListener(OnInputFieldValueChanged);
      }
      SyncFieldWithSliderValue();
    }

    protected override void OnDisable()
    {
      base.OnDisable();
      //onValueChanged.RemoveListener(OnSliderValueChanged);

      if (InputField != null)
      {
        //inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
        InputField.onSubmit.RemoveListener(OnInputFieldValueChanged);
      }
    }

    protected override void Set(float input, bool sendCallback = true)
    {
      base.Set(input, sendCallback);
      SyncFieldWithSliderValue();

      if (InputField != null)
      {
        int characterLimit = Mathf.FloorToInt(Mathf.Log10(maxValue) + 1);

        var decimalPlaces = MathUtility.GetDecimals(minValue);
        decimalPlaces = Mathf.Max(decimalPlaces, MathUtility.GetDecimals(maxValue));
        decimalPlaces = Mathf.Max(decimalPlaces, MathUtility.GetDecimals(StepSize));
        if (decimalPlaces > 0)
        {
          characterLimit += (decimalPlaces + 1);
        }

        if (characterLimit > 0)
        {
          InputField.characterLimit = characterLimit;
        }
      }
    }

    //public override void SetValueWithoutNotify(float input)
    //{
    //  base.SetValueWithoutNotify(input);
    //  SyncFieldWithSliderValue();
    //}

    //protected void OnSliderValueChanged(float value)
    //{
    //  if (inputField == null) { return; }
    //  inputField.text = value.ToString();
    //}

    protected void SyncFieldWithSliderValue()
    {
      if (InputField == null) { return; }
      InputField.text = value.ToString();
    }

    protected void OnInputFieldValueChanged(string value)
    {
      bool isNumeric = FloatExtensions.TryParseFloat(value, out float inputValue);

      if (isNumeric)
      {
        this.value = inputValue;
      }

      if (InputField != null)
      {
        // We always update the input field to ensure the
        // value shown on the input field is always matching the
        // current value on the slider.
        // This makes sure incorrect values or out of bound values
        // will be overriden by the correct value of the slider.
        InputField.text = this.value.ToString();
        InputField.GetType().GetPrivateMethod("MarkGeometryAsDirty")?.Invoke(InputField, null);
      }

      // Select the slider after submitting the value
      // via the input field automatic navigation works
      Select();
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/Slider/Replace With StepSliderWithInputField", priority = 2)]
    private static void ReplaceSliderWithStepSliderWithInputField(MenuCommand command)
    {
      var slider = (Slider)command.context;
      ReplaceSlider<StepSliderWithInputField>(slider);
    }
#endif
  }
}