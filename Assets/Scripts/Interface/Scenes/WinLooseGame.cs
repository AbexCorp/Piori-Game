using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WinLooseGame : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    #region >>> Mouse <<<

    [Header("Mouse Interaction")]
    [SerializeField]
    private LayerMask _mouseHitMask;

    private Vector2 _mousePosition;
    public Vector2 MousePosition => _mousePosition;

    private IMouseInteractable _hover = null;

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        if (Camera.main == null)
        {
            _mousePosition = Vector2.zero;
            return;
        }

        _mousePosition = context.ReadValue<Vector2>();
        MouseHover(context);
    }
    private void MouseHover(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (Camera.main == null)
            return;
        ////
        //Ui Raycast
        ////
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            if (results.FirstOrDefault().gameObject.TryGetComponent<IMouseInteractable>(out var interracted))
            {
                if (_hover != null)
                {
                    if (interracted != _hover)
                    {
                        _hover.OnHoverExit(context);
                        _hover = interracted;
                        _hover.OnHoverEnter(context);
                        return;
                    }
                    return;
                }
                else
                {
                    _hover = interracted;
                    _hover.OnHoverEnter(context);
                    return;
                }
            }
            else
            {
                if (_hover != null)
                    _hover.OnHoverExit(context);
                _hover = null;
                return;
            }
        }
        else
        {
            if (_hover != null)
                _hover.OnHoverExit(context);
            _hover = null;
        }




        ////
        //Gameobject Raycast
        ////
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask: _mouseHitMask))
        {
            if (hit.collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
            {
                if (_hover != null)
                {
                    if (interacted != _hover)
                    {
                        _hover.OnHoverExit(context);
                        _hover = interacted;
                        _hover.OnHoverEnter(context);
                    }
                }
                else
                {
                    _hover = interacted;
                    _hover.OnHoverEnter(context);
                }
            }
            else
            {
                if (_hover != null)
                    _hover.OnHoverExit(context);
                _hover = null;
            }
        }
        else
        {
            if (_hover != null)
                _hover.OnHoverExit(context);
            _hover = null;
        }
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed == false) return;
        if (Camera.main == null)
            return;
        ////
        //Ui Raycast
        ////
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            if (results.FirstOrDefault().gameObject.TryGetComponent<IMouseInteractable>(out var interracted))
            {
                if (interracted != null)
                    interracted.OnClick(context);
                return;
            }
        }




        ////
        //Gameobject Raycast
        ////
        Ray ray = Camera.main.ScreenPointToRay(MousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask: _mouseHitMask).OrderBy(x => x.distance).ToArray();

        //Single clicks
        if (hits.Length <= 0)
            return;
        if (hits[0].collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
            interacted.OnClick(context);

        //Multiple clicks
        //for(int i = 0; i < hits.Length; i++)
        //{
        //    if (hits[i].collider.gameObject.TryGetComponent<IMouseInteractable>(out var interacted))
        //    {
        //        interacted.OnClick(context);
        //    }
        //}
    }

    #endregion
}
