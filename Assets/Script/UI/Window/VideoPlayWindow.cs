using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;


public class VideoPlayWindow : VRUIWindow {

	[SerializeField] GameObject screenPrefab;
	[SerializeField] GameObject liveScreenPrefab;
	MediaPlayerCtrl videoPlayer;

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
	public struct VoiceInteraction
	{
		public VoiceButton voiceButton;
		public VRBasicButton confirmButton;
		public VRBasicButton cancleButton;
		public Text voiceText;
		public Image backImage;
		public float fadeInDuration;
		public float fadeOutDuration;
	}
	[SerializeField] VoiceInteraction m_voiceInteraction;
	ChatArg temMsg;

	[System.Serializable]
	public struct LoadAnimation
	{
		public Image IconFrame;
		public Image EyeIcon;
		public float fadeDuration;
	}
	[SerializeField] LoadAnimation m_loadAnimation;
	[SerializeField] VRBasicButton[] panelButtons;

	void Start()
	{
		FollowView.gameObject.SetActive ( false );
//		panelButtons = FollowView.gameObject.GetComponentsInChildren<VRBasicButton>();
		HidePlayPanelButtons(0);
		HideLoadAnimation();
		HideVoiceComponents(0);
	}

	override protected void OnDisable()
	{
		base.OnDisable();
		VREvents.PlayVideo -= OnPlayVideoEvent;
		VREvents.VoiceRecord -= OnVoiceRecord;
		VREvents.ReciveTranslatedMessage -= OnRecieveTranslatedMessage;
		VREvents.ChatMessageRecieve -= OnChatMessageRecieve;
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.PlayVideo += OnPlayVideoEvent;
		VREvents.VoiceRecord += OnVoiceRecord;
		VREvents.ReciveTranslatedMessage += OnRecieveTranslatedMessage;
		VREvents.ChatMessageRecieve += OnChatMessageRecieve;
	}

	void OnChatMessageRecieve (ChatArg msg)
	{
		VREvents.FireChatMessage( msg );
	}

	void OnRecieveTranslatedMessage (ChatArg msg)
	{
		isWaittingForMessage = false;

		float dur = m_voiceInteraction.fadeInDuration;
		m_voiceInteraction.confirmButton.OnBecomeVisible(dur,true);
		m_voiceInteraction.cancleButton.OnBecomeVisible(dur,true);
		m_voiceInteraction.voiceText.text = msg.message;
		temMsg = msg;

	}

	void OnVoiceRecord (Message msg)
	{
		bool isOn = (bool)msg.GetMessage("isOn");
		if ( isOn == false ) // end record
		{
			float dur = m_voiceInteraction.fadeInDuration;
			m_voiceInteraction.backImage.DOFade( 0.5f , dur );
			m_voiceInteraction.voiceText.DOFade( 1f , dur );

			StartCoroutine ( WaitForMessage() );
		}
	}

	bool isWaittingForMessage = false;
	IEnumerator WaitForMessage()
	{
		isWaittingForMessage = true;
		m_voiceInteraction.voiceText.text = "。";

		while( true  )
		{
			yield return new WaitForSeconds(1f);

			if ( !isWaittingForMessage ) 
				yield break;
				
			m_voiceInteraction.voiceText.text = m_voiceInteraction.voiceText.text + "。";
			if ( m_voiceInteraction.voiceText.text.Length > 3 )
				m_voiceInteraction.voiceText.text = "。";
		}
	}

	public void OnVoiceConfirm()
	{
		if ( temMsg != null )
		{
			temMsg.cameraForward = Camera.main.transform.forward;
			VREvents.FirePostChatMessageToServer( temMsg );
		}

		OnVoiceCancle();
	}

	public void OnVoiceCancle()
	{
		m_voiceInteraction.voiceButton.Reset();
		float dur = m_voiceInteraction.fadeOutDuration;
		HideVoiceComponents( dur );
	}

	public void HideVoiceComponents( float dur )
	{
		Debug.Log("Hide Voice Components");
		m_voiceInteraction.cancleButton.OnBecomeInvisible(dur,true);
		m_voiceInteraction.confirmButton.OnBecomeInvisible(dur,true);
		m_voiceInteraction.backImage.DOFade(0,dur);
		m_voiceInteraction.voiceText.DOFade(0,dur);
	}

	void OnPlayVideoEvent (Message msg)
	{
		VideoInfo info = (VideoInfo)msg.GetMessage(Global.MSG_VIDEO_INFO_KEY);
		Debug.Log("Play Video " + info.title + " " + info.playUrl );

		StartCoroutine( PlayVideoFake( info , 3f ));
		UpdateScreen(info);

	}

	void UpdateScreen( VideoInfo info )
	{
		if ( videoPlayer != null )
		{
			// remove the old video
			videoPlayer.gameObject.SetActive( false );
		}
			
		GameObject screen = Instantiate( info.isLive ? liveScreenPrefab : screenPrefab ) as GameObject;
		screen.transform.SetParent( transform , true );
		screen.transform.position = Vector3.zero;

		videoPlayer = screen.GetComponent<MediaPlayerCtrl>();
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );

		if ( videoPlayer != null )
			videoPlayer.gameObject.SetActive(true);
		ShouldUpdate = true;

		FollowView.enabled = true;

		FollowView.gameObject.SetActive( true );
		lastDegree = 90f;
		HideVoiceComponents(0);
	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );

		if ( videoPlayer != null )
			videoPlayer.gameObject.SetActive(false);
		ShouldUpdate = false;

		FollowView.enabled = false;

		PlayPanelBack.DOFade( 0 , time ).OnComplete( DisablePlayPanel );

		// all buttons become invisible
		VRBasicButton[] buttons = FollowView.GetComponentsInChildren<VRBasicButton>();
		foreach( VRBasicButton btn in buttons )
		{
			btn.OnBecomeInvisible( time );
		}
	}

	public void DisablePlayPanel()
	{
		FollowView.gameObject.SetActive( false );
	}

	public void OnPlayVideo()
	{
		if ( videoPlayer != null )
			videoPlayer.Play();
		playButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(true);
	}

	IEnumerator PlayVideoFake( VideoInfo info , float delay )
	{
		UpdateScreen(info);
		ShowLoadAnimation();

		yield return new WaitForSeconds( 1f );

//		videoPlayer.DownloadStreamingVideoAndLoad( info.playUrl );
//		videoPlayer.m_strFileName = info.playUrl;
		videoPlayer.Load( info.playUrl );

		yield return new WaitForSeconds( 1f );

		videoPlayer.Play();

		while( videoPlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY )
		{
			yield return null;
		}

		HideLoadAnimation();
		OnPlayVideo();
	}

	public void OnPauseVideo()
	{
		if ( videoPlayer != null )
			videoPlayer.Pause();
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

			foreach( VRBasicButton btn in panelButtons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeVisible(duration * 0.5f);
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
			foreach( VRBasicButton btn in panelButtons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeInvisible(duration * 0.5f );
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