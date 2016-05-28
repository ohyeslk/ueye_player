using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class VideoPlayWindow : UIWindow {

	[SerializeField] MediaPlayerCtrl video;

	[SerializeField] GameObject playButton;
	[SerializeField] GameObject pauseButton;

	[SerializeField] GameObject PlayPanel;
	[SerializeField] Image PlayPanelBack;
	/// <summary>
	/// when the camera degree ( from V3.down ) equals ShowButtonDegree,
	/// show the buttons on the play panel
	/// </summary>
	[SerializeField] float ShowButtonDegree = 30f;
	/// <summary>
	/// When the camera degree ( from V3.down ) is smaller than ShowPanelDegree,
	/// begin to show the play panel
	/// </summary>
	[SerializeField] float ShowPanelDegree = 45f;
	[SerializeField] float ButtonShowTime = 1f;
	[SerializeField] float ButtonHideTime = 0.5f;
	[SerializeField] VRBasicButton ExpandButton;

	VRBasicButton[] buttons;
	void Start()
	{
		PlayPanel.gameObject.SetActive ( false );
		video.gameObject.SetActive(false);
		video.Pause();
		buttons = PlayPanel.GetComponentsInChildren<VRBasicButton>();
		Debug.Log("Buttons " + buttons.Length );
		HidePlayPanelButtons(0);
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
		video.Load( info.playUrl );
		OnPlayVideo();
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
//		video.gameObject.SetActive(to);
		Debug.Log("Become Visible " + to );
		ShouldUpdate = to;
		if ( to )
		{
			PlayPanel.SetActive( true );
			lastDegree = 90f;
		}else
		{
			PlayPanelBack.DOFade( 0 , time ).OnComplete( DisablePlayPanel );
			HidePlayPanelButtons( time );
		}
	}

	public void DisablePlayPanel()
	{
		PlayPanel.gameObject.SetActive( false );
	}

	public void OnPlayVideo()
	{
		video.Play();
		playButton.SetActive(false);
		pauseButton.SetActive(true);
	}
	public void OnPauseVideo()
	{
		video.Pause();
		playButton.SetActive(true);
		pauseButton.SetActive(false);
	}

	public void OnReturn()
	{
		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.SELECT_WINDOW;
		VREvents.FireActiveWindow( arg );
	}

	float lastDegree = 90f;
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

			// Call show buttons as soon as the degree is lower than the show button degree
//			if ( lastDegree > ShowButtonDegree && degree < ShowButtonDegree )
//			{
//				ShowPlayPanelButtons( ButtonShowTime );
//			}

			// call hide buttons as soon as the degree is higher than the show panel degree
//			if ( lastDegree < ShowPanelDegree && degree > ShowPanelDegree )
//			{
//				HidePlayPanelButtons( ButtonHideTime );
//			}

			lastDegree = degree;
		}
	}

	public void OnExpand()
	{
		if( PlayPanelExpanded )
		{
			HidePlayPanelButtons( ButtonHideTime );
		}else
		{
			ShowPlayPanelButtons( ButtonShowTime );
		}
	}

	bool PlayPanelExpanded = false;
	public void ShowPlayPanelButtons( float time )
	{
		{
			StartCoroutine( ShowPlayPanelButtonsDo(time) );
			PlayPanelExpanded = true;
		}
	}

	IEnumerator ShowPlayPanelButtonsDo(float time)
	{
		{
			RectTransform recTrans = (RectTransform) transform;
			Sequence seq = DOTween.Sequence();
			PlayPanelBack.transform.DOKill();
			ExpandButton.transform.DOKill();
			seq.Append( PlayPanelBack.transform.DOScaleX( 1f , time * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalMoveY( -250f , time * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalRotate( new Vector3( 0 , 0 , 90f ) , time * 0.5f ));
	
			yield return new WaitForSeconds( time * 0.5f );

			foreach( VRBasicButton btn in buttons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeVisible(time * 0.5f);
//					btn.transform.rotation = Quaternion.LookRotation( Camera.main.transform.position - btn.transform.position );
				}
			}
		}
		yield break;
	}

	public void HidePlayPanelButtons( float time )
	{
		{
			StartCoroutine( HidePlayPanelButtonsDo(time) );
			PlayPanelExpanded = false;
		}
	}

	IEnumerator HidePlayPanelButtonsDo(float time)
	{
		{
			foreach( VRBasicButton btn in buttons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeInvisible(time * 0.5f );
//					btn.transform.rotation = Quaternion.LookRotation( Camera.main.transform.position - btn.transform.position );
				}
			}

			yield return new WaitForSeconds( time * 0.5f );

			RectTransform recTrans = (RectTransform) transform;
			Sequence seq = DOTween.Sequence();
			PlayPanelBack.transform.DOKill();
			PlayPanelBack.transform.DOKill();
			ExpandButton.transform.DOKill();
			seq.Append( PlayPanelBack.transform.DOScaleX( 0 , time * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalMoveY( 0 , time * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalRotate( new Vector3( 0 , 0 , -90f ) , time * 0.5f ));
		}
		yield break;
	}

}