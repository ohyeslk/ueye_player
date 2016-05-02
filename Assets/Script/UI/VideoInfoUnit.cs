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

	VideoInfoUnitState m_state = VideoInfoUnitState.Normal;
	public VideoInfoUnitState State
	{
		get {
			return m_state;
		}
	}
	public void Init(VideoInfo info , VideoUnitInitAnimation anim )
	{
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