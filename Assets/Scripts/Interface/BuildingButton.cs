using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingButton : Button
{
    private void Start()
    {
        OnClickEvent.AddListener(GameManager.Instance.BuildingManager.Build);
    }

    private BuildingProfile _assignedProfile;
    public void SetProfile(BuildingProfile p)
    {
        _assignedProfile = p;
        SetText(_assignedProfile.UniqueID);
    }

    public override void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnClickEvent?.Invoke();
            GameManager.Instance.BuildingManager.SelectProfile(_assignedProfile);
        }
    }
}
