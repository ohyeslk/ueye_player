using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class CommentLine : MonoBehaviour
{
	public Text text;
	[SerializeField]float fadeOutTime;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

	public void OnBecomeInvisible()
	{
		text.DOKill ();
		text.DOFade (0, fadeOutTime);
	}

    public void moveUp()
    {
        this.transform.DOLocalMoveY(70, 0.25f).SetRelative(); ;
    }
}
