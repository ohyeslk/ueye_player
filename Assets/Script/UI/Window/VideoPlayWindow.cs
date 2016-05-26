using UnityEngine;
using System.Collections;

public class VideoPlayWindow : UIWindow {

	[SerializeField] GameObject panel;
	[SerializeField] MediaPlayerCtrl video;

	[SerializeField] GameObject playButton;
	[SerializeField] GameObject pauseButton;

	void Start()
	{
		panel.SetActive ( false );
		video.gameObject.SetActive(false);
		video.Pause();
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
//		Debug.Log("Play Video" + info.playUrl );
		OnPlayVideo();
	}

	protected override void OnBecomeInvsible ( float time )
	{
		base.OnBecomeInvsible ( time );
		BecomeVisible ( false );
	}

	protected override void OnBecomeVisible ( float time )
	{
		base.OnBecomeVisible ( time );
		BecomeVisible ( true );
	}

	void BecomeVisible( bool to )
	{
		panel.SetActive(to);
		video.gameObject.SetActive(to);
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

}
