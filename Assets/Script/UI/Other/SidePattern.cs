using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class SidePattern : MonoBehaviour {
	[SerializeField] Image m_Image;
	[SerializeField] Sprite enableSprite;
	[SerializeField] Sprite disableSprite;

	void Awake()
	{
		if ( m_Image == null )
			m_Image = GetComponent<Image>();
	}

	bool m_enable = false;
	public bool PatternEnable
	{
		get {
			return m_enable;
		}
		set {
			m_enable = value;
			if ( m_enable )
			{
				m_Image.sprite = enableSprite;
			}else
			{
				m_Image.sprite = disableSprite;
			}
		}
	}

	public void OnBecomeVisible( float time )
	{
		if ( m_Image != null ) m_Image.DOFade( 1f , time );
	}
	public void OnBecomeInvisible( float time )
	{
		if ( m_Image != null ) m_Image.DOFade( 0 , time );
	}
}
