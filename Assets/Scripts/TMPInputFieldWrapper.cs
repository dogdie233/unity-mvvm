using UnityEngine;

using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class TMPInputFieldWrapper : MonoBehaviour, IText
{
    [SerializeField] private TMP_InputField field;
    private DataBinding binding;

    private void Awake()
    {
        binding = GetComponent<DataBinding>();
        field.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetText(string text)
    {
        if (text == field.text) return;
        field.text = text;
    }

    private void OnValueChanged(string newValue)
    {
        binding.SetText(newValue);
    }
}
