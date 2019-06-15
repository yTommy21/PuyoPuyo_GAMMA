using UnityEngine;
using System.Collections;

public class MetaInputManager : MonoBehaviour 
{
	// 選択されたオブジェクトを格納する
	public GameObject[] destroyList = new GameObject[5];

	// 選択された数
	private int destroyCount = 0;

	public bool isMouseDebug = true;

	[Header("Y座標誤差修正用")]
	public float concessionPosY;
	[Header("Z座標誤差修正用")]
	public float concessionPosZ;

	private GameManager gameManager; 

	void Awake() {
		gameManager = GetComponent<GameManager > ();
	}

	private void Update()
	{
		if (gameManager.isGetInputTrigger) {
			MainManager ();
		}

		// 選択されているオブジェクトを点滅させる
		foreach(GameObject obj in destroyList)
		{
			if(obj == null) { continue; }
			obj.GetComponent<PuzzleDrop>().isInputSelectting = true;
		}
	}

	private bool MainManager()
	{
		if (isMouseDebug)
		{
			if ((!Input.anyKey && destroyCount != 0))
			{
				DestroyDrop();
			}
		}
		else
		{
			if ((Input.touchCount == 0 && destroyCount != 0))
			{
				DestroyDrop();
			}
		}

		if (isMouseDebug)
		{
			if(!Input.anyKey) { return false; }
		}
		else
		{
			if (Input.touchCount != 1) { return false; }
		}

		if (destroyCount >= 5) { return false; }

		Vector3 targetPos = Vector3.zero;
		Ray ray;

		if (Input.touchCount >= 1)
		{
			// タッチした座標をスクリーン座標に変換して保管する
			targetPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

		}
		if (Input.GetKey(KeyCode.Mouse0))
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.up);
		}

		RaycastHit2D hit = Physics2D.Raycast(targetPos, -Vector2.up);
		//Debug.DrawRay(Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position), -Vector2.up);

		// Rayがcolliderに当たっているかどうか。
		if (hit.collider != null)
		{
			// 当たっているならそのcolliderがattachされているgameObjectをhitDropに格納する。
			GameObject hitDrop = hit.collider.gameObject;

			// クリックされたのがメイン出なかった場合処理を終了する
			if (!hitDrop.GetComponent<PuzzleDrop>().isMain) { return false; }

			// すでに同じドロップがあれば処理を終了する
			foreach (GameObject obj in destroyList)
			{
				if (obj == hitDrop)
				{
					return false;
				}
			}

			// 前回選択されたドロップの周りに存在しているドロップでなければ処理を終了する
			if (destroyCount != 0)
			{
				if (!CanSelectDrop(hitDrop, destroyList[destroyCount - 1]))
				{
					return false;
				}
			}

			destroyList[destroyCount] = hitDrop;

			destroyCount++;
		}

		return true;
	}

	// 周りにあるドロップのみを選択していればtrueを返す
	private bool CanSelectDrop(GameObject selectObj, GameObject beforeObj)
	{
		int currentId = selectObj.GetComponent<PuzzleDrop>().dropID;
		int beforeId = beforeObj.GetComponent<PuzzleDrop>().dropID;
		//		Debug.Log ("currentId: " + currentId);
		//		Debug.Log ("beforeId: " + beforeId);

		int width = 8;

		// todo: 左端右端バグ
		// BeforeIDが左端だった場合のタッチ判定
		if (beforeId == 0 || beforeId == 8 || beforeId == 16 ||
			beforeId == 24 || beforeId == 32 || beforeId == 40) 
		{
			// 左端だった場合逆の右端のひとつ下にずれた３つの判定にできるタッチ判定を消す
			if ((beforeId + width) <= currentId && currentId <= (beforeId + width + 1) ||
				beforeId <= currentId && currentId <= (beforeId + 1) ||
				(beforeId - width) <= currentId && currentId <= (beforeId - width + 1)) {
				return true;
			}
		} 
		// BeforIDが右端だった場合のタッチ判定
		else if (beforeId == 7 || beforeId == 15 || beforeId == 23 ||
			beforeId == 31 || beforeId == 39 || beforeId == 47) 
		{
			// 右端だった場合逆の左端のひとつ上にずれた３つの判定にできるタッチ判定を消す
			if ((beforeId + width - 1) <= currentId && currentId <= (beforeId + width) ||
				beforeId - 1 <= currentId && currentId <= beforeId ||
				beforeId - width - 1 <= currentId && currentId <= beforeId - width)
			{
				return true;
			}
		}
		// 左端、右端以外の真ん中がBeforeIDの場合のタッチ判定
		else if ((beforeId + width - 1) <= currentId && currentId <= (beforeId + width + 1) ||
			beforeId - 1 <= currentId && currentId <= beforeId + 1 ||
			beforeId - width - 1 <= currentId && currentId <= beforeId - width + 1)
		{
			return true;
		}
		return false;
	}

	// 選択されたドロップをまとめて削除する
	private void DestroyDrop()
	{
		foreach (GameObject drop in destroyList)
		{
			if (drop != null)
			{
				drop.SendMessage("OnDropClick", SendMessageOptions.DontRequireReceiver);
			}
		}

		destroyCount = 0;
	}
}

