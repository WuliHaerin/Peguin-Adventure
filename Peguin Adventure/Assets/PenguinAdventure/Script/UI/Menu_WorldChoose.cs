using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_WorldChoose : MonoBehaviour {
	public GameObject Lock2;
	public GameObject Lock3;
	public Button World2;
	public Button World3;
	public Button unlockBtn1;
	public Button unlockBtn2;
	public GameObject unlockPanel;
	public int curSelectedWorld = 0;
	static bool isUnlock2;
	static bool isUnlock3;

	int WorldReached;
	// Use this for initialization
	void Start () {
		Refresh();
		
	}

	void Refresh()
    {
		WorldReached = PlayerPrefs.GetInt("WorldReached", 1);

		if (WorldReached > 1)
		{
			Lock2.SetActive(false);
			if(unlockBtn1!=null)
            {
				unlockBtn1.gameObject.SetActive(false);
            }
		}
		else
			World2.interactable = false;

		if (WorldReached > 2)
		{
			Lock3.SetActive(false);
			if (unlockBtn2 != null)
			{
				unlockBtn2.gameObject.SetActive(false);
			}
		}
		else
			World3.interactable = false;
	}

	public void UnlockWorld()
    {
		AdManager.ShowVideoAd("192if3b93qo6991ed0",
			(bol) => {
				if (bol)
				{
					if (curSelectedWorld == 2)
					{
						PlayerPrefs.SetInt("WorldReached", 2);
						unlockPanel.SetActive(false);
						isUnlock2 = true;
						Refresh();
						//PlayerPrefs.SetInt("World2" + "HighestLevel", int.MaxValue);
					}
					else if (curSelectedWorld == 3)
					{
						PlayerPrefs.SetInt("WorldReached", 3);
						unlockPanel.SetActive(false);
						isUnlock3 = true;
						Refresh();
						//PlayerPrefs.SetInt("World3" + "HighestLevel", int.MaxValue);
					}
					else
                    {
						Debug.LogError("没有该选项");
                    }

					AdManager.clickid = "";
					AdManager.getClickid();
					AdManager.apiSend("game_addiction", AdManager.clickid);
					AdManager.apiSend("lt_roi", AdManager.clickid);


				}
				else
				{
					StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
				}
			},
			(it, str) => {
				Debug.LogError("Error->" + str);
				//AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
			});
	}

	public void SetUnlockPanel(int value)
    {
		Debug.Log(value);
		if(value==2 && WorldReached > 1)
        {
			return;
        }
		if (value == 3 && WorldReached > 2)
		{
			return;
		}
		unlockPanel.SetActive(true);
		curSelectedWorld = value;
    }


}
