using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VoteWindow : MonoBehaviour {

	[SerializeField] GameObject voteButtonPrefab;
	[SerializeField] Transform voteButtonPanel;
	[SerializeField] Text VoteTitle;
	List<VoteButton> voteButtons = new List<VoteButton>();

	public void Init( VoteArg msg )
	{
		VoteTitle.text = msg.title ;

		int voteNumber = msg.options.Length;

		for( int i = voteButtons.Count ; i < voteNumber ; ++ i )
		{
			GameObject voteButton = Instantiate( voteButtonPrefab ) as GameObject;
			voteButton.transform.SetParent( voteButtonPanel );
			voteButton.transform.localScale = Vector3.one;
			voteButton.transform.localPosition = Vector3.zero;
			voteButton.transform.localRotation = Quaternion.identity;

			VoteButton vbCom = voteButton.GetComponent<VoteButton>();
			voteButtons.Add( vbCom );
		}

		for( int i = 0 ; i < voteNumber ; ++ i )
		{
			voteButtons[i].Init( msg ,  i );
		}
	}
	
}
