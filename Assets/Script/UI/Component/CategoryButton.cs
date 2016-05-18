using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CategoryButton : VRBasicButton {
	CategoryInfo m_info;
	CategoryWindow parent;

	[System.Serializable]
	public struct CategoryInitAnimation
	{
		public float duration;
	}
	[SerializeField] CategoryInitAnimation initAnim;

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
			Texture2D tex = (Texture2D)msg.GetMessage(Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY);
			Rect rec = new Rect(0,0,tex.width ,tex.height );
			img.sprite = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);
		}

		PlayInitAnimation();
	}

	public void Init( CategoryInfo info , CategoryWindow p )
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

		ResetConfirm();
	}

	void PlayInitAnimation()
	{
		img.DOFade( 1f , initAnim.duration );
		text.DOFade( 1f , initAnim.duration );
	}
}


