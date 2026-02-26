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

    [Header("Tooltip")]
    [SerializeField]
    protected bool _showTooltip = false;
    [SerializeField]
    protected string _tooltipText = "";

    [Header("Pointer")]
    [SerializeField]
    protected bool _usePointer = false;
    [SerializeField]
    protected Vector2 _pointerPosition = Vector2.zero;
    [SerializeField]
    protected Image _pointer;
    [SerializeField]
    protected Sprite _pointerImage;
    [SerializeField]
    protected bool _positionPointerByCode = true;


    void Start()
    {
        _image.color = _colorDefault;
        if (_usePointer)
        {
            if(_pointerImage != null)
            {
                _pointer.sprite = _pointerImage;
            }
            if(_positionPointerByCode)
                _pointer.rectTransform.anchoredPosition = _pointerPosition;
        }
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
        if(_usePointer)
            EnablePointer(true);
    }

    public virtual void OnHoverExit(InputAction.CallbackContext context)
    {
        _image.color = _colorDefault;
        if (_showTooltip)
            GameManager.Instance.InterfaceManager.TooltipSetActive(false);
        if (_usePointer)
            EnablePointer(false);
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

    private void EnablePointer(bool value)
    {
        _pointer.gameObject.SetActive(value);
    }
    private void OnDrawGizmosSelected()
    {
        if (_usePointer == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere((Vector2)gameObject.transform.position + _pointerPosition, 20f);
    }
}
