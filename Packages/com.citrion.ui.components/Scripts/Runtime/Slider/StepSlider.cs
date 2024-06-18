using CitrioN.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CitrioN.UI
{
  enum SliderAxis
  {
    Horizontal = 0,
    Vertical = 1
  }

  public class StepSlider : Slider
  {
    [SerializeField]
    [Tooltip("The step size for a single step.\n" +
             "Only used if the value is higher than 0.")]
    protected float singleStepSize = 0f;

    // TODO Prevent negative values?
    [SerializeField]
    [Tooltip("The amount of steps the slider should have.\n" +
             "Only used if the single step size is not\n" +
             "higher than 0.")]
    protected int stepCount = 0;

    public float StepSize
    {
      get
      {
        if (singleStepSize > 0) { return singleStepSize; }
        if (StepCount > 0)
        {
          return (maxValue - minValue) / stepCount;
        }
        // If not single step size or step count was set
        // make it 10 steps by default
        return (maxValue - minValue) * 0.1f;
      }
      set => singleStepSize = Mathf.Max(value, 0);
    }

    SliderAxis axis { get { return (direction == Direction.LeftToRight || direction == Direction.RightToLeft) ? SliderAxis.Horizontal : SliderAxis.Vertical; } }

    bool reverseValue { get { return direction == Direction.RightToLeft || direction == Direction.TopToBottom; } }

    public int StepCount
    {
      get => stepCount;
      set => stepCount = Mathf.Max(value, 0);
    }

    protected override void Set(float input, bool sendCallback = true)
    {
      if (input > minValue && input < maxValue)
      {
        input = SnapInputToClosestStepSize(input);
      }
      base.Set(input, sendCallback);
    }

    private float SnapInputToClosestStepSize(float input)
    {
      //ConsoleLogger.Log($"Input: {input}");
      var currentStepSize = StepSize;
      var remainder = (input - minValue) % currentStepSize;
      if (remainder == 0) { return input; }
      var add = remainder >= currentStepSize / 2;
      // Round to the closest incremenet of the step size
      var output = input + (add ? StepSize - remainder : -remainder);

      var decimalPlaces = MathUtility.GetDecimals(StepSize);
      decimalPlaces = Mathf.Max(decimalPlaces, MathUtility.GetDecimals(minValue));
      decimalPlaces = Mathf.Max(decimalPlaces, MathUtility.GetDecimals(maxValue));

      if (decimalPlaces > 0)
      {
        output = output.Truncate(decimalPlaces + 1);
      }
      return output;
    }

    public override void OnMove(AxisEventData eventData)
    {
      if (!IsActive() || !IsInteractable())
      {
        base.OnMove(eventData);
        return;
      }

      switch (eventData.moveDir)
      {
        case MoveDirection.Left:
          if (axis == SliderAxis.Horizontal && FindSelectableOnLeft() == null)
            Set(reverseValue ? value + StepSize : value - StepSize);
          else
            base.OnMove(eventData);
          break;
        case MoveDirection.Right:
          if (axis == SliderAxis.Horizontal && FindSelectableOnRight() == null)
            Set(reverseValue ? value - StepSize : value + StepSize);
          else
            base.OnMove(eventData);
          break;
        case MoveDirection.Up:
          if (axis == SliderAxis.Vertical && FindSelectableOnUp() == null)
            Set(reverseValue ? value - StepSize : value + StepSize);
          else
            base.OnMove(eventData);
          break;
        case MoveDirection.Down:
          if (axis == SliderAxis.Vertical && FindSelectableOnDown() == null)
            Set(reverseValue ? value + StepSize : value - StepSize);
          else
            base.OnMove(eventData);
          break;
      }
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/Slider/Replace With StepSlider", priority = 1)]
    private static void ReplaceSliderWithStepSlider(MenuCommand command)
    {
      var slider = (Slider)command.context;
      ReplaceSlider<StepSlider>(slider);
    }

    protected static void ReplaceSlider<T>(Slider slider) where T : Slider
    {
      var obj = slider.gameObject;

      var interactable = slider.interactable;
      var transition = slider.transition;
      var targetGraphic = slider.targetGraphic;
      var navigation = slider.navigation;
      var fillRect = slider.fillRect;
      var handleRect = slider.handleRect;
      var direction = slider.direction;
      var minValue = slider.minValue;
      var maxValue = slider.maxValue;
      var wholeNumbers = slider.wholeNumbers;
      var value = slider.value;
      var onValueChanged = slider.onValueChanged;
      var colors = slider.colors;
      var spiteState = slider.spriteState;
      var animationTriggers = slider.animationTriggers;

      float stepSize = 0f;
      int stepCount = 0;

      if (slider is StepSlider stepSlider)
      {
        stepSize = stepSlider.StepSize;
        stepCount = stepSlider.StepCount;
      }

      Undo.DestroyObjectImmediate(slider);

      var newSlider = obj.AddComponent<T>();
      newSlider.transition = transition;
      newSlider.targetGraphic = targetGraphic;
      newSlider.navigation = navigation;
      newSlider.fillRect = fillRect;
      newSlider.handleRect = handleRect;
      newSlider.direction = direction;
      newSlider.minValue = minValue;
      newSlider.maxValue = maxValue;
      newSlider.wholeNumbers = wholeNumbers;
      newSlider.interactable = interactable;
      newSlider.value = value;
      newSlider.onValueChanged = onValueChanged;
      newSlider.colors = colors;
      newSlider.spriteState = spiteState;
      newSlider.animationTriggers = animationTriggers;

      if (newSlider is StepSlider newStepSlider)
      {
        newStepSlider.StepSize = stepSize;
        newStepSlider.StepCount = stepCount;
      }

      Undo.RegisterCreatedObjectUndo(newSlider, "Replaced with new slider");
    }
#endif
  }
}