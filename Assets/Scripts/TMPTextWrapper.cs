using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMPTextWrapper : MonoBehaviour, IText
{
    [SerializeField] private TMP_Text text;

    public void SetText(string text)
    {
        this.text.text = text;
    }
}
