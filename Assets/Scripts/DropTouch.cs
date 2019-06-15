using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropTouch : MonoBehaviour {
	/*
	// ドロップのGameObjectの取得
	public GameObject DropObj;
	// シーンのCameraObjectを取得
	//public Camera camera;
	// ドロップの左上と右下のローカル座標取得
	public GameObject leftUp;
	public GameObject rightDown;

	private GameObject firstDrop;
	private GameObject beforDrop;
	private GameObject selectDrop;

	private InputManager inputManager;

	private Vector3 leftUpPos = new Vector3();
	private Vector3 rightDownPos = new Vector3();
	private int count = 0;

	void Start()
	{
		selectDrop = DropObj;
		inputManager = FindObjectOfType<InputManager> ();
	}

	void Update()
	{
		#if UNITY_IOS
		if (inputManager.TouchTrigger) 
		{
			if (inputManager.removeObject.Count < 5) 
			{
				DropToucRange();

				if (firstDrop == null) 
				{
				// タッチ座標がタッチ判定内にあるかどうか
				if (inputManager.TouchPos.x < rightDownPos.x && inputManager.TouchPos.x > leftUpPos.x &&
					inputManager.TouchPos.y < leftUpPos.y && inputManager.TouchPos.y > rightDownPos.y)
					{
						// todo:タッチされたドロップの様子を変える
						firstDrop = selectDrop;
						beforDrop = selectDrop;
						inputManager.removeObject.Add (DropObj);
					}
				} 
				else if (firstDrop) 
				{
					float distance = Vector2.Distance (inputManager.TouchPos, beforDrop.transform.position);
					if (distance < 10.0f) {
						if (inputManager.TouchPos.x < rightDownPos.x && inputManager.TouchPos.x > leftUpPos.x &&
							inputManager.TouchPos.y < leftUpPos.y && inputManager.TouchPos.y > rightDownPos.y) 
						{
							// todo:タッチされたドロップの様子を変える
							beforDrop = DropObj;
							inputManager.removeObject.Add (DropObj);
						}
					}

				}
			}
		}
		#endif

	// ドロップのタッチ判定を設定する
	void DropToucRange()
	{
		DropLeftUpPos ();
		DropRightDownPos ();
	}

	// ドロップのタッチ判定の左上のワールド座標を生成
	void DropLeftUpPos()
	{
		leftUpPos = leftUp.transform.position;
	}

	// ドロップのタッチ判定の右下のワールド座標を生成
	void DropRightDownPos()
	{
		rightDownPos = rightDown.transform.position;
	}

	void inDrop ()
	{
		
	}
		*/

}