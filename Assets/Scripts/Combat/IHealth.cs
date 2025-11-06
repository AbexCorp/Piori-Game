using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public GameObject ParentGameObject { get; }
    public int UniqueID { get; }
    public string UniqueName { get; }
    public int HealthMax { get; }
    public int HealthCurrent { get; }
    public void GetDamaged(int damage, int attackerID, string attackerName);
}
