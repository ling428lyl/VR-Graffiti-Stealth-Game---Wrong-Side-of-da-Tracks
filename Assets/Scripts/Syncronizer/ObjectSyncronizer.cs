using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjectSyncronizer : MonoBehaviour
{
	public bool syncHat;
	[SerializeField] private XRSocketInteractor hatSocketInteractor;
	[SerializeField] private HatObject hatBackup;
	public bool syncBelt;
	[SerializeField] private Transform beltTarget;
	[SerializeField] private BeltObject beltBackup;
	[SerializeField] private XRSocketInteractor beltLeftSocket;
	[SerializeField] private XRSocketInteractor beltRightSocket;
	[SerializeField] private CanSyncronizer leftCanBackup;
	[SerializeField] private CanSyncronizer rightCanBackup;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(1);
		if (syncHat)
		{
			SyncHat();
		}
		else
		{
			SyncronizerManager.instance.hatOn = false;
		}

		if (syncBelt)
		{
			SyncBelt();
		}
		else
		{
			SyncronizerManager.instance.beltOn = false;
			SyncronizerManager.instance.leftCanOn = false;
			SyncronizerManager.instance.rightCanOn = false;
		}
	}

	private void SyncBelt()
	{
		if (SyncronizerManager.instance.beltOn)
		{
			beltBackup.ForceApply(beltTarget);
			if (SyncronizerManager.instance.leftCanOn)
			{
				leftCanBackup.FoceApply(beltLeftSocket);
			}
			if (SyncronizerManager.instance.rightCanOn)
			{
				rightCanBackup.FoceApply(beltRightSocket);
			}
		}
	}

	private void SyncHat()
	{
		if (SyncronizerManager.instance.hatOn)
		{
			hatBackup.FoceApply(hatSocketInteractor);
		}
	}
}
