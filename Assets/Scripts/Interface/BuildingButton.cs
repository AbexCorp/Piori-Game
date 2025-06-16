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
        _showTooltip = true;
        _tooltipText = $"{_assignedProfile.UniqueID}\n Cost: {_assignedProfile.Cost}\n Health: {_assignedProfile.HealthMax}";
    }

    public override void Activate()
    {
        OnClickEvent?.Invoke();
        GameManager.Instance.BuildingManager.SelectProfile(_assignedProfile);
        GameManager.Instance.BuildingManager.Build();
    }
}
