using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

public class LightEditor : Editor
{
    [SerializeField] private TMP_InputField IntensityInputField;
    [Inject] private InputService InputService;
    private Light Light { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override string EditorTag => "Light";

    protected override void Start()
    {
        base.Start();
        IntensityInputField.onSubmit.AddListener(SubmitOrLoseFocus);
        IntensityInputField.onDeselect.AddListener(SubmitOrLoseFocus);
    }

    private void SubmitOrLoseFocus(string text)
    {
        if (float.TryParse(text, out float intensity))
        {
            Light.intensity = intensity;
            return;
        }

        IntensityInputField.text = Light.intensity.ToString(CultureInfo.InvariantCulture);
    }

    protected override void OnPopulate()
    {
        Light = InputService.SelectedObject.GetComponentInChildren<Light>();
        IntensityInputField.text = Light.intensity.ToString(CultureInfo.InvariantCulture);
    }
}