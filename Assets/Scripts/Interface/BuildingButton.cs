using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingButton : CustomButton
{
    private BuildingProfile _assignedProfile;
    public void SetProfile(BuildingProfile p)
    {
        _assignedProfile = p;
        SetText(_assignedProfile.UniqueID);
    }

    public override void Activate()
    {
        OnClickEvent?.Invoke();
        GameManager.Instance.BuildingManager.SelectProfile(_assignedProfile);
        GameManager.Instance.BuildingManager.Build();
    }
}
