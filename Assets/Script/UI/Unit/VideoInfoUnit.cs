using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class VideoInfoUnit : MonoBehaviour {
	[SerializeField] Image videoPost;
	[SerializeField] Text videoName;
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
		if ( videoName != null )
			videoName.text = info.title;

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
		
	public void OnConfirm ()
	{
		Debug.Log("[On Confirm]" + name);
		return;
		parent.PlayVideo( Info );
	}

	public void OnHover(UIHoverEvent e)
	{
		UpdateState(e);
	}

	/// <summary>
	/// Show the confirm button 
	/// </summary>
	public void OnShowConfirm()
	{
		confirmAni.confirm.DOKill();
		confirmAni.confirmRing.DOKill();
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.showTime;
		confirmAni.confirm.enabled = true;
		confirmAni.confirmRing.enabled = true;
		confirmAni.confirm.DOFade( 1f , time );
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , 0 );
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY , time );

	}

	/// <summary>
	/// Hide the confirm button
	/// </summary>
	public void OnHideConfirm()
	{
		confirmAni.confirm.DOKill();
		confirmAni.confirmRing.DOKill();
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.hideTime;
		confirmAni.confirm.transform.DOLocalMoveY( confirmAni.posY + confirmAni.moveY , time );
		confirmAni.confirm.DOFade( 0 , time  );
		confirmAni.confirmRing.DOFillAmount( 0 , time ).OnComplete(ResetConfirm);
	}

	/// <summary>
	/// hide the confirm ring
	/// </summary>
	public void OnHideConfirmRing()
	{
		confirmAni.confirm.transform.DOKill();
		float time = confirmAni.hideTime;
		confirmAni.confirmRing.DOFillAmount( 0 , time );
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
				m_state = VideoInfoUnitState.Hovered;
			}
		}
		if ( State == VideoInfoUnitState.Hovered )
		{

			if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				videoPost.transform.DOKill();
				videoPost.transform.DOScale( Vector3.one , hoverAnimation.duration );
				m_state = VideoInfoUnitState.Normal;
			}
		}
	}

	public void UpdateConfirmRing( float process )
	{
		confirmAni.confirmRing.fillAmount = confirmAni.confirmCurve.Evaluate( process );
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
	public float showTime;
	public float hideTime;
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