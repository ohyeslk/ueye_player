using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoteButton : MonoBehaviour {

	VoteOption option;

	[SerializeField] Text detail;
	[SerializeField] Image numberBar;


	public void OnEnable()
	{
		VREvents.VoteUpdate += OnVoteUpdate;
	}

	public void OnDisable()
	{
		VREvents.VoteUpdate -= OnVoteUpdate;
	}

	public void Init( VoteArg msg , int index )
	{
		option = msg.options[index];
		detail.text = option.detail;
		numberBar.fillAmount = 1.0f * msg.options[index].number / ( msg.TotalVote + 10 );
	}

	void OnVoteUpdate (VoteArg msg)
	{
		foreach( VoteOption opt in msg.options )
		{
			if ( opt.detail == option.detail )
			{
				UpdateUI( opt , msg.TotalVote );

			}
		}
	}

	public void UpdateUI( VoteOption option , long totalNumber )
	{
		numberBar.fillAmount = 1.0f * option.number / ( totalNumber + 10 );
	}

	public void OnVote()
	{
		Message msg = new Message(this);
		msg.AddMessage( Global.MSG_USER_VOTE_OPTION , option );
		VREvents.FireUserVote( msg );
	}
}
