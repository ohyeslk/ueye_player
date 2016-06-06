using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;
using System.Text;

internal class WebReqState
{
	public byte[] Buffer;

	public FileStream fs;

	public const int BufferSize = 1024;

	public Stream OrginalStream;

	public HttpWebResponse WebResponse;

	public WebReqState(string path)
	{
		Buffer = new byte[1024];
		fs = new FileStream(path,FileMode.Create);
	}

}

public class HttpHelper {
	string m_url;
	bool m_Done = false;
	public static string TemperarySavePath = "";
	public bool Done{
		get { return m_Done;}
	}

	static public string GetLocalFilePath( string url )
	{
		return GetTemperarySavePath() + '/' + GetNameFromURL(url);
	}

	static public string GetTemperarySavePath()
	{
		if ( TemperarySavePath == "" )
		{
			TemperarySavePath = Application.persistentDataPath + "/TemData";
//			if ( Application.platform == RuntimePlatform.OSXEditor )
//			{
//				TemperarySavePath = Application.persistentDataPath + "/TemData";
//			}else if ( Application.platform == RuntimePlatform.IPhonePlayer )
//			{
//				TemperarySavePath = Application.persistentDataPath + "/TemData";
//			}else if ( Application.platform == RuntimePlatform.Android )
//			{
//				TemperarySavePath = Application.persistentDataPath + "/TemData";
//			}
		}
		return TemperarySavePath;
	}

	public static string GetNameFromURL( string url )
	{
		//		id ++;
		//		string[] names = url.Split('/');
		//		string name = names[names.Length-1];
		//		string res = name.Substring(0,name.Length-4) + id.ToString() + name.Substring(name.Length-4,4);
		//
		//		return res;
		return GetInt64HashFromURL( url ).ToString() + url.Substring( url.Length - 4 , 4 );
	}
		
	static public Int64 GetInt64HashFromURL( string url )
	{
//		Int64 hashCode = 0;
//		if (!string.IsNullOrEmpty(url))
//		{
//			//Unicode Encode Covering all characterset
//			byte[] byteContents = Encoding.Unicode.GetBytes(url);
//			System.Security.Cryptography.SHA256 hash = 
//				new System.Security.Cryptography.SHA256CryptoServiceProvider();
//			byte[] hashText = hash.ComputeHash(byteContents);
//			//32Byte hashText separate
//			//hashCodeStart = 0~7  8Byte
//			//hashCodeMedium = 8~23  8Byte
//			//hashCodeEnd = 24~31  8Byte
//			//and Fold
//			Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
//			Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
//			Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 24);
//			hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
//		}
//		return (hashCode);

		var s1 = url.Substring(0, url.Length / 2);
		var s2 = url.Substring( url.Length / 2);

		var x= ((long)s1.GetHashCode()) << 0x20 | s2.GetHashCode();

		return x;

	}

	/// <summary>
	/// initilize the temparary save directory
	/// need to be called in awake or start because the Application.persistentDataPath 
	/// can only be called on main thread
	/// </summary>
	public static void Init()
	{
		if ( ! Directory.Exists( GetTemperarySavePath() ) )
			Directory.CreateDirectory( GetTemperarySavePath());
	}

	public HttpHelper( string url )
	{
//		this.path = path;
		m_url = url;
	}


	public void AsyDownLoad()
	{
//		Debug.Log("Start Down Load " + url );
		HttpWebRequest httpRequest = WebRequest.Create(m_url) as HttpWebRequest;
		httpRequest.BeginGetResponse( new AsyncCallback(ResponseCallback) , httpRequest );
	}

	public string GetLocalFilePath()
	{
		return GetLocalFilePath( m_url );
	}

	void ResponseCallback(IAsyncResult ar)
	{
		HttpWebRequest req = ar.AsyncState as HttpWebRequest;
		if ( req == null )
		{
			Debug.Log("Request equal to null" ) ;
			return;
		}
		HttpWebResponse response = req.EndGetResponse( ar ) as HttpWebResponse;
		if ( response.StatusCode != HttpStatusCode.OK )
		{
			Debug.Log("Response not OK");
			response.Close();
			return;
		}

		WebReqState st = new WebReqState( GetLocalFilePath() );
		st.WebResponse = response;
		Stream responseStream = response.GetResponseStream();
		st.OrginalStream = responseStream;
		responseStream.BeginRead(st.Buffer,0,WebReqState.BufferSize,new AsyncCallback(ReadDataCallback),st);
	}

	void ReadDataCallback(IAsyncResult ar)
	{
		WebReqState rs = ar.AsyncState as WebReqState;
		int read =rs.OrginalStream.EndRead(ar);

		if(read>0)
		{
			rs.fs.Write(rs.Buffer,0,read);
			rs.fs.Flush();
			rs.OrginalStream.BeginRead(rs.Buffer, 0, WebReqState.BufferSize, new AsyncCallback(ReadDataCallback), rs);
		}
		else
		{
			rs.fs.Close();
			rs.OrginalStream.Close();
			rs.WebResponse.Close();
//			Debug.Log(assetName+":::: success");
			m_Done = true;
		}
	}
}