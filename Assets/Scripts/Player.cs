using UnityEngine;
using System.Collections;

public class Player : Character
{
	// キャラクターの回復力
	public int Recovery;
	// キャラクターの１回消した分のダメージの数値
	public int DamageGiven;
	// 連鎖含む全てのダメージ数値
	public int AllDamageGiven;

	private int nowHealth; 
	 
	// 動物のHPを表示する
	public UILabel HPlabel;
	public UISlider slider;
	
	public bool attackTriggaer = false;

	private GameManager gameManager;

	void Awake()
	{
		gameManager = FindObjectOfType<GameManager>();
	}

	void Start ()
	{
		HPlabel.text = Health.ToString();
		nowHealth = Health;
	}

	void Update ()
	{
		slider.value = nowHealth / Health;
	}

	
	// 自分の属性のドロップが消えていれば攻撃トリガーをオンにする
	void AttackPowerGet(Player.ELEMENT element) {
		switch(element)
		{
			case ELEMENT.Fire: attackTriggaer = true;
			break;
			case ELEMENT.Water: attackTriggaer = true;
			break;
			case ELEMENT.Wind: attackTriggaer = true;
			break;
			case ELEMENT.Darkness: attackTriggaer = true;
			break;
			case ELEMENT.Light: attackTriggaer = true;
			break;
		}
	}
	
	// 攻撃するかどうかを確認し、ダメージを計算する。
	public void CheckAndAttack()
	{
		AttackPowerGet(element);
		// 自分の属性のドロップが消えたかどうか
		if (attackTriggaer)
		{
			// 消えてた場合、ダメージの計算をする
		}
	}
}
