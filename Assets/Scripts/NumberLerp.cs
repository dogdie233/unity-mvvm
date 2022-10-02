using UnityEngine;

public class NumberLerp : MonoBehaviour, IText
{
    [SerializeField] private float animDuration;
    [SerializeField] private int decimalDigits = 0;
    private IText nextText;
    private float? previousValue;
    private float? newValue;
    private float launchTime;

    private string toStringFormat = "";

    public int DecimalDigits
    {
        get => decimalDigits;
        set
        {
            if (value == decimalDigits) return;
            decimalDigits = value;
            toStringFormat = "N" + value.ToString();
        }
    }

    public void Start()
    {
        var components = GetComponents<IText>();
        var foundSelf = false;
        foreach (var component in components)
        {
            if (foundSelf)
            {
                nextText = component;
                break;
            }
            if (component == this) foundSelf = true;
        }
        if (nextText == null)
        {
            Debug.LogError("Can't find next IText!");
        }
        toStringFormat = "N" + decimalDigits.ToString();
    }

    public void SetText(string text)
    {
        if (string.IsNullOrEmpty(text)) text = "0";
        if (previousValue == null || !enabled || animDuration <= 0f)
        {
            nextText?.SetText(text);
            previousValue = float.Parse(text);
            newValue = previousValue;
            return;
        }
        newValue = float.Parse(text);
        launchTime = Time.time;
    }

    private void Update()
    {
        if (previousValue != newValue)
        {
            var delta = (Time.time - launchTime) / animDuration;
            if (0f <= delta && delta <= 1f)
            {
                nextText?.SetText(Mathf.Lerp(previousValue.Value, newValue.Value, delta).ToString(toStringFormat));
                return;
            }
            previousValue = newValue;
            nextText?.SetText(newValue.ToString());
        }
    }
}
