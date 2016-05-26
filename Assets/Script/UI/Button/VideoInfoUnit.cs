using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[ExecuteInEditMode]
public class VideoInfoUnit : VRBasicButton {
	VideoSelectWindow parent;

	[SerializeField] protected Image img;
	[SerializeField] protected Text text;

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
	/// <summary>
	/// defined the duration and movement in Y of the clear animation
	/// </summary>
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
	/// <summary>
	/// Return the state of the video info unit
	/// </summary>
	/// <value>The state.</value>
	public VideoInfoUnitState State
	{
		get {
			return m_state;
		}
	}

	[System.Serializable]
	public struct VideoUnitSetting
	{
		public float initDelay;
		public float InitDelayPerUnit;
		public float initDuration;
		public AnimationCurve initScaleCurve;
		public float anglePerUnit;
		public float radius;
		public float recieveDuration;
		public AnimationCurve recieveScaleCurve;
		public float FadeInTime;
		public float fadeOutTime;
	}
	[SerializeField] VideoUnitSetting m_setting;

	VideoInfo m_info = new VideoInfo();

	[SerializeField] Image frame;
	[SerializeField] Image help;
	[SerializeField] Image blackCover;
	[SerializeField] float blackCoverAlpha;

	/// <summary>
	/// set to true when initilize and fade in; 
	/// set to false when enter the detail/play window ( fade out ) or remove from selecet window (clear)
	/// </summary>
	bool isVisible = false;

	public VideoInfo Info{
		get { return m_info; }
	}

//	public float angle;
//	public float radius;
//	void Update()
//	{
//		transform.rotation = Quaternion.Euler ( 0 , angle , 0 );
//		Vector3 pos = transform.localPosition;
//		pos.z = ( Mathf.Cos( angle * Mathf.Deg2Rad ) - 1 ) * radius;
//		transform.localPosition = pos;
//	}


	override public void OnConfirm ()
	{
		Debug.Log("[On Confirm]" + name);
		base.OnConfirm();
		parent.ShowVideoDetail( Info );
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

	void Awake()
	{
		if ( blackCover != null )
			blackCover.enabled = false;
		if ( text != null )
			text.enabled = false;
	}

	void RecieveTexture( URLRequestMessage msg )
	{
		if ( msg.postObj == this )
		{
			Texture2D tex = (Texture2D)msg.GetMessage(Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY);
			Rect rec = new Rect(0,0,tex.width ,tex.height );
			m_info.Post = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);

			Outline imgOutline = img.gameObject.GetComponent<Outline>();
			if ( imgOutline != null )
				imgOutline.enabled = true;

			PlayRecieveImgAnimation();
		}
	}

	/// <summary>
	/// record the start time of hovering 
    /// </summary>
	public void Init(VideoInfo info , int index , VideoSelectWindow _p )
	{
		// initilize the sprite first
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.url = info.coverUrl;
		VREvents.FireRequesTexture( msg );

		m_state = VideoInfoUnitState.Init;
		parent = _p;

		// update the delay time
		m_setting.initDelay = index * m_setting.InitDelayPerUnit;

		m_info = info;
		if ( text != null )
		{
			text.text = info.title;
			text.DOFade(0 , 0);
			text.enabled = false;
		}

		// set angle and position offset 
		float angle = m_setting.anglePerUnit * ( ( index % parent.VideoPerRow ) - ( parent.VideoPerRow - 1f ) / 2f ) ;
		transform.localRotation = Quaternion.Euler ( 0 ,angle , 0 );
		Vector3 pos = transform.localPosition;
		pos.z = ( Mathf.Cos( angle * Mathf.Deg2Rad ) - 1 ) * m_setting.radius;
		transform.localPosition = pos;

		// set visible to true
		isVisible = true;

		ResetSubButton();

		PlayInitAnimation();
	}

	public override void OnEnterHover ()
	{
		base.OnEnterHover ();
		ShowBlackCover(subButtonAnimation.showTime);
		ShowText(subButtonAnimation.showTime);
	}

	public override void OnExitHover ()
	{
		base.OnExitHover ();
		HideBlackCover(subButtonAnimation.hideTime);
		HideText(subButtonAnimation.hideTime);
	}

	public void ShowBlackCover( float duration )
	{
		if ( blackCover != null ){
			Debug.Log("Show Black ");
			blackCover.DOKill();
			blackCover.enabled = true;
			blackCover.DOFade( blackCoverAlpha , duration );
		}
	}

	public void ShowText( float duration)
	{
		if ( text != null ) {
			text.enabled = true;
			text.DOKill();
			text.DOFade( 1f , duration / 2f );
		}
	}

	public void HideBlackCover( float duration )
	{
		if ( blackCover != null ) {
//			Debug.Log("Hide black ");
			blackCover.DOKill();
			blackCover.DOFade( 0 , duration ).OnComplete(DisableBlackCover);
		}
		
	}
	void DisableBlackCover(){ blackCover.enabled = false; }

	public void HideText( float duration )
	{
		if ( text != null ) {
			text.DOKill();
			text.DOFade( 0 , duration ).OnComplete(DisableText);
		}
		
	}
	void DisableText () { text.enabled = false; }

	bool shouldPlayRecieveAnimation = false;
	void PlayRecieveImgAnimation( )
	{
		if ( initAnimCoroutine != null )
		{
			shouldPlayRecieveAnimation = true;
		}
		else
		{
			StartCoroutine( DoRecieveAnimation ( ) );
		}
	}

	IEnumerator DoRecieveAnimation()
	{
		float timer = 0;
		img.sprite = m_info.Post;

		if ( isVisible )
		{
			help.enabled = true;
			{ 
				Color col = help.color;
				col.a = 1f;
				help.color = col;
				help.sprite = img.sprite;
				help.DOFade( 0 , m_setting.recieveDuration );
			}
			{
				Color col = img.color;
				col.a = 0;
				img.color = col;
				img.DOFade( 1f , m_setting.recieveDuration );
			}
		}

		while( timer < m_setting.recieveDuration )
		{
			float curveTime = timer / m_setting.recieveDuration;
			img.transform.localScale = Vector3.one * m_setting.recieveScaleCurve.Evaluate( curveTime );
			timer += Time.deltaTime;
			yield return null;
		}

		help.enabled = false;

	}

	void PlayInitAnimation()
	{
		VideoUnitSetting anim = m_setting;

		initAnimCoroutine = StartCoroutine( DoInitAnimation() );
	}

	Coroutine initAnimCoroutine;
	IEnumerator DoInitAnimation()
	{
		yield return new WaitForSeconds( m_setting.initDelay );


		img.enabled = true;
		img.DOFade( 0 , m_setting.initDuration ).From();

		float timer = 0;
		while( timer < m_setting.initDuration )
		{
			float curveTime = timer / m_setting.initDuration;
			img.gameObject.transform.localScale = Vector3.one * m_setting.initScaleCurve.Evaluate( curveTime );
			timer += Time.deltaTime;

			yield return null;
		}

		frame.enabled = true;
		if ( isVisible )
		{
			Color col = frame.color;
			col.a = 0f;
			frame.color = col;
			frame.DOFade( 1f , m_setting.recieveDuration );
		}

		initAnimCoroutine = null;
		if ( shouldPlayRecieveAnimation  )
			StartCoroutine( DoRecieveAnimation( ) );
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
		

	public void Clear( VideoSelectWindow.ClearType type )
	{
		PlayClearAnimation( type );
	}

	public void PlayClearAnimation(VideoSelectWindow.ClearType type)
	{
		isVisible = false;

		img.DOKill();
		frame.DOKill();
		img.transform.DOKill();

		float y = 0;

		switch( type )
		{
		case VideoSelectWindow.ClearType.Up:
			y = clearAnimation.moveY;
			break;
		case VideoSelectWindow.ClearType.Down:
			y = - clearAnimation.moveY;
			break;
		case VideoSelectWindow.ClearType.Disapper:
			y = 0;
			break;
		default:
			y = 0;
			break;
		};

		Sequence seq = DOTween.Sequence();
		seq.Append( img.DOFade( 0 , clearAnimation.duration ));
		seq.Join( frame.DOFade( 0 , clearAnimation.duration ));
		seq.Join( img.transform.DOLocalMoveY( y, clearAnimation.duration ).SetEase(Ease.InBack) );
		seq.AppendCallback( CompleteClear );
		HideBlackCover( clearAnimation.duration);
		HideText(clearAnimation.duration);
		OnExitHover();
	}

	void CompleteClear()
	{
		transform.SetParent( null , true );
		gameObject.SetActive( false );
		GameObject.Destroy( gameObject , 1f );
	}

	public void PlayFadeInAnimation( float time )
	{
		isVisible = true;
		float t = ( time <= 0 ) ? m_setting.FadeInTime : time;
		img.DOFade( 1f , t );
		frame.DOFade( 1f , t );
	}

	public void PlayFadeOutAnimation( float time )
	{
		isVisible = false;
		float t = ( time <= 0 ) ? m_setting.FadeInTime : time;
		img.DOKill();
		frame.DOKill();

		img.DOFade( 0 , t );
		frame.DOFade( 0 , t );
		HideBlackCover(t);
		HideText(t);
		if ( subButtonAnimation.subButton != null )
			subButtonAnimation.subButton.DOFade( 0 , t );
	}

}


public enum VideoInfoUnitState
{
	None,
	Init,
	Normal,
	Hovered,
}