using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[ExecuteInEditMode]
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
	}
	[SerializeField] VideoUnitSetting m_setting;

	VideoInfo m_info = new VideoInfo();

	[SerializeField] Image frame;
	[SerializeField] Image help;
	[SerializeField] Image blackCover;
	[SerializeField] float blackCoverAlpha;
	Sprite m_recieveSprite;

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
			m_recieveSprite = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);

			Outline imgOutline = img.gameObject.GetComponent<Outline>();
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
		float angle = m_setting.anglePerUnit * ( ( index % parent.column ) - ( parent.column - 1f ) / 2f ) ;
		transform.rotation = Quaternion.Euler ( 0 ,angle , 0 );
		Vector3 pos = transform.localPosition;
		pos.z = ( Mathf.Cos( angle * Mathf.Deg2Rad ) - 1 ) * m_setting.radius;
		transform.localPosition = pos;

		ResetSubButton();

		PlayInitAnimation();
	}

	public override void OnEnterHover ()
	{
		base.OnEnterHover ();
		ShowBlackCover();
		ShowText();
	}

	public override void OnExitHover ()
	{
		base.OnExitHover ();
		HideBlackCover();
		HideText();
	}

	public void ShowBlackCover()
	{
		if ( blackCover != null ){
			Debug.Log("Show Black ");
			blackCover.DOKill();
			blackCover.enabled = true;
			blackCover.DOFade( blackCoverAlpha , subButtonAnimation.showTime );
		}
	}

	public void ShowText()
	{
		if ( text != null ) {
			text.enabled = true;
			text.DOFade( 1f , subButtonAnimation.showTime / 2f );
		}
	}

	public void HideBlackCover()
	{
		if ( blackCover != null ) {
			Debug.Log("Hide black ");
			blackCover.DOKill();
			blackCover.DOFade( 0 , subButtonAnimation.hideTime ).OnComplete(DisableBlackCover);
		}
		
	}
	void DisableBlackCover(){ blackCover.enabled = false; }

	public void HideText()
	{
		if ( text != null )
			text.DOFade( 0 , subButtonAnimation.hideTime ).OnComplete(DisableText);
		
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
		help.enabled = true;
		{ 
			Color col = help.color;
			col.a = 1f;
			help.color = col;
			help.sprite = img.sprite;
			help.DOFade( 0 , m_setting.recieveDuration );
		}
		frame.enabled = true;
		{ 
			Color col = frame.color;
			col.a = 0f;
			frame.color = col;
			frame.DOFade( 1f , m_setting.recieveDuration );
		}
		{
			Color col = img.color;
			col.a = 0;
			img.color = col;
			img.sprite = m_recieveSprite;
			img.DOFade( 1f , m_setting.recieveDuration );
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
		OnExitHover();
	}

	void CompleteClear()
	{
		transform.SetParent( null );
		gameObject.SetActive( false );
		GameObject.Destroy( gameObject , 1f );
	}

}


public enum VideoInfoUnitState
{
	None,
	Init,
	Normal,
	Hovered,
}