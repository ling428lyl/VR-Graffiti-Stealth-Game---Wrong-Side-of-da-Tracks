using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HatObject : SyncronizedObject
{
	[SerializeField] private XRGrabInteractable grabInteractable;

	private void Start()
	{
		grabInteractable.selectEntered.AddListener(OnWearHat);
	}


	private void OnWearHat(SelectEnterEventArgs arg0)
	{
		if (arg0.interactorObject.GetType() == typeof(XRSocketInteractor))
		{
			SyncronizerManager.instance.hatOn = true;
		}
		if (arg0.interactorObject.GetType() == typeof(NearFarInteractor))
		{
			SyncronizerManager.instance.hatOn = false;
		}
	}

	public void FoceApply(XRSocketInteractor hatSocketInteractor)
	{
		hatSocketInteractor.StartManualInteraction((IXRSelectInteractable)grabInteractable);
	}
}
