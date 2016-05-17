using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VideoInfoUnit : MonoBehaviour {
	[SerializeField] Image videoPost;
	[SerializeField] Text videoName;
	[SerializeField] VideoInfoUnitSensor Sensor2D;
	[SerializeField] VideoInfoUnitSensor Sensor3D;
	[SerializeField] VideoUnitConfirmAnimation confirmAni;
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

	public UISensor tempSensor
	{
		get {
			if ( LogicManager.VRMode == VRMode.VR_2D )
				return Sensor2D;
			else
				return Sensor3D;
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
		VREvents.SwitchVRMode -= OnSwitchVRMode;
	}

	void OnEnable()
	{
		VREvents.PostTexture += RecieveTexture;
		VREvents.SwitchVRMode += OnSwitchVRMode;
	}

	void OnSwitchVRMode( Message msg )
	{
		Sensor3D.Reset();
		Sensor2D.Reset();
		VRMode to = (VRMode) msg.GetMessage(Global.MSG_SWITCHVRMODE_MODE_KEY ) ;
		if ( to == VRMode.VR_2D )
		{
			Sensor3D.SetEnable( false );
			Sensor2D.SetEnable( true );
		}else
		{
			Sensor3D.SetEnable( true );
			Sensor2D.SetEnable( false );
		}
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
//		if ( videoPost != null )
//			videoPost.sprite  = info.Post;
		if ( videoName != null )
			videoName.text = info.title;

//		videoPost.gameObject.SetActive( false );
		videoName.gameObject.SetActive( false );

		ResetConfirm();

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
		
	public  void OnConfirm ()
	{
		Debug.Log("[On Confirm]" + name);
		tempSensor.OnConfirm ();
		parent.PlayVideo( Info );
	}

	 public void OnHover(UIHoverEvent e)
	{
		Debug.Log("[On Hover] " + name + " " + e.hoverPhase );
		UpdateState(e);
		UpdateConfirm(e);
	}

	public void OnFucus( )
	{
		
	}

	void UpdateState(UIHoverEvent e)
	{
		if ( State == VideoInfoUnitState.Normal ) {
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			{
				videoPost.transform.DOScale( hoverAnimation.scale , hoverAnimation.duration );

//				if ( videoName.GetComponent<Outline>() )
//					videoName.GetComponent<Outline>().enabled = true;

				m_state = VideoInfoUnitState.Hovered;
			}
		}
		if ( State == VideoInfoUnitState.Hovered )
		{

			if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				videoPost.transform.DOKill();
				videoPost.transform.DOScale( Vector3.one , hoverAnimation.duration );

//				if ( videoName.GetComponent<Outline>() )
//					videoName.GetComponent<Outline>().enabled = false;
				m_state = VideoInfoUnitState.Normal;
			}
		}
	}

	float confirmProcess = 0 ;
	void UpdateConfirm( UIHoverEvent e )
	{
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Middle )
		{
			if ( tempSensor.FocusTime >0 )
			{
				confirmAni.confirmRing.fillAmount =
					confirmAni.confirmCurve.Evaluate( tempSensor.FocusTime / tempSensor.GetTotalConfirmTime());
			}
		}
		if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
		{
			confirmAni.confirm.DOKill();
			confirmAni.confirmRing.DOKill();
			confirmAni.confirm.transform.DOKill();
			float time = tempSensor.GetTotalFocusTime();
			confirmAni.confirm.enabled = true;
			confirmAni.confirmRing.enabled = true;
			confirmAni.confirm.DOFade( 1f , time );
			confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , 0 );
			confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY , time );
		}else if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
		{
			float time = tempSensor.GetTotalFocusTime() / 2f;
			confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , time );
			confirmAni.confirm.DOFade( 0 , time  );
			confirmAni.confirmRing.DOFillAmount( 0 , time ).OnComplete(ResetConfirm);
		}
		
	}

	void ResetConfirm()
	{
		confirmAni.confirmRing.fillAmount = 0;
		Color col = confirmAni.confirm.color ;
		col.a = 0.01f;
		confirmAni.confirm.color = col;
		confirmAni.confirmRing.enabled = false;
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

[System.Serializable]
public struct VideoUnitConfirmAnimation
{
	public Image confirm;
	public Image confirmRing;
	public AnimationCurve confirmCurve;
	public float posY;
					public float moveY;
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