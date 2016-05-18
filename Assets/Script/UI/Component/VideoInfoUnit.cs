using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VideoInfoUnit : VRBasicButton {
	VideoSelectWindow parent;

	[System.Serializable]
	public struct HoverAnimation
	{
		public Vector3 scale;
		public float duration;
	}
	[SerializeField] HoverAnimation hoverAnimation;



	[System.Serializable]
	public struct ClearAnimation
	{
		public float duration;
		public float moveY;
	}
	[SerializeField] ClearAnimation clearAnimation;

	private VideoInfoUnitState inner_state = VideoInfoUnitState.Normal;
	private VideoInfoUnitState m_state
	{
		get {
			return inner_state;
		}
		set {
			inner_state = value;
		}
	}
	public VideoInfoUnitState State
	{
		get {
			return m_state;
		}
	}


	VideoInfo m_info = new VideoInfo();
	VideoUnitInitAnimation m_anim;

	public VideoInfo Info{
		get { return m_info; }
	}


	override public void OnConfirm ()
	{
		Debug.Log("[On Confirm]" + name);
		base.OnConfirm();
		parent.PlayVideo( Info );
	}

	override public void OnHover(UIHoverEvent e)
	{
		base.OnHover(e);
		UpdateState(e);
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

			PlayInitAnimation();
		}
	}

	/// <summary>
	/// record the start time of hovering 
    /// </summary>
	public void Init(VideoInfo info , VideoUnitInitAnimation anim , VideoSelectWindow _p )
	{
		// initilize the sprite first
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.url = info.coverUrl;
		VREvents.FireRequesTexture( msg );

		m_state = VideoInfoUnitState.Init;
		m_anim = anim;
		parent = _p;

		m_info = info;
		if ( text != null )
			text.text = info.title;

		text.gameObject.SetActive( false );

		ResetConfirm();

	}

	void PlayInitAnimation()
	{
		VideoUnitInitAnimation anim = m_anim;

		img.gameObject.SetActive( true );
		text.gameObject.SetActive( true );
		img.transform.DOScale( Vector3.zero , anim.duration ).From().SetDelay(anim.delay);
		img.transform.DORotate( new Vector3( 90, 90, 90 ) , anim.duration ).From().SetDelay(anim.delay );
		img.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay );
		text.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay ).OnComplete(CompleteInit);
	}

	void CompleteInit()
	{
		m_state = VideoInfoUnitState.Normal;
	}


	void UpdateState(UIHoverEvent e)
	{
		if ( State == VideoInfoUnitState.Normal ) {
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			{
				img.transform.DOScale( hoverAnimation.scale , hoverAnimation.duration );
				m_state = VideoInfoUnitState.Hovered;
			}
		}
		if ( State == VideoInfoUnitState.Hovered )
		{

			if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				img.transform.DOKill();
				img.transform.DOScale( Vector3.one , hoverAnimation.duration );
				m_state = VideoInfoUnitState.Normal;
			}
		}
	}
		

	override public void Clear()
	{
		base.Clear();
		PlayClearAnimation();
	}

	void PlayClearAnimation()
	{
		img.DOFade( 0 , clearAnimation.duration );
		img.transform.DOLocalMoveY( clearAnimation.moveY , clearAnimation.duration );
		text.DOFade( 0 , clearAnimation.duration ).OnComplete(CompleteClear);
		OnHideConfirm();
	}

	void CompleteClear()
	{
		transform.SetParent( null );
		gameObject.SetActive( false );
		GameObject.Destroy( gameObject , 1f );
	}

}
[System.Serializable]
public struct VideoUnitInitAnimation
{
	public float delay;
	public float duration;
}

public enum VideoInfoUnitState
{
	None,
	Init,
	Normal,
	Hovered,
}

[System.Serializable]
public struct VideoUnitSelectParameter
{
	public float selectTime;
	public float process;
}