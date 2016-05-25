using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DetailWindow : UIWindow {

	[SerializeField] Image img;
	[SerializeField] Text title;
	[SerializeField] Text description;
	[SerializeField] GameObject panel;
	[SerializeField] Image backImage;
	[SerializeField] Image playImage;
	[SerializeField] Sprite defaulSprite;

	VideoInfo m_info;

	[System.Serializable]
	struct DetailWindowSetting
	{
		public float showDuration;
		public float hideDuration;
	}
	[SerializeField] DetailWindowSetting m_setting;

	override protected void OnDisable()
	{
		base.OnDisable();
		VREvents.ShowDetail -= OnShowDetail;
		VREvents.PostTexture -= RecieveTexture;
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.ShowDetail += OnShowDetail;
		VREvents.PostTexture += RecieveTexture;
	}

	protected override void OnBecomeVisible ()
	{
		Debug.Log(" Detail Becom visible");
		base.OnBecomeVisible ();
		PlayInitAnimation();
	}

	protected override void OnBecomeInvsible ()
	{
		base.OnBecomeInvsible ();
		PlayExitAnimation();
	}

	void RecieveTexture( URLRequestMessage msg )
	{
		if ( msg.postObj == this )
		{
			Texture2D tex = (Texture2D)msg.GetMessage(Global.MSG_REQUEST_TEXTURE_TEXTURE_KEY);
			Rect rec = new Rect(0,0,tex.width ,tex.height );
			m_info.Post = Sprite.Create( tex , rec , new Vector2(0.5f,0.5f) , 100);

			img.sprite = m_info.Post;
		}
	}
	/// <summary>
	/// save the video info
	/// </summary>
	/// <param name="msg">Message.</param>
	void OnShowDetail( Message msg )
	{
		if ( msg.GetMessage( Global.MSG_VIDEO_INFO_KEY ) != null )
		{
			m_info = (VideoInfo) msg.GetMessage(Global.MSG_VIDEO_INFO_KEY );

			if ( m_info.Post != null )
			{
				img.sprite = m_info.Post;
			}else
			{
				// initilize the sprite first
				URLRequestMessage coverMsg = new URLRequestMessage(this);
				coverMsg.url = m_info.coverUrl;
				VREvents.FireRequesTexture( coverMsg );
			}
			description.text = m_info.description;
			title.text = m_info.title;
		}else{
			Debug.Log("【Error】unable to get the video info");
		}
	}

	/// <summary>
	/// Play the initilzation animation
	/// </summary>
	void PlayInitAnimation()
	{
		// enable the components
		panel.gameObject.SetActive ( true );
		description.text = "";

		Sequence seq = DOTween.Sequence();
		seq.Append( img.DOFade( 0 , 0 ) );
		seq.Join( description.DOFade( 0 , 0 ));
		seq.Join( title.DOFade( 0 , 0 ));
		seq.Join( backImage.DOFade( 0 , 0 ));
		seq.Join( playImage.DOFade( 0 , 0 ));
		seq.Append( img.DOFade( 1f , m_setting.showDuration ) );
		seq.Join( description.DOFade( 1f , m_setting.showDuration ) );
		seq.Join( description.DOText( m_info.description , m_setting.showDuration ));
		seq.Join( backImage.DOFade( 1f , m_setting.hideDuration ));
		seq.Join( playImage.DOFade( 1f , m_setting.hideDuration ));
		seq.Join( title.DOFade( 1f , m_setting.showDuration ));
	}

	/// <summary>
	/// Play the exit animation
	/// </summary>
	void PlayExitAnimation()
	{
		Sequence seq = DOTween.Sequence();
		seq.Append( img.DOFade( 0f , m_setting.hideDuration ) );
		seq.Join( description.DOFade( 0f , m_setting.hideDuration ) );
		seq.Join( description.DOText( "" , m_setting.hideDuration ));
		seq.Join( title.DOFade( 0f , m_setting.hideDuration ));
		seq.Join( backImage.DOFade( 0 , m_setting.hideDuration ));
		seq.Join( playImage.DOFade( 0 , m_setting.hideDuration ));
		seq.AppendCallback( ExitComplete );

	}

	/// <summary>
	/// Called when PlayExitAnimation finished ( set image and text to false)
	/// </summary>
	void ExitComplete()
	{
		img.sprite = defaulSprite;
		panel.gameObject.SetActive( false );
	}
		
	/// <summary>
	/// for function button, play the video according to video info
	/// </summary>
	public void OnPlayVideo()
	{
		Debug.Log("Play video " + m_info.playUrl );
		Message msg = new Message(this);
		msg.AddMessage( Global.MSG_VIDEO_INFO_KEY , m_info );
		VREvents.FirePlayVideo( msg );

		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.PLAY_WINDOW;
		VREvents.FireActiveWindow( arg );
	}

	/// <summary>
	/// for function button, back to the select window
	/// </summary>
	public void OnBack()
	{
		WindowArg arg = new WindowArg(this);
		arg.type = WindowArg.Type.SELECT_WINDOW;
		VREvents.FireActiveWindow( arg );
	}
}
