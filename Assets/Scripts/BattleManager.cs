using UnityEngine;

public class BattleManager : MonoBehaviour {

	// キャラクターの回復する数値
	//public int Healing;
	
	
	// それぞれのドロップが消えた個数
	public bool RedCount = false;
	public bool BlueCount = false;
	public bool GreenCount = false;
	public bool PurpleCount = false;
	public bool YellowCount = false;
	public bool CureCount = false;
	
	// ドロップの消えた数の総数
	public int totalCount = 0;
	
	// 総合ダメージ数
	public int totalDamage = 0;

	[HeaderAttribute("プレイヤーオブジェカード")]
	public Player[] players = new Player[6];
	[HeaderAttribute("敵オブジェカード")]
	//public Enemy[] enemys = new Enemy[3];

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager> ();
	}
	
	
	
	// 攻撃開始
	public void AttackStart() 
	{
		AttackPlayerCheck();
		
		totalDamageCalculate();
		
		//PlayerGivenDamegeEnemy();
		
		BattleTriggerOff();
	}
	
	
	// 消されたドロップの種類ごとのカウント
	public void ElementCheck(PuzzleDrop.DROP_TYPE type)
	{
		totalCount++;
		switch(type)
		{
			case PuzzleDrop.DROP_TYPE.RED:
			RedCount = true;
			//RedCount++;
			break;
			case PuzzleDrop.DROP_TYPE.BLUE:
			BlueCount = true;
			//BlueCount++;
			break;
			case PuzzleDrop.DROP_TYPE.GREEN:
			GreenCount = true;
			//GreenCount++;
			break;
			case PuzzleDrop.DROP_TYPE.PURPLE:
			PurpleCount = true;
			break;
			case PuzzleDrop.DROP_TYPE.YELLOW:
			YellowCount = true;
			break;
			//case PuzzleDrop.DROP_TYPE.CURE:
			
			//CureCount++;
			//break;
		}
	}
	
	// どのPlayerが攻撃するのかのチェック
	public void AttackPlayerCheck()
	{
		for (int i = 0; i < 6; i++)
		{
			//players[i].CheckAndAttack();
			players[i].attackTriggaer = false;
		}
	}
	
	// 総ダメージ量
	void totalDamageCalculate()
	{
		for (int i = 0; i < 6; i++)
		{
			totalDamage += players[i].DamageGiven;
		}
	}
	/*
	// EnemyのHealthを減らす
	void PlayerGivenDamegeEnemy()
	{
		if (enemys[0].DeathCheck)
		{
			if (enemys[0].Health > totalDamage)
			{
				enemys[0].Health -= totalDamage;
			}
		}
	}*/
	
/*
	// 回復量の計算
	void CalculateHealing()
	{
		int PTHealingAverage;
		// コンボ数取得
		ComboCount = gameManager.comboCount;


	}
*/

	// 連鎖倍率の計算をして値を返す
	public float ChainRatioCaluculate()
	{
		float chainRatio = 0;
		
		// コンボ数取得
		int comboCount = gameManager.comboCount;
		if (comboCount <= 4)
		{
			// コンボ数を平方根計算
			double RatioSource = Mathf.Sqrt(comboCount);
			chainRatio = (float)RatioSource;

		}
		else if (comboCount >= 5) 
		{
			double ComboBounusRatio = 0.2;
			double RatioSource = Mathf.Sqrt(comboCount);
			chainRatio = (float)RatioSource + (float)ComboBounusRatio;
		}
		
		return chainRatio;
	}
	
	// 全てのトリガーをOFFにする
	public void BattleTriggerOff()
	{
		totalCount = 0;
		totalDamage = 0;
		
		RedCount = false;
		BlueCount = false;
		GreenCount = false;
		PurpleCount = false;
		YellowCount = false;
		
		for (int i = 0; i < 6; i++)
		{
			players[i].attackTriggaer = false;
		}
	}
}
