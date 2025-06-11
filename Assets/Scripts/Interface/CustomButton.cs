using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IMouseInteractable
{
    public UnityEvent OnClickEvent;

    public RectTransform RectTransform;
    [SerializeField]
    protected TMP_Text _text;
    [SerializeField]
    protected Image _image;

    [SerializeField]
    protected Color _colorDefault;
    [SerializeField]
    protected Color _colorHover;


    void Start()
    {
        _image.color = _colorDefault;
    }
    public void SetText(string text)
    {
        _text.text = text;
    }


    public void OnHoverEnter(InputAction.CallbackContext context)
    {
        _image.color = _colorHover;
    }

    public void OnHoveExit(InputAction.CallbackContext context)
    {
        _image.color = _colorDefault;
    }

    public virtual void OnClick(InputAction.CallbackContext context)
    {
        OnClickEvent?.Invoke();
    }
}
