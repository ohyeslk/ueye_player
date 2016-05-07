using UnityEngine;
using System.Collections;

public class VideoPlayWindow : UIWindow {

	[SerializeField] GameObject panel;
	[SerializeField] MediaPlayerCtrl video;

	[SerializeField] UIButton playButton;
	[SerializeField] UIButton pauseButton;

	void Start()
	{
		panel.SetActive ( false );
		video.gameObject.SetActive(false);
		video.Pause();
	}

	void OnDisable()
	{
		VREvents.ActiveWindow -= VREvents_ActiveWindow;
		VREvents.PlayVideo -= OnPlayVideoEvent;
	}

	void OnEnable()
	{
		VREvents.ActiveWindow += VREvents_ActiveWindow;
		VREvents.PlayVideo += OnPlayVideoEvent;
	}

	void OnPlayVideoEvent (Message msg)
	{
		VideoInfo info = (VideoInfo)msg.GetMessage(Global.MSG_PLAYVIDEO_INFO_KEY);
		BecomeVisible(true);
		OnPlayVideo();
	}



	void VREvents_ActiveWindow (WindowArg arg)
	{
		if ( arg.type == WindowArg.Type.PLAY_WINDOW )
		{
			BecomeVisible( true );
			OnPlayVideo();
		}else{
			BecomeVisible( false );
			OnPauseVideo();
		}
	}

	void BecomeVisible( bool to )
	{
		panel.SetActive(to);
		video.gameObject.SetActive(to);
	}

	public void OnPlayVideo()
	{
		video.Play();
		playButton.gameObject.SetActive(false);
		pauseButton.gameObject.SetActive(true);
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

}
