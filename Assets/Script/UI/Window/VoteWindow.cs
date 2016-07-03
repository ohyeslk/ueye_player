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

		int totalVote = 0;
		for( int i = 0 ; i < msg.options.Length ; ++ i ) totalVote += msg.options[i].number;

		int voteNumber = msg.options.Length;


		for( int i = 0 ; i < voteNumber ; ++ i )
		{
			GameObject voteButton = Instantiate( voteButtonPrefab ) as GameObject;
			voteButton.transform.SetParent( voteButtonPanel );

			VoteButton vbCom = voteButton.GetComponent<VoteButton>();
			vbCom.Init( msg.options[i] , totalVote ,  i );
			voteButtons.Add( vbCom );
		}
	}

	
}
