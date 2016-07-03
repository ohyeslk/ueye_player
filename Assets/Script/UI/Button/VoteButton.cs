using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoteButton : MonoBehaviour {

	VoteOption option;

	[SerializeField] Text detail;
	[SerializeField] Image numberBar;

	public void Init( VoteOption _option , int totalVote , int index )
	{
		option = _option;
		detail.text = option.detail;
		numberBar.fillAmount = _option.number / ( totalVote + 10 );
	}

	public void UpdateUI( VoteOption _option )
	{
		
	}

	public void OnVote()
	{
		Message msg = new Message(this);
		VREvents.FireUserVote( msg );
	}
}
