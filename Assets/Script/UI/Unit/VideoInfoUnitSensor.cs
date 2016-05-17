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

	public override void OnConfirm ()
	{
		base.OnConfirm();
		P.OnConfirm();
	}
		
	public override void OnHover (UIHoverEvent e)
	{
		base.OnHover(e);
		P.OnHover(e);
	}

	public override void OnFocus ()
	{
		base.OnFocus();
		P.OnFucus();
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
