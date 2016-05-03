using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class VideoInfoUnit : UISensor {
	[SerializeField] Image videoPost;
	[SerializeField] Text videoName;

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

	public VideoInfo Info{
		get { return m_info; }
	}

	/// <summary>
	/// record the start time of hovering 
               	/// </summary>

	public void Init(VideoInfo info , VideoUnitInitAnimation anim )
	{
		m_info = info;
		if ( videoPost != null )
			videoPost.sprite  = info.Post;
		if ( videoName != null )
			videoName.text = info.name;

		m_state = VideoInfoUnitState.Init;

		videoPost.transform.DOScale( Vector3.zero , anim.duration ).From().SetDelay(anim.delay);
		videoPost.transform.DORotate( new Vector3( 90, 90, 90 ) , anim.duration ).From().SetDelay(anim.delay );
		videoPost.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay );
		videoName.DOFade( 0 , anim.duration ).From().SetDelay(anim.delay ).OnComplete(CompleteInit);
	}

	void CompleteInit()
	{
		m_state = VideoInfoUnitState.Normal;
	}


	override public void OnHover(UIHoverEvent e)
	{
		base.OnHover(e);
		UpdateState(e);
	}

	override public void OnFocus()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		varg.type = UISensorArg.SensorType.VideoUnit;
		VREvents.FireUIFocus(varg);
	}

	override public void OnConfirm()
	{
		var varg = new UISensorArg(this);
		varg.focusTime = focusTime;
		varg.confirmTime = confirmTime;
		varg.type = UISensorArg.SensorType.VideoUnit;
		VREvents.FireUIConfirm(varg);
	}

	void UpdateState(UIHoverEvent e)
	{
		if ( State == VideoInfoUnitState.Normal ) {
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.Begin )
			{
				videoPost.transform.DOScale( hoverAnimation.scale , hoverAnimation.duration );

				if ( videoName.GetComponent<Outline>() )
					videoName.GetComponent<Outline>().enabled = true;
				m_state = VideoInfoUnitState.Hovered;
			}
		}
		if ( State == VideoInfoUnitState.Hovered )
		{
			if ( e.hoverPhase == UIHoverEvent.HoverPhase.End )
			{
				videoPost.transform.DOKill();
				videoPost.transform.DOScale( Vector3.one , hoverAnimation.duration );

				if ( videoName.GetComponent<Outline>() )
					videoName.GetComponent<Outline>().enabled = false;
				m_state = VideoInfoUnitState.Normal;
			}
		}
	}
}
[System.Serializable]
public struct VideoInfo
{
	public Sprite Post;
	public string name;
}
[System.Serializable]
public struct VideoUnitInitAnimation
{
	public float delay;
	public float duration;
}

public enum VideoInfoUnitState
{
	None,
	Init,
	Normal,
	Hovered,
}
