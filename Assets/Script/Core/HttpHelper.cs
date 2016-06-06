using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.IO;

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
	static int id = 0;
	string path = null;
	string assetName = "test";
	bool m_Done = false;
	public bool Done{
		get { return m_Done;}
	}
	public string LocalFilePath {
		get {
			return "file://" + path + "/" + assetName;
		}
	}
	public HttpHelper(string path)
	{
		this.path = path;
		if ( !Directory.Exists( path ) )
		{
			Directory.CreateDirectory( path );
		}
	}

	public string GetNameFromURL( string url )
	{
		id ++;
		string[] names = url.Split('/');
		string name = names[names.Length-1];
		string res = name.Substring(0,name.Length-4) + id.ToString() + name.Substring(name.Length-4,4);

		return res;
	}

	public void AsyDownLoad(string url)
	{
//		Debug.Log("Start Down Load " + url );
		assetName = GetNameFromURL( url );
		HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
		httpRequest.BeginGetResponse( new AsyncCallback(ResponseCallback) , httpRequest );
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

		WebReqState st = new WebReqState(path + "/" + assetName);
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