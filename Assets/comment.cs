using UnityEngine;
using System.Collections;
using DG.Tweening;

public class comment : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void moveUp()
    {
        this.transform.DOLocalMoveY(70, 0.25f).SetRelative(); ;
    }
}
