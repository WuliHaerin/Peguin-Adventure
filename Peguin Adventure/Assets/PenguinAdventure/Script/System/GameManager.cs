using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public GameObject AdPanel;

	[Header("Setup Level")]
	[Tooltip("Is this final level of the World? the next World will be unlock")]
	public bool isFinishWorld;

	public int bullets = 10;		//bullets for player fire
	public int star1 = 100;			//score needed to reach 1 star
	public int star2 = 500;			//score needed to reach 2 star
	public int star3 = 1000;		//score needed to reach 3 star
	[Header("")]
	public AudioClip soundSuccess;	//sound played when level complete
	public AudioClip soundFail;		//sound played when fail

	public static GameManager instance;		//global call

	public enum GameState
	{
		Menu, Playing, Pause
	}
	[HideInInspector]
	public GameState state;

	private int score = 0;
	private int stars = 0;
	private int heart = 5;

	//Get Score from another script ex: int score = GameManager.Score;
	public static int Score{
		get{ return instance.score; }
		set{ instance.score = value; }
	}

	//Get Stars from another script
	public static int Stars{
		get{ return instance.stars; }
		set{ instance.stars = value; }
	}

	//Get Hearts from another script
	public static int Hearts{
		get{ return instance.heart; }
		set{ instance.heart = Mathf.Clamp(value,0,7); }
	}

	//Get Bullets from another script
	public static int Bullets{
		get{ return instance.bullets; }
		set{ instance.bullets = value; }
	}

	//Get Best of this level from another script
	public static int Best {
		get{ return PlayerPrefs.GetInt ("World" + GlobalValue.worldPlaying + GlobalValue.levelPlaying+"best", 0); }
		set{ PlayerPrefs.SetInt ("World" + GlobalValue.worldPlaying + GlobalValue.levelPlaying+"best", value); }
	}

	//Get BestStars of this level from another script
	public static int BestStars {
		get{ return PlayerPrefs.GetInt ("World" + GlobalValue.worldPlaying + GlobalValue.levelPlaying+"stars", 0); }
		set{ PlayerPrefs.SetInt ("World" + GlobalValue.worldPlaying + GlobalValue.levelPlaying+"stars", value); }
	}

	//Get Highest Level of this level from another script
	public static int HighestLevel {
		get{ return PlayerPrefs.GetInt ("World" + GlobalValue.worldPlaying + "HighestLevel", 1); }
		set{ PlayerPrefs.SetInt ("World" + GlobalValue.worldPlaying + "HighestLevel", value); }
	}

	//Get current state from another script
	public static GameState CurrentState{
		get{ return instance.state; }
		set{ instance.state = value; }
	}
		
	private PlayerController player;


	public void SetAdPanel(bool a)
    {
		AdPanel.SetActive(a);
		//Time.timeScale = a == true ? 0 : 1;
    }

	void Awake(){
		instance = this;
	}

	// Use this for initialization
	void Start () {
		state = GameState.Menu;
		player = FindObjectOfType<PlayerController> ();
//		AdsController.HideAds();
	}

	// Update is called once per frame
	void Update () {
		
		if (Input.anyKeyDown && state != GameState.Playing)		//Start playing when hit anykey or any touch
			Play ();
	}

	//play game
	public void Play(){
		state = GameState.Playing;		//set state to playing
		player.Play ();		//call funstion Play() in player object
	}

	//called by another script by: GameManager.instance.GameSucess();
	public void GameSuccess(){
//		AdsController.ShowAds();		//show admob

//		if (ApplovinController.Instance != null)
//			ApplovinController.Instance.ShowAds ();		//show applovin

		state = GameState.Menu;		//set state to Menu to stop player, enemy, monster,...because they only work when state = playing
		//check level pass
		if (GlobalValue.levelPlaying >= HighestLevel)
			HighestLevel = GlobalValue.levelPlaying + 1;	//if this level >= highest level then save next level number

		//check and save best score
		if (score > Best) {
			Best = score;
		
		//save best star of this level
			if (score >= star3 && BestStars < 3)
				BestStars = 3;
			else if (score >= star2 && BestStars < 2)
				BestStars = 2;
			else if (score >= star1 && BestStars < 1)
				BestStars = 1;
		}
		MenuManager.instance.ShowLevelComplete ();		//Tell UI object show level complete panel
		SoundManager.PlaySfx (soundSuccess);		//play success sound

		//Unlock New World
		if (isFinishWorld) {
			int WorldReached = PlayerPrefs.GetInt ("WorldReached", 1);
			if (WorldReached == GlobalValue.worldPlaying)
				PlayerPrefs.SetInt ("WorldReached", GlobalValue.worldPlaying + 1);
		}


		AdManager.ShowInterstitialAd("1lcaf5895d5l1293dc",
			() => {
				Debug.LogError("--插屏广告完成--");

			},
			(it, str) => {
				Debug.LogError("Error->" + str);
			});
	}


	public void GameOver()
    {
		SoundManager.PlaySfx(soundFail, 0.5f);      //play fail sound
		player.Dead();      //Tell player operate Dead() function
		state = GameState.Menu;
		StartCoroutine(SetAdTrue());
    }

	IEnumerator SetAdTrue()
    {
		yield return new WaitForSeconds(2);
		SetAdPanel(true);

	}

	public void SetAdFalse()
    {
		StopCoroutine(SetAdTrue());
	}

	//called by another script by: GameManager.instance.GameOver();
	public void Revive(){


		if (state == GameState.Playing) {   //only work when in playing mode, to prevent duplicate calling

			AdManager.ShowVideoAd("192if3b93qo6991ed0",
			(bol) => {
				if (bol)
				{
					SetAdFalse();
					SetAdPanel(false);					
					StartCoroutine(Restartcheckpoint(0.5f));

					//			AdsController.ShowAds();		//show admob

					//		if (ApplovinController.Instance != null)
					//			ApplovinController.Instance.ShowAds ();		//show applovin

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
	}

	IEnumerator Restartcheckpoint(float time){

		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}
