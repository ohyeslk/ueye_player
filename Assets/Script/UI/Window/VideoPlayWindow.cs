using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class VideoPlayWindow : UIWindow {

	[SerializeField] MediaPlayerCtrl video;

	[SerializeField] VRBasicButton playButton;
	[SerializeField] VRBasicButton pauseButton;
	[SerializeField] VRBasicButton ExpandButton;

	[SerializeField] FollowView FollowView;
	[SerializeField] Image PlayPanelBack;

	/// <summary>
	/// when the camera degree ( from V3.down ) equals ShowButtonDegree,
	/// show the buttons on the play panel
	/// </summary>
	[SerializeField] float ShowButtonDegree = 70f;
	/// <summary>
	/// When the camera degree ( from V3.down ) is smaller than ShowPanelDegree,
	/// begin to show the play panel
	/// </summary>
	[SerializeField] float ShowPanelDegree = 80f;
	[SerializeField] float ButtonShowTime = 1f;
	[SerializeField] float ButtonHideTime = 0.5f;

	[System.Serializable]
	public struct LoadAnimation
	{
		public Image IconFrame;
		public Image EyeIcon;
		public float fadeDuration;
	}
	[SerializeField] LoadAnimation m_loadAnimation;
	VRBasicButton[] buttons;

	void Start()
	{
		FollowView.gameObject.SetActive ( false );
		video.gameObject.SetActive(false);
		video.Pause();
		buttons = FollowView.gameObject.GetComponentsInChildren<VRBasicButton>();
		HidePlayPanelButtons(0);
		HideLoadAnimation();
	}

	override protected void OnDisable()
	{
		base.OnDisable();
		VREvents.PlayVideo -= OnPlayVideoEvent;
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.PlayVideo += OnPlayVideoEvent;
	}

	void OnPlayVideoEvent (Message msg)
	{
		VideoInfo info = (VideoInfo)msg.GetMessage(Global.MSG_VIDEO_INFO_KEY);
		Debug.Log("Play Video " + info.title + " " + info.playUrl );

		StartCoroutine( PlayVideoFake( info , 3f ));
//		OnPlayVideo();
	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );
		BecomeVisible ( false , time  );
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );
		BecomeVisible ( true , time );
	}

	void BecomeVisible( bool to , float time )
	{
		video.gameObject.SetActive(to);
		ShouldUpdate = to;

		FollowView.enabled = to;

		if ( to )
		{
			FollowView.gameObject.SetActive( true );
			lastDegree = 90f;


		}else
		{
			PlayPanelBack.DOFade( 0 , time ).OnComplete( DisablePlayPanel );
			HidePlayPanelButtons( time );
			ExpandButton.OnBecomeInvisible( time );
		}
	}

	public void DisablePlayPanel()
	{
		FollowView.gameObject.SetActive( false );
	}

	public void OnPlayVideo()
	{
		video.Play();
		playButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(true);
	}

	IEnumerator PlayVideoFake( VideoInfo info , float delay )
	{

		ShowLoadAnimation();
//		video.DownloadStreamingVideoAndLoad( info.playUrl );

		video.enabled = false;

		yield return new WaitForSeconds( 1f );

		video.enabled = true;

		video.Load( info.playUrl );

		while( video.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY )
		{
			yield return null;
		}

		HideLoadAnimation();
		OnPlayVideo();
	}

	public void OnPauseVideo()
	{
		video.Pause();
		playButton.gameObject.SetActive(true);
		pauseButton.gameObject.SetActive(false);
	}

	public void OnReturn()
	{
		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.SELECT_WINDOW;
		VREvents.FireActiveWindow( arg );
	}

	float lastDegree = 90f;
	/// <summary>
	/// Should Update if become visible
	/// </summary>
	bool ShouldUpdate = false;
	public void LateUpdate()
	{
		if ( ShouldUpdate  )
		{
			float degree = Vector3.Angle( Camera.main.transform.forward , Vector3.down );
			//if ( !PlayPanelExpanded )
			{
				// update teh color of the PlayPanelBack
				if ( degree < ShowPanelDegree ) {
					{
					Color col = PlayPanelBack.color;
					col.a = Mathf.Clamp01( ( ShowPanelDegree - degree ) / ( ShowPanelDegree - ShowButtonDegree )) * 0.33f;
					PlayPanelBack.color = col;
					}
					{
					Color col = ExpandButton.subButtonAnimation.subButton.color;
					col.a = Mathf.Clamp01( ( ShowPanelDegree - degree ) / ( ShowPanelDegree - ShowButtonDegree ));
					ExpandButton.subButtonAnimation.subButton.color = col;
					}
				}
			}

			lastDegree = degree;
		}
	}

	public void OnExpand()
	{
		if( PlayPanelExpanded )
		{
			HidePlayPanelButtons( ButtonHideTime );
			FollowView.enabled = true;
		}else
		{
			ShowPlayPanelButtons( ButtonShowTime );
			FollowView.enabled = false;
		}
	}

	public void ShowLoadAnimation() {

		m_loadAnimation.EyeIcon.DOFade( 1f , m_loadAnimation.fadeDuration );
		m_loadAnimation.IconFrame.DOFade( 1f , m_loadAnimation.fadeDuration );
	}

	public void HideLoadAnimation() {

		m_loadAnimation.EyeIcon.DOFade( 0 , m_loadAnimation.fadeDuration );
		m_loadAnimation.IconFrame.DOFade( 0 , m_loadAnimation.fadeDuration );
	}

	bool PlayPanelExpanded = false;
	/// <summary>
	/// Play the Show Planel Buttons Animation and set PlayPanelExpanded to true
	/// </summary>
	/// <param name="time">Show duration.</param>
	public void ShowPlayPanelButtons( float duration )
	{
		{
			StartCoroutine( ShowPlayPanelButtonsDo(duration) );
			PlayPanelExpanded = true;
		}
	}

	IEnumerator ShowPlayPanelButtonsDo(float duration)
	{
		{
			RectTransform recTrans = (RectTransform) transform;
			Sequence seq = DOTween.Sequence();
			PlayPanelBack.transform.DOKill();
			ExpandButton.transform.DOKill();
			seq.Append( PlayPanelBack.transform.DOScaleX( 1f , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalMoveY( -300f , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalRotate( new Vector3( 0 , 0 , 90f ) , duration * 0.5f ));
	
			yield return new WaitForSeconds( duration * 0.5f );

			foreach( VRBasicButton btn in buttons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeVisible(duration * 0.5f);
//					btn.transform.rotation = Quaternion.LookRotation( Camera.main.transform.position - btn.transform.position );
				}
			}
		}
		yield break;
	}

	/// <summary>
	/// Play the Hide Planel Buttons Animation and set PlayPanelExpanded to false
	/// </summary>
	/// <param name="time">Show duration.</param>
	public void HidePlayPanelButtons( float duration )
	{
		{
			StartCoroutine( HidePlayPanelButtonsDo(duration) );
			PlayPanelExpanded = false;
		}
	}

	IEnumerator HidePlayPanelButtonsDo(float duration)
	{
		{
			foreach( VRBasicButton btn in buttons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeInvisible(duration * 0.5f );
//					btn.transform.rotation = Quaternion.LookRotation( Camera.main.transform.position - btn.transform.position );
				}
			}

			yield return new WaitForSeconds( duration * 0.5f );

			RectTransform recTrans = (RectTransform) transform;
			Sequence seq = DOTween.Sequence();
			PlayPanelBack.transform.DOKill();
			PlayPanelBack.transform.DOKill();
			ExpandButton.transform.DOKill();
			seq.Append( PlayPanelBack.transform.DOScaleX( 0 , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalMoveY( 0 , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalRotate( new Vector3( 0 , 0 , -90f ) , duration * 0.5f ));
		}
		yield break;
	}

}