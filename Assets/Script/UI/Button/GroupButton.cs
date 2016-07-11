using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class GroupButton : VRFloatButton {

	[SerializeField] Group group;
	[SerializeField] Sprite NormalSprite;
	[SerializeField] Sprite FocusSprite;
	[SerializeField] Image SelectedImage;

	const string MSG_GROUP = "group";

	public void OnEnable()
	{
		VREvents.GroupButtonConfirm += OnGroupButtonConfirm;
		SelectedImage.sprite = FocusSprite;
	}

	void OnGroupButtonConfirm (Message msg)
	{
		Group conGroup = (Group)msg.GetMessage( MSG_GROUP );
		if ( conGroup == group && msg.sender != this )
		{
			UnSelect();
		}
	}

	public void UnSelect()
	{
		FlowFadeOut();
		subButtonAnimation.subButton.sprite = NormalSprite;
		isSelected = false;
	}

	public void Awake()
	{
		subButtonAnimation.subButton.sprite = NormalSprite;
	}


	public void OnDisable()
	{
		VREvents.GroupButtonConfirm -= OnGroupButtonConfirm;
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
	}

	public override void OnConfirm ()
	{
		base.OnConfirm();

	

		Message groupMsg = new Message(this);
		groupMsg.AddMessage(MSG_GROUP,group);
		VREvents.FireGroupButtonConfirm( groupMsg );
	}

	public override void OnExitHover ()
	{
		base.OnExitHover ();

		if ( !isSelected )
		{
			subButtonAnimation.subButton.sprite = NormalSprite;
		}
	}

	protected override void UpdateByProcess ()
	{
		base.UpdateByProcess ();

		if ( SelectedImage != null )
		{
			Color col = SelectedImage.color;
			col.a = flowProcess;
			SelectedImage.color = col;
		}

		if ( subButtonAnimation.subButton != null )
		{
			Color col = subButtonAnimation.subButton.color;
			col.a = 1f - flowProcess;
			subButtonAnimation.subButton.color = col;
		}
	}

	public override void OnBecomeInvisible (float time)
	{
		base.OnBecomeInvisible (time);

		if ( SelectedImage != null )
		{
			SelectedImage.DOFade( 0 , time );
		}
	}

	public override void OnBecomeVisible (float time)
	{
		base.OnBecomeVisible (time);

		// hide the selected image
		if ( SelectedImage != null )
		{
			SelectedImage.DOFade( 0 , 0 );
		}
	}

//	public void StartGrowBig()
//	{
//		StartCoroutine( GrowBigDo());
//	}
//
//	IEnumerator GrowBigDo ()
//	{
//		float time = 0 ; 
//		FlowHelp.enabled = true;
//
//		{
//			Color col = FlowHelp.color;
//			col.a = 0.5f;
//			FlowHelp.color = col;
//		}
//
//		while ( true )
//		{
//			if ( time >= GrowTime )
//				break;
//
//			FlowHelp.transform.localScale = Vector3.one * (growCurve.Evaluate( time / GrowTime ) * 0.7f + 1f);
//
//			{
//				Color col = FlowHelp.color;
//				col.a = (1f - growCurve.Evaluate( time / GrowTime ) ) * 1f;
//				FlowHelp.color = col;
//			}
//
//			time += Time.deltaTime;
//
//			yield return null;
//		}
//
//		{
//			Color col = FlowHelp.color;
//			col.a = 0;
//			FlowHelp.color = col;
//		}
//
//		FlowHelp.enabled = false;
//	}


}

public enum Group
{
	TOP_BUTTON,
	CATEGORY_BUTTON,
}