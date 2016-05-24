﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EffectManager : MonoBehaviour {

	[SerializeField] RippleEffect rippleEffect;
	[SerializeField] float rippleEffectTime;

	[SerializeField] MeshRenderer BGPMeshRender;
	[SerializeField] Color BGPFadeToColor;
	[SerializeField] float BGPFadeTime;

	void OnEnable()
	{
		VREvents.ActiveWindow += OnActiveWindow;
	}

	void OnDisable()
	{
		VREvents.ActiveWindow -= OnActiveWindow;
	}

	void OnActiveWindow (WindowArg arg)
	{
//		if ( arg.type == WindowArg.Type.DETAIL_WINDOWS || arg.type == WindowArg.Type.SELECT_WINDOW )
		{
			rippleEffect.enabled = true;
			rippleEffect.EmitAll();
			rippleEffect.waveSpeed = 1f / rippleEffectTime * 2.5f ;
			Sequence seq = DOTween.Sequence();
			seq.AppendInterval( rippleEffectTime );
			seq.AppendCallback( DisableRippleEffect );
		}
		// background image color
		if ( arg.type == WindowArg.Type.DETAIL_WINDOWS )
		{
			BGPMeshRender.material.DOColor( BGPFadeToColor , BGPFadeTime );
		}else if ( arg.type == WindowArg.Type.SELECT_WINDOW )
		{
			BGPMeshRender.material.DOColor( Color.white , BGPFadeTime );
		}
	}

	void DisableRippleEffect() { rippleEffect.enabled = false ; }

}
