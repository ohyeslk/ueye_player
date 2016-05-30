using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CategoryButton : VRBasicButton {
	CategoryInfo m_info;
	CategoryWindow parent;

	[SerializeField] protected Image img;
	[SerializeField] protected Text text;

	[System.Serializable]
	public struct CategorySetting
	{
		public float duration;
		public float anglePerUnit;
		public float radius;
	}
	[SerializeField] CategorySetting m_setting;


	override public void OnConfirm ()
	{
		Debug.Log("[On Confirm]" + name);
		base.OnConfirm();
		URLRequestMessage msg = new URLRequestMessage( this );
		msg.AddMessage(Global.MSG_REQUEST_CATEGORYVIDEO_CATEGORY_KEY , m_info.name );
		VREvents.FireRequestCategoryVideoList( msg );
	}

	void OnDisable()
	{
		VREvents.PostTexture -= RecieveTexture;
	}

	void OnEnable()
	{
		VREvents.PostTexture += RecieveTexture;
	}

	void RecieveTexture( URLRequestMessage msg )
	{
		if ( msg.postObj == this )
		{
			img.sprite = (Sprite)msg.GetMessage(Global.MSG_REQUEST_TEXTURE_SPRITE_KEY);
		}

		PlayInitAnimation();
	}

	public void Init( CategoryInfo info , int index , CategoryWindow p )
	{
		// initilize the sprite first
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.url = info.bgUrl;
		VREvents.FireRequesTexture( msg );

		parent = p;
		text.text = info.name;
		m_info = info;

		img.DOFade(0,0);
		text.DOFade(0,0);

		//set the angle and position offset
		float angle = m_setting.anglePerUnit * ( ( index % parent.column ) - ( parent.column - 1f ) / 2f ) ;
		transform.localRotation = Quaternion.Euler ( 0 ,angle , 0 );
		Vector3 pos = transform.localPosition;
		pos.z = ( Mathf.Cos( angle * Mathf.Deg2Rad ) - 1 ) * m_setting.radius;
		transform.localPosition = pos;

		ResetSubButton();
	}

	void PlayInitAnimation()
	{
		img.DOFade( 1f , m_setting.duration );
		text.DOFade( 1f , m_setting.duration );
	}
}


