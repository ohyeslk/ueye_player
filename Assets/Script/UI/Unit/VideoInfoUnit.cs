using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VideoInfoUnit : UISensor , IPointerClickHandler {
	[SerializeField] Image videoPost;
	[SerializeField] Text videoName;

	VideoSelectWindow parent;

	[System.Serializable]
	public struct HoverAnimation
	{
		public Vector3 scale;
		public float duration;
	}
	[SerializeField] HoverAnimation hoverAnimation;

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
			videoPost.sprite = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);

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
		if ( videoPost != null )
			videoPost.sprite  = info.Post;
		if ( videoName != null )
			videoName.text = info.title;

		videoPost.gameObject.SetActive( false );
		videoName.gameObject.SetActive( false );
	}

	void PlayInitAnimation()
	{
		VideoUnitInitAnimation anim = m_anim;

		videoPost.gameObject.SetActive( true );
		videoName.gameObject.SetActive( true );
		videoPost.transform.DOScale( Vector3.zero , anim.duration ).From().SetDelay(anim.delay);
		videoPost.transform.DORotate( new Vector3( 90, 90, 90 ) , anim.duration ).From().SetDelay(anim.delay );
		videoPost.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay );
		videoName.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay ).OnComplete(CompleteInit);
	}

	void CompleteInit()
	{
		m_state = VideoInfoUnitState.Normal;
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();
		parent.PlayVideo( Info );

	}

	override public void OnHover(UIHoverEvent e)
	{
		base.OnHover(e);
		UpdateState(e);
	}

	void UpdateState(UIHoverEvent e)
	{
		if ( State == VideoInfoUnitState.Normal ) {
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			{
				videoPost.transform.DOScale( hoverAnimation.scale , hoverAnimation.duration );

				if ( videoName.GetComponent<Outline>() )
					videoName.GetComponent<Outline>().enabled = true;
				m_state = VideoInfoUnitState.Hovered;
			}
		}
		if ( State == VideoInfoUnitState.Hovered )
		{
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				videoPost.transform.DOKill();
				videoPost.transform.DOScale( Vector3.one , hoverAnimation.duration );

				if ( videoName.GetComponent<Outline>() )
					videoName.GetComponent<Outline>().enabled = false;
				m_state = VideoInfoUnitState.Normal;
			}
		}
	}

	/// <summary>
	/// When the VR Mode is 2D, the info unit sense the click action
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick( PointerEventData eventData)
	{
		if (LogicManager.VRMode == VRMode.VR_2D )
		{
			OnConfirm();
		}
	}

}
[System.Serializable]
public struct VideoInfo
{
	public Sprite Post;
	public string title;
	public string playUrl;
	public string coverUrl;
	public string description;
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
