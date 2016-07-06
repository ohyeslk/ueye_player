using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DepthPanelWindow : MonoBehaviour {

	[SerializeField] MediaPlayerCtrl videoPlayer;

//	const string m_url = "http://d2fn2cfzrmdwta.cloudfront.net/liveout/s/playlist.m3u8";
	public static string m_url = "http://balalalivecf-38f2.kxcdn.com/liveout/s/playlist.m3u8";
//	public static string m_url = "http://sc-hls-live.speedycloud.cn/live/fac20fa7b255c91125401009c4409301.m3u8";

	void Awake()
	{
		OnRefresh();
	}

	public void OnBack()
	{
		SceneManager.LoadScene("VideoSelect");
	}

	public void OnRefresh()
	{
		Debug.Log("Load " + m_url );
		videoPlayer.Load(m_url);
		videoPlayer.Play();
	}
}
