using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class DetailWindow : UIWindow {

	[SerializeField] Image img;
	[SerializeField] Text text;

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
	}

	override protected void OnEnable()
	{
		base.OnEnable();
		VREvents.ShowDetail += OnShowDetail;
	}


	void OnShowDetail( Message msg )
	{
		img.enabled = true;
		text.enabled = true;

		PlayInitAnimation();
	}

	void PlayInitAnimation()
	{
		img.DOFade( 1f , m_setting.showDuration );
		text.text = "";
		text.DOFade( 1f , m_setting.showDuration );
		text.DOText( "aaa " , m_setting.showDuration );
	}

	void PlayExitAnimation()
	{
		img.DOFade( 0f , m_setting.hideDuration );
		text.DOFade( 0f , m_setting.hideDuration );
	}

	void ExitComplete()
	{
		img.enabled = false;
		text.enabled = false;
	}
}
