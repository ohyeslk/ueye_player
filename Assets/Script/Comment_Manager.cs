using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class Comment_Manager : MonoBehaviour
{
    public struct CommentStructure
    {
        public CommentStructure(int size, string content, string userName)
        {
            this._fontSize = size;
            this._content = content;
            this._userName = userName;
        }
        public CommentStructure(int size, string content)
        {
            this._fontSize = size;
            this._content = content;
            this._userName = "";
        }
        public CommentStructure(string content)
        {
            this._fontSize = 67;
            this._content = content;
            this._userName = "";
        }
	
        public int _fontSize;
        public string _content;
        public string _userName;
    }


    public GameObject prefab;
    public GameObject newCommentPosition;
    public int limit = 7;
    CustomList<GameObject> Comments;
	List<CommentLine> commentList = new List<CommentLine>();

    // Use this for initialization
    void Start()
    {

    }
    void OnEnable()
    {
        Comments = new CustomList<GameObject>(limit);

        CustomList<GameObject>.OnEnqueue += EnqueueListener;
        CustomList<GameObject>.OnFull += FullListener;
        VREvents.ShowChatMessage += addComment; 
		VREvents.ActiveWindow += OnActiveWindow;
    }

    void OnActiveWindow (WindowArg arg)
    {
		if (arg.type != WindowArg.Type.PLAY_WINDOW) {
			OnBecomeInvisible ();
		}
    }

	void OnBecomeInvisible()
	{
		for (int i = commentList.Count - 1; i >= 0; --i) {
			commentList [i].OnBecomeInvisible ();
			commentList.RemoveAt (i);
		}
	}

	void OnDisable()
	{
		VREvents.ShowChatMessage -= addComment;
		VREvents.ActiveWindow -= OnActiveWindow;
	}

    // Update is called once per frame
    void Update()
    {
        //test comment
        if (Input.GetKeyDown(KeyCode.A))
        {

            GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
            temp.GetComponent<Text>().text = Random.Range(10000.0f, 20000000.0f).ToString();
            Comments.Add(temp);
        }
    }

    public void addComment(ChatArg chatMessage)
    {
		addCommentDirection( chatMessage );
    }

    //add comment with commentstructure
    public void addComment(CommentStructure CS)
    {
		addComment( CS._userName + ": " + CS._content ,  CS._fontSize );
    }

    //add comment with cotent
    public void addComment(string content)
    {
		addComment( content , 85 );
    }
    public void addComment(string content, int fontSize)
    {
        GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Text>().text = content;
        temp.GetComponent<Text>().fontSize = fontSize;
        Comments.Add(temp);
    }

	public void addCommentDirection( ChatArg msg )
	{
		addCommentDirection( msg.userName + ": " + msg.message , 85 , msg.cameraForward );
	}


	public void addCommentDirection( string content, int fontSize , Vector3 direction )
	{
		float distance = ( newCommentPosition.transform.position - Camera.main.transform.position ).magnitude;
		Vector3 newPosition = direction.normalized * distance + Camera.main.transform.position + Vector3.up * Random.Range( 0.4f , 0.7f );

		GameObject temp = GameObject.Instantiate(prefab) as GameObject;
		temp.GetComponent<Text>().text = content;
		temp.GetComponent<Text>().fontSize = fontSize;
		temp.transform.SetParent( newCommentPosition.transform );
		temp.transform.localScale = Vector3.one;
		temp.transform.position = newPosition;
		Vector3 lookDirection = direction;
		lookDirection.y = 0;
		temp.transform.localRotation = Quaternion.LookRotation(lookDirection);

		Text text = temp.GetComponent<Text>();
		if ( text != null )
		{
			text.DOFade( 0 , 4f ).SetDelay ( 8f );
			text.transform.DOMoveY ( 3.5f , 12f ).SetRelative( true ).SetEase( Ease.InCubic );
		}

		CommentLine commentLine = temp.GetComponent<CommentLine> ();

		commentList.Add (commentLine);

	}


    void EnqueueListener()
    {
        if (Comments.Count != 1)
		{
            moveUpForAll();
        }
        setNewComment(Comments[Comments.Count - 1]);
    }

    void FullListener()
    {
        GameObject temp = Comments[0];
        Comments.RemoveAt(0);
        GameObject.Destroy(temp);
    }

    void moveUpForAll()
    {
        for (int i = 0; i < Comments.Count - 1; i++)
        {
            Comments[i].GetComponent<CommentLine>().moveUp();

        }

    }

    //when insert new comment, set it in the bottom
    void setNewComment(GameObject newComment)
    {
        newComment.transform.SetParent(this.transform, false);
        newComment.transform.position = newCommentPosition.transform.position;
        newComment.transform.localScale = new Vector3(1, 1, 1);
    }


}


class CustomList<GameObject> : List<GameObject>
{

    int limit = -1;
    public delegate void EnqueueAction();
    public static event EnqueueAction OnEnqueue;

    public delegate void OnFullAction();
    public static event OnFullAction OnFull;
    public CustomList(int limit)
        : base(limit)
    {
        this.limit = limit;
    }
    public new void Add(GameObject obj)
    {
        if (this.Count >= this.limit)
        {
            OnFull();
        }

        base.Add(obj);
        if (OnEnqueue != null)
        {
            OnEnqueue();
        }
    }
}


