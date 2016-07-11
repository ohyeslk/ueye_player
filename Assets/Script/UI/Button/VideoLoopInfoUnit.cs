using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class VideoLoopInfoUnit : VRBasicButton {

	/// <summary>
	/// Components 
	/// </summary>
	[SerializeField] protected Image img;
//	[SerializeField] protected Image frame;
//	[SerializeField] protected Image help;
	[SerializeField] protected Image blackCover;
	[SerializeField] protected Transform TouchFrame3D;
	[SerializeField] protected Text text;
	[SerializeField] protected AudioSource initSound;
	[SerializeField] protected AudioSource imageLoadSound;
	[SerializeField] protected float blackCoverAlpha = 0.6f;

	/// <summary>
	/// Animation 
	/// </summary>
	[System.Serializable]
	public struct HoverAnimation
	{
		public float hoverSensity;
		public float remainNormalDuration;
		public Ease remainNormalEaseType;
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
	protected VideoInfoUnitState m_state
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

	/// <summary>
	/// Setting
	/// </summary>
	[System.Serializable]
	public struct VideoUnitSetting
	{
		public float initDelay;
		public float initDuration;
		public float radius;
		public float recieveDuration;
		public float FadeInTime;
		public float fadeOutTime;
	}
	[SerializeField] protected VideoUnitSetting m_setting;



	/// <summary>
	/// set to true when initilize and fade in; 
	/// set to false when enter the detail/play window ( fade out ) or remove from selecet window (clear)
	/// </summary>
	protected bool isVisible = false;
	Quaternion initRotation;
	protected int m_index;

	protected VideoInfo m_info = new VideoInfo();
	public VideoInfo Info{
		get { return m_info; }
	}
		
	[SerializeField] LoopAnimation loopAnimation;

	[System.Serializable]
	public struct LoopAnimation
	{
		public AnimationCurve alphaCurve;
		public AnimationCurve scaleCurve;
		public AnimationCurve positionCurve;
		public float width;
	}
		
	void OnDisable()
	{
		VREvents.PostTexture -= RecieveTexture;
		// TODO add event listener here
	}

	void OnEnable()
	{
		VREvents.PostTexture += RecieveTexture;
		// TODO remove event listener here
	}


	public void UpdatePosition( float process )
	{
		if ( process > 0.4f && process < 0.6f )
		{
			transform.SetAsLastSibling();
			m_Enable = true;
		}
		else
		{
			m_Enable = false;
		}
		
		{
			Color col = img.color;
			col.a = loopAnimation.alphaCurve.Evaluate( process );
			img.color = col;
		}

		{
			transform.localScale = Vector3.one * loopAnimation.scaleCurve.Evaluate( process );
		}

		{
			transform.localPosition = new Vector3( loopAnimation.positionCurve.Evaluate(process) * loopAnimation.width / 2f , 0 , 0 );
		}

	}

	public void Init( VideoInfo _info )
	{
		// save the parameter
		m_info = _info;
		m_index = 0;

		// set state to init
		m_state = VideoInfoUnitState.Init;

		// initilize the sprite
		URLRequestMessage msg = new URLRequestMessage(this);
		msg.url = m_info.coverUrl;
		VREvents.FireRequesTexture( msg );

		// hide the text
		ResetText();

		// set visible to true
		isVisible = true;

		ResetSubButton();

		// play the show up animation
		PlayInitAnimation();

	}

	public void ResetText()
	{
		if ( text != null )
		{
			text.text = m_info.title;
			text.DOFade(0 , 0);
			text.enabled = false;
		}
	}


	public void PlayInitAnimation()
	{
		if ( gameObject.activeSelf )
			initAnimCoroutine = StartCoroutine( DoInitAnimation() );
	}

	Coroutine initAnimCoroutine;
	bool shouldPlayRecieveAnimation = false;
	IEnumerator DoInitAnimation()
	{
		yield return new WaitForSeconds( m_setting.initDelay );

		if ( initSound != null )
			initSound.Play();

		img.enabled = true;
		img.DOFade( 0 , m_setting.initDuration ).From();

		float timer = 0;
		while( timer < m_setting.initDuration )
		{
			float curveTime = timer / m_setting.initDuration;
//			imgOutside.localScale = Vector3.one * m_setting.initScaleCurve.Evaluate( curveTime );

			timer += Time.deltaTime;

			yield return null;
		}

		initAnimCoroutine = null;

		if ( shouldPlayRecieveAnimation  )
			StartCoroutine( DoRecieveAnimation( ) );
	}

	void RecieveTexture( URLRequestMessage msg )
	{
		if ( msg.postObj == this )
		{
			Debug.Log("Recieve Image");
			m_info.Post = (Sprite)msg.GetMessage(Global.MSG_REQUEST_TEXTURE_SPRITE_KEY);
			PlayRecieveImgAnimation();
		}
	}

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

		if ( isVisible )
		{
			if ( imageLoadSound != null )
				imageLoadSound.Play();

//			help.enabled = true;
//			{ 
//				Color col = help.color;
//				col.a = 1f;
//				help.color = col;
//				help.sprite = img.sprite;
//				help.DOFade( 0 , m_setting.recieveDuration ).SetEase(Ease.OutQuad);
//			}
		}

		img.sprite = m_info.Post;

		while( timer < m_setting.recieveDuration )
		{
			float curveTime = timer / m_setting.recieveDuration;
//			imgOutside.localScale = Vector3.one * m_setting.recieveScaleCurve.Evaluate( curveTime );
			timer += Time.deltaTime;
			yield return null;
		}

//		help.enabled = false;
	}

	public override void OnConfirm ()
	{
		base.OnConfirm ();

		Message msg = new Message(this);
		msg.AddMessage( Global.MSG_VIDEO_INFO_KEY , m_info );
		VREvents.FireShowDetail( msg );


		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.DETAIL_WINDOWS;
		VREvents.FireActiveWindow( arg );
	}

	public void Clear()
	{
	}

}
