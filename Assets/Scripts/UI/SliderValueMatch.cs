using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SliderValueMatch : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;

    public float max;
    public float min;

    void Start()
    {
        // Make sure both the Input Field and Slider are assigned in the Inspector!
        if (inputField == null || slider == null)
        {
            Debug.LogError("Input Field or Slider is not assigned in the Inspector!");
            enabled = false; // Disable the script if something is missing
            return;
        }

        // Add listeners for when the Input Field value changes
        inputField.onEndEdit.AddListener(OnInputValueChanged);

        // Add listeners for when the Slider value changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialize the Input Field with the Slider's starting value
        inputField.text = slider.value.ToString();
    }

    void OnInputValueChanged(string text)
    {
        // Try to convert the text to a float
        if (float.TryParse(text, out float value))
        {
            // Make sure the value is within the Slider's range
            value = Mathf.Clamp(value, min, max);

            // Update the Slider's value
            slider.value = value;

            inputField.text = value.ToString();
        }
        else
        {
            // If the text is not a valid number, reset the Input Field to the Slider's value
            inputField.text = slider.value.ToString();
        }
    }

    // This function is called when the Slider's value changes
    void OnSliderValueChanged(float value)
    {
        // Update the Input Field's text to the Slider's value
        inputField.text = value.ToString();
    }
}
