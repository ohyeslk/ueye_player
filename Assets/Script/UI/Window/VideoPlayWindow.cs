using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class VideoPlayWindow : VRUIWindow {

	[SerializeField] GameObject screenPrefab;
	[SerializeField] GameObject liveScreenPrefab;
	[SerializeField] GameObject presentPrefab;
	[SerializeField] VoteWindow voteWindow;
	MediaPlayerCtrl videoPlayer;
	VideoInfo tempInfo;

	[SerializeField] VRBasicButton playButton;
	[SerializeField] VRBasicButton pauseButton;
	[SerializeField] VRBasicButton ExpandButton;
	[SerializeField] VRBasicButton refreshButton;

	[SerializeField] FollowView FollowView;
	[SerializeField] GameObject AdsPanel;
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
		public float maxWaitTime;
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
	[SerializeField] List<VRBasicButton> panelButtons;

	void Start()
	{
		FollowView.gameObject.SetActive ( false );
		AdsPanel.SetActive( false );
//		panelButtons = FollowView.gameObject.GetComponentsInChildren<VRBasicButton>();
		HidePlayPanel(0);
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
		VREvents.VoteCreated -= OnVoteCreated;

	}


	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.PlayVideo += OnPlayVideoEvent;
		VREvents.VoiceRecord += OnVoiceRecord;
		VREvents.ReciveTranslatedMessage += OnRecieveTranslatedMessage;
		VREvents.ChatMessageRecieve += OnChatMessageRecieve;
		VREvents.VoteCreated += OnVoteCreated;
	}

	void OnVoteCreated (VoteArg msg)
	{
		//TODO : donot create the vote window
//		voteWindow.gameObject.SetActive( true );
//		voteWindow.Init( msg );
	}

	void OnChatMessageRecieve (ChatArg msg)
	{
		VREvents.FireShowChatMessage( msg );
	}

	void OnRecieveTranslatedMessage (ChatArg msg)
	{
		temMsg = msg;
		ShowChatMessageResult( msg );
	}

	void ShowChatMessageResult( ChatArg msg )
	{
		if ( isWaittingForMessage == true )
		{
			isWaittingForMessage = false;

			float dur = m_voiceInteraction.fadeInDuration;
			m_voiceInteraction.confirmButton.OnBecomeVisible(dur,true);
			m_voiceInteraction.cancleButton.OnBecomeVisible(dur,true);
			m_voiceInteraction.voiceText.text = msg.message;
		}
	}

	void OnVoiceRecord (Message msg)
	{
		bool isOn = (bool)msg.GetMessage("isOn");
		if ( isOn == false ) // end record
		{
			float dur = m_voiceInteraction.fadeInDuration;
			m_voiceInteraction.backImage.DOFade( 0.5f , dur );
			m_voiceInteraction.voiceText.DOFade( 1f , dur );

			if ( waitForMesage != null )
			{
				StopCoroutine( waitForMesage );
			}

			waitForMesage = StartCoroutine ( WaitForMessage() );
		}
	}

	Coroutine waitForMesage = null;
	bool isWaittingForMessage = false;
	IEnumerator WaitForMessage()
	{
		isWaittingForMessage = true;
		m_voiceInteraction.voiceText.text = "。";
		float startTime = Time.time;

		while( true )
		{
			yield return new WaitForSeconds(0.66f);

			if ( !isWaittingForMessage ) 
				yield break;
				
			m_voiceInteraction.voiceText.text = m_voiceInteraction.voiceText.text + "。";
			if ( m_voiceInteraction.voiceText.text.Length > 3 )
				m_voiceInteraction.voiceText.text = "。";

			if ( Time.time - startTime > m_voiceInteraction.maxWaitTime )
			{
				ChatArg msg = new ChatArg(this);
				msg.message = "我不懂你在说什么";
				msg.userName = UserManager.UserName;
				msg.cameraForward = Camera.main.transform.forward;

				ShowChatMessageResult( msg );

				temMsg = msg;
			}
		}
	}

	public void OnVoiceConfirm()
	{
		if ( temMsg != null )
		{
			temMsg.cameraForward = Camera.main.transform.forward;
			VREvents.FirePostChatMessageToServer( temMsg );
			temMsg = null;

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
		m_voiceInteraction.voiceButton.Reset();
	}

	void OnPlayVideoEvent (Message msg)
	{
		tempInfo = (VideoInfo)msg.GetMessage(Global.MSG_VIDEO_INFO_KEY);
//		Debug.Log("Play Video " + tempInfo.title + " " + tempInfo.playUrl );

		StartCoroutine( PlayVideoFake( tempInfo , 1f ) );

	}

	IEnumerator PlayVideoFake( VideoInfo info , float delay )
	{
		UpdateScreen(info);

		if ( videoPlayer == null )
			yield break;

		videoPlayer.UnLoad();

		ShowLoadAnimation();

		yield return new WaitForSeconds( delay );

		//		videoPlayer.DownloadStreamingVideoAndLoad( info.playUrl );
		//		videoPlayer.m_strFileName = info.playUrl;

		videoPlayer.Load( info.playUrl );

		OnPlayVideo();

		while( videoPlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.NOT_READY )
		{

			yield return null;
		}

		HideLoadAnimation();
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
		screen.transform.position = ( info.isLive ? Vector3.back * 14f : Vector3.zero);

		videoPlayer = screen.GetComponent<MediaPlayerCtrl>();

		// update the play buttons
		UpdatePlayButtonActive( );

		if ( info.isLive )
		{
			AdsPanel.SetActive( true );
		}
	}

	void UpdatePlayButtonActive( )
	{
		Debug.Log("Update Play Button ");

		if ( panelButtons.Contains( playButton ) )
			panelButtons.Remove( playButton );
		if ( panelButtons.Contains( pauseButton ) )
			panelButtons.Remove( pauseButton );
		if ( panelButtons.Contains( refreshButton ) )
			panelButtons.Remove( refreshButton );
		
		VRBasicButton target = null;
		if ( tempInfo.isLive )
		{
			playButton.gameObject.SetActive( false );
			pauseButton.gameObject.SetActive( false );
			refreshButton.gameObject.SetActive( true );
			panelButtons.Add( refreshButton );
		}else
		{
			refreshButton.gameObject.SetActive( false );
			bool isPaused = videoPlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PAUSED;
			playButton.gameObject.SetActive( isPaused );
			pauseButton.gameObject.SetActive( !isPaused );

			panelButtons.Add( playButton );
			panelButtons.Add ( pauseButton );
		}

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
		HidePlayPanel( 0 );


	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );

		if ( videoPlayer != null )
			videoPlayer.gameObject.SetActive(false);
		ShouldUpdate = false;

		FollowView.enabled = false;

		PlayPanelBack.DOFade( 0 , time ).OnComplete( DisablePlayPanel );

		HidePlayPanel( ButtonShowTime );

		// all buttons become invisible
		VRBasicButton[] buttons = FollowView.GetComponentsInChildren<VRBasicButton>();
		foreach( VRBasicButton btn in buttons )
		{
			btn.OnBecomeInvisible( time );
		}

		AdsPanel.SetActive( false );
	}

	public void DisablePlayPanel()
	{
		FollowView.gameObject.SetActive( false );
	}

	public void OnRefresh()
	{
		StartCoroutine( PlayVideoFake( tempInfo , 1f ) );
	}

	public void OnPlayVideo()
	{
		if ( videoPlayer != null )
			videoPlayer.Play();

		UpdatePlayButtonActive();


	}

	public void OnPauseVideo()
	{
		if ( videoPlayer != null )
			videoPlayer.Pause();
		
		UpdatePlayButtonActive();

//		playButton.gameObject.SetActive(true);
//		pauseButton.gameObject.SetActive(false);
	}

	public void OnReturn()
	{
		{
			WindowArg arg = new WindowArg(this);
			arg.type = WindowArg.Type.SELECT_WINDOW;
			VREvents.FireActiveWindow( arg );
		}

		{
			ChatArg msg = new ChatArg(this);
			VREvents.FireExitChanel(msg);
		}
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
				// TODO : update all button's alpha on the PlayPanelBack
				if ( degree < ShowPanelDegree ) {
//					{
//						Color col = PlayPanelBack.color;
//						col.a = Mathf.Clamp01( ( ShowPanelDegree - degree ) / ( ShowPanelDegree - ShowButtonDegree )) * 0.33f + 0.2f ;
//						PlayPanelBack.color = col;
//					}
					{
						Color col = ExpandButton.subButtonAnimation.subButton.color;
						col.a = Mathf.Clamp01( ( ShowPanelDegree - degree ) / ( ShowPanelDegree - ShowButtonDegree )) + 0.2f ;
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
			HidePlayPanel( ButtonHideTime );
		}else
		{
			ShowPlayPanel( ButtonShowTime );
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
	public void ShowPlayPanel( float duration )
	{
		{
			StartCoroutine( ShowPlayPanelButtonsDo(duration) );
			PlayPanelExpanded = true;
		}
		FollowView.enabled = false;
	}

	IEnumerator ShowPlayPanelButtonsDo(float duration)
	{
		{
			RectTransform recTrans = (RectTransform) transform;
			Sequence seq = DOTween.Sequence();
			PlayPanelBack.transform.DOKill();
			ExpandButton.transform.DOKill();
			seq.Append( PlayPanelBack.transform.DOScaleX( 1f , duration * 0.5f ));
			seq.Join ( PlayPanelBack.DOFade( 0.5f , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalMoveY( -300f , duration * 0.5f ));
			seq.Join( ExpandButton.transform.DOLocalRotate( new Vector3( 0 , 0 , 90f ) , duration * 0.5f ));
	
			yield return new WaitForSeconds( duration * 0.5f );

			UpdatePlayButtonActive();

			foreach( VRBasicButton btn in panelButtons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeVisible(duration * 0.5f , true );
				}
			}
		}
		yield break;
	}

	/// <summary>
	/// Play the Hide Planel Buttons Animation and set PlayPanelExpanded to false
	/// </summary>
	/// <param name="time">Show duration.</param>
	public void HidePlayPanel( float duration )
	{
		{
			StartCoroutine( HidePlayPanelButtonsDo(duration) );
			PlayPanelExpanded = false;
		}

		HideVoiceComponents( duration );

		FollowView.enabled = true;
	}

	IEnumerator HidePlayPanelButtonsDo(float duration)
	{
		{
			foreach( VRBasicButton btn in panelButtons )
			{
				if ( btn != ExpandButton )
				{
					btn.OnBecomeInvisible(duration * 0.5f , false);

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

	public void OnSwitch()
	{
		
	}

	public void OnMoveOut()
	{
		Vector3 pos = videoPlayer.transform.position;
		pos += Vector3.forward * 1f ;
		videoPlayer.transform.position = pos;
	}

	public void OnMoveIn()
	{
		Vector3 pos = videoPlayer.transform.position;
		pos += Vector3.back * 1f ;
		videoPlayer.transform.position = pos;
	}

	public void OnPresent()
	{
		if ( LogicManager.VRMode == VRMode.VR_2D )
		{
			CreatePresentAhead( presentPrefab );
		}
	}

	public void CreatePresentAhead( GameObject prefab )
	{
		GameObject present = Instantiate( prefab );
		present.transform.position = Camera.main.transform.forward.normalized * 1f ;

		Debug.Log("Forward " + Camera.main.transform.forward );

//		present.transform.DOMoveY( 0.5f , 2f ).From().SetRelative( true ).SetEase( Ease.InBack );

	}

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.H ) )
		{
			OnPresent ();
		}
	}


}