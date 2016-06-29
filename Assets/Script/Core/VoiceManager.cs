using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;

/// <summary>
/// Manage the voice convertion system
/// </summary>
public class VoiceManager : MonoBehaviour {

	static string m_token;
	public static string TOKEN
	{
		get {
			return m_token;
		}
	}

	static bool m_isRecording;
	public static bool IsRecording
	{
		get {
			return m_isRecording;
		}
	}

	void Awake()
	{
		if ( m_audio == null )
		{
			m_audio = GetComponent<AudioSource>();
			if ( m_audio == null )
			{
				m_audio = gameObject.AddComponent<AudioSource>();
			}
		}
	}

	void Start()
	{
		VREvents.FireRequestBaiduYuyinToken(new URLRequestMessage(this));
	}

	void OnEnable()
	{
		VREvents.PostBaiduYuyinToken += onBaiduYuyinToken;
		VREvents.VoiceRecord += onRecordVoice;
		VREvents.PostBaiduYuyinTranslate += OnPostBaiduYuyinTranslate;
	}


	void OnDisable()
	{
		VREvents.PostBaiduYuyinToken -= onBaiduYuyinToken;
		VREvents.VoiceRecord -= onRecordVoice;
		VREvents.PostBaiduYuyinTranslate -= OnPostBaiduYuyinTranslate;
	}
		
	void OnPostBaiduYuyinTranslate (URLRequestMessage msg)
	{
		string res = msg.GetMessage(Global.MSG_BAIDU_YYIN_TRANSLATE_RESULT).ToString();

		JSONObject resJSON = new JSONObject(res );
		Debug.Log("Json " + resJSON.ToString());

		string sentence = "请问你在说什么？";

		if ( resJSON.GetField("result") != null )
		{
			sentence = resJSON.GetField("result").ToString();
			sentence = sentence.Substring(2,sentence.Length-4);
		}

		Message msgSend = new Message(this);
		msgSend.AddMessage("data" , sentence);
		VREvents.FirePostChatMessage(msgSend);
	}

	void onBaiduYuyinToken (URLRequestMessage msg)
	{
		m_token = msg.GetMessage(Global.MSG_BAIDU_YUYIN_TOKEN).ToString();
		Debug.Log("=== Got token === ");
	}

	void onRecordVoice(Message msg )
	{
		bool isOn = (bool)msg.GetMessage("isOn");
		Debug.Log("On Record " + isOn);

		if ( isOn )
		{
			BeginRecord( msg );
		}else
		{
			EndRecord( msg );
		}
	}

	void BeginRecord( Message msg )
	{
		if ( !IsRecording )
		{
			m_isRecording = true;
			m_audio.clip = Microphone.Start(Microphone.devices[0], true , 600 , 8000);
			m_audio.loop = true;
			m_audio.mute = true;
			while (!(Microphone.GetPosition(audioDeviceName) > 0)){};
			m_audio.Play();

			audioDeviceName = Microphone.devices[0];
			m_isRecording = true;
		}
	}

	void EndRecord( Message msg )
	{
		if ( IsRecording )
		{
			m_isRecording = false;

			int pos = Microphone.GetPosition(audioDeviceName);

			Microphone.End(Microphone.devices[0]);

			if ( pos > 1 )
			{
				float[] talk = new float[pos - 1];
				m_audio.clip.GetData(talk, recStart);
				play(talk);
			}
		}
	}

	[SerializeField] AudioSource m_audio;
	private string audioDeviceName;
	private int recStart;


	public class postObj
	{
		public string format { get; set; }
		public int rate { get; set; }
		public int channel { get; set; }
		public string token { get; set; }
		public string lan { get; set; }
		public string cuid { get; set; }
		public int len { get; set; }
		public string speech { get; set; }
	}

	public void play(float[] audios)
	{
		Int16[] intData = new Int16[audios.Length];

		Byte[] bytesData = new Byte[audios.Length * 2];
		float sumf = 0;

		int rescaleFactor = 32767; 

		for (int i = 0; i < audios.Length; i++)
		{
			intData[i] = (short)(audios[i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
			sumf += Math.Abs(audios[i]);
		}
		sumf /= audios.Length;

		Stream fileStream = CreateEmpty();
		fileStream.Write(bytesData, 0, bytesData.Length);
		WriteHeader(fileStream, m_audio.clip);
		byte[] lastbyte = new byte[fileStream.Length];
		fileStream.Read(lastbyte, 0, lastbyte.Length);

		string base64str = System.Convert.ToBase64String(lastbyte);

		request("http://vop.baidu.com/server_api", base64str, lastbyte.Length);

	}

	public void request(string url, string base64audio, int length)
	{
		postObj jsonObj = new postObj()
		{
			format = "wav",
			rate = 8000,
			channel = 1,
			// TODO : adjust the language option
			// lan= "en",
			token = TOKEN,
			cuid = "77777778888883",
			len=length,
			speech = base64audio
		};
		string strJson= JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

		URLRequestMessage msg = new URLRequestMessage(this);
		msg.AddMessage(Global.MSG_BAIDU_YYIN_TRANSLATE_JSON,strJson );
		msg.url = url;

		VREvents.FireRequestBaiduYuyinTranslate( msg );

	}

	private static void WriteHeader(Stream stream, AudioClip clip)
	{
		int hz = clip.frequency;
		int channels = clip.channels;
		int samples = clip.samples;

		stream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		stream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
		stream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		stream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		stream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		stream.Write(subChunk1, 0, 4);

		UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		stream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channels);
		stream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(hz);
		stream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
		stream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort)(channels * 2);
		stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		stream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		stream.Write(datastring, 0, 4);

		Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
		stream.Write(subChunk2, 0, 4);
	}

	private static Stream CreateEmpty()
	{
		Stream fileStream = new MemoryStream();
		byte emptyByte = new byte();

		for (int i = 0; i < 44; i++)
		{
			fileStream.WriteByte(emptyByte);
		}

		return fileStream;
	}

}
