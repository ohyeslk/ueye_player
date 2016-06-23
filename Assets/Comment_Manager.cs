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

    // Use this for initialization
    void Start()
    {

    }
    void OnEnable()
    {
        Comments = new CustomList<GameObject>(limit);

        CustomList<GameObject>.OnEnqueue += EnqueueListener;
        CustomList<GameObject>.OnFull += FullListener;
        VREvents.ChatMessage += addComment; 

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
        GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Text>().text = chatMessage.userName + ": " + chatMessage.message;
        temp.GetComponent<Text>().fontSize = 67;
       // temp.GetComponent<Text>().fontSize = ;
        Comments.Add(temp);
    }

    //add comment with commentstructure
    public void addComment(CommentStructure CS)
    {
        GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Text>().text = CS._userName + ": " + CS._content;
        temp.GetComponent<Text>().fontSize = CS._fontSize;
        Comments.Add(temp);
    }

    //add comment with cotent
    public void addComment(string content)
    {
        GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Text>().text = content;
        temp.GetComponent<Text>().fontSize = 67;
        Comments.Add(temp);
    }
    public void addComment(string content, int fontSize)
    {
        GameObject temp = GameObject.Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        temp.GetComponent<Text>().text = content;
        temp.GetComponent<Text>().fontSize = fontSize;
        Comments.Add(temp);
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
            Comments[i].GetComponent<comment>().moveUp();

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


