using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltObject : SyncronizedObject
{
	public FollowToTheSide followToTheSide;
	public void ForceApply(Transform target)
	{
		followToTheSide.target = target;
		followToTheSide.enabled = true;
	}
}
