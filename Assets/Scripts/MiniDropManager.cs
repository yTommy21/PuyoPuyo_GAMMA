using UnityEngine;

public class MiniDropManager : MonoBehaviour {


    public GameObject miniDrop;
    public Transform[] InstancePos = new Transform[8];

    private GameManager gameManager;
    private PuzzleDrop.DROP_TYPE[] beforeType = new PuzzleDrop.DROP_TYPE[8]; 

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        // 初期化
        for(int i = 0; i < beforeType.Length; i++)
        {
            beforeType[i] = PuzzleDrop.DROP_TYPE.NULL;
        }
    }

    void Update()
    {
        int subPanelWidth = 6;
        int y = subPanelWidth - 1; // サブパネルのy軸の数
        for(int x = 0; x < InstancePos.Length; x++)
        {
            

            if (gameManager.subPanel[y * subPanelWidth + x] != null)
            {
                


                /*
                PuzzleDrop currentDrop = gameManager.subPanel[y * subPanelWidth + x];


                    // すでに同じ物が生成されていいないかをチェックする
                    if (beforeType[x] != currentDrop.dropType)
                    {
                        // 今から生成するものを保管する
                        beforeType[x] = currentDrop.dropType;

                        // 元のsubに応じて生成するものを変化させる
                        switch (currentDrop.dropType)
                        {
                            case PuzzleDrop.DROP_TYPE.BLUE:
                                MiniDropDestroy(x);
                                MiniDropInstance(x, "blueSDrop");
                                break;
                            case PuzzleDrop.DROP_TYPE.GREEN:
                                MiniDropDestroy(x);
                                MiniDropInstance(x, "greenSDrop");
                                break;
                            case PuzzleDrop.DROP_TYPE.PURPLE:
                                MiniDropDestroy(x);
                                MiniDropInstance(x, "purpleSDrop");
                                break;
                            case PuzzleDrop.DROP_TYPE.RED:
                                MiniDropDestroy(x);
                                MiniDropInstance(x, "redSDrop");
                                break;
                            case PuzzleDrop.DROP_TYPE.YELLOW:
                                MiniDropDestroy(x);
                                MiniDropInstance(x, "yellowSDrop");
                                break;
                            default:
                                break;
                        }
                    
                }
            }
            else
            {
                // 子供となっているminiDropを削除する
                if(InstancePos[x].childCount >= 1)
                {
                    beforeType[x] = PuzzleDrop.DROP_TYPE.NULL;
                    InstancePos[x].DestroyChildren();
                }
                */
            }
            
        }
    }

    // ミニドロップを生成する
    void MiniDropInstance(int x, string spriteName)
    {
        GameObject obj = Instantiate(miniDrop) as GameObject;
        obj.GetComponent<UISprite>().spriteName = spriteName;
        obj.transform.parent = InstancePos[x];
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    // ドロップを削除する
    void MiniDropDestroy(int x)
    {
        if (InstancePos[x].childCount >= 1)
        {
            InstancePos[x].DestroyChildren();
        }
    }
}
