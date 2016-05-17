using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VideoInfoUnitSensor : UISensor {

	[SerializeField] VideoInfoUnit parent;
	VideoInfoUnit P
	{
		get {
			if ( parent == null )
			{
				parent = transform.parent.GetComponent<VideoInfoUnit>();
			}
			return parent;
		}
	}

	public bool isConfirmSensor = false;

	public override void OnConfirm ()
	{
		if ( !isConfirmSensor )
		{
			base.OnConfirm();
			P.OnConfirm();
		}
	}
		
	public override void OnHover (UIHoverEvent e)
	{
		base.OnHover(e);
		if ( isConfirmSensor )
		{
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
				P.ShowConfirm();
			else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End)
			{
				if ( e.next == null || e.next.transform.parent != transform )
					P.HideConfirm();
			}
		}
		else{
			P.OnHover(e);
		}
	}

	public override void OnFocus ()
	{
		if ( !isConfirmSensor )
		{
			base.OnFocus();
			P.OnFucus();
		}
	}


	public void SetEnable( bool to )
	{
		Image img = GetComponent<Image>();
		if ( img != null )
		{
			img.raycastTarget = to;
		}
	}
}
