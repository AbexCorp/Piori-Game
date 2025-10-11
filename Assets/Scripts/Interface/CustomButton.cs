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

    [SerializeField]
    protected bool _showTooltip = false;
    [SerializeField]
    protected string _tooltipText = "";


    void Start()
    {
        _image.color = _colorDefault;
    }
    public void SetText(string text)
    {
        _text.text = text;
    }


    public virtual void OnHoverEnter(InputAction.CallbackContext context)
    {
        _image.color = _colorHover;
        if (_showTooltip)
        {
            GameManager.Instance.InterfaceManager.ChangeTooltipText(_tooltipText);
            GameManager.Instance.InterfaceManager.TooltipSetActive(true);
        }
    }

    public virtual void OnHoverExit(InputAction.CallbackContext context)
    {
        _image.color = _colorDefault;
        if (_showTooltip)
            GameManager.Instance.InterfaceManager.TooltipSetActive(false);
    }

    public virtual void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Activate();
        }
    }
    public virtual void Activate()
    {
        OnClickEvent?.Invoke();
    }
}
