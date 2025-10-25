using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingIcon : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rect;
    [SerializeField]
    private TMP_Text _text;
    [SerializeField]
    private Image _icon;

    public RectTransform RectTransform => _rect;

    public void Set(string text, Sprite icon)
    {
        _text.text = text;
        _icon.sprite = icon;
    }
}
