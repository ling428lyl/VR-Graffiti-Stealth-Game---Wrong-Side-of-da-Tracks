using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CanSyncronizer : SyncronizedObject
{
	[SerializeField] private XRGrabInteractable grabInteractable;

	private bool wasOnLeftSocket = false;
	private bool wasOnRightSocket = false;
	private void OnEnable()
	{
		grabInteractable.selectEntered.AddListener(OnPlaceCan);
	}


	private void OnPlaceCan(SelectEnterEventArgs arg0)
	{
		if (arg0.interactorObject.GetType() == typeof(XRSocketInteractor))
		{
			if (arg0.interactorObject.transform.name.Contains("Left"))
			{
				SyncronizerManager.instance.leftCanOn = true;
				wasOnLeftSocket = true;
			}
			else
			{
				SyncronizerManager.instance.rightCanOn = true;
				wasOnRightSocket = true;
			}
		}
		if (arg0.interactorObject.GetType() == typeof(NearFarInteractor))
		{
			if (wasOnLeftSocket)
			{
				SyncronizerManager.instance.leftCanOn = false;
				wasOnLeftSocket = false;
			}
			else if (wasOnRightSocket)
			{
				SyncronizerManager.instance.rightCanOn = false;
				wasOnRightSocket = false;
			}
		}
	}

	public void FoceApply(XRSocketInteractor hatSocketInteractor)
	{
		hatSocketInteractor.StartManualInteraction((IXRSelectInteractable)grabInteractable);
	}
	private void OnDisable()
	{
		grabInteractable.selectEntered.RemoveListener(OnPlaceCan);
	}
}
