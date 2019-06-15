using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	public int Health;
	public int Attack;
	public bool DeathCheck;

	public enum ELEMENT
	{
		Fire,
		Water,
		Wind,
		Darkness,
		Light,
		NULL,

		NUM
	}
	
    // キャラクターの属性	
	public ELEMENT element = ELEMENT.Fire;
}
