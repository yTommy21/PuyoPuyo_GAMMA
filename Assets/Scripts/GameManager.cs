using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // メインパネルの数
    private int mainPanelWidth = 8;

    private int mainPanelHeight = 6;

    // サブパネルの数
    private int subPanelWidth = 8;

    private int subPanelHeight = 6;

    // 画像マージンサイズ
    private float MainDropImageMargin = 80;

    // Drop達
    public GameObject[] drops;

    // cureDrop

    // ドロップが消える時間の遅延
    public float destroyDelay;
    public float lineEffectDilay = 1.0f;
    public float TouchDestroyDelay;
    
    //
    public PuzzleDrop[] subPanel = new PuzzleDrop[6 * 8];   // 左上が0

    public PuzzleDrop[] mainPanel = new PuzzleDrop[6 * 8];  // 左下が0

    // subPanel原点座標
    public Vector2 SubPanelZeroPos = new Vector2(-279, 352);

    //private Vector2 SubPanelZeroPos = new Vector2(-280, -48);

    // 親となるパネルオブジェクト
    public GameObject panelObj;

    private bool isAnimation = false;

    public bool isInput = false;

    public bool isGetInputTrigger = false;

    private bool initTrigger = false;

    private bool updateTrigger = true;

    private List<Vector2> destroyTargetArrayPos = new List<Vector2>();
    private List<Vector2> checkTriggerPos = new List<Vector2>();

    private bool comboTrigger = false;

    public int comboCount = 0;

    public GameObject ComboEffect;

    private GameObject instanceComboEffectObj = null;

    private MiniDropManager2 minidropManager;


    public List<int> explosionIDPos = new List<int>();

    public GameObject lineEffect;

    private List<GameObject> instanceLineEffect = new List<GameObject>();

    void Awake()
    {
        minidropManager = GetComponent<MiniDropManager2>();
    }

    // 初期化処理
    private IEnumerator Start()
    {
        isGetInputTrigger = false;
        initTrigger = false;
        while (true)
        {
            // sabにドロップを生成
            InstanceDropToSubPanel();
            // subに4つ以上のつながりが存在していれば作り直す
            if (!StartSubPanelComboCheck()) { break; }
        }
         
        // サブにレアを１つ生成する
        SetSubRare();


        // sabのドロップを落下させる
        DownSubPanelDrops();

        // minidropの画像を更新する
        minidropManager.SpriteChenge();

        // 移動の処理が完了するまで待機
        yield return StartCoroutine(this.MoveFinCheck());
        // subのドロップを補充する
        SubPanelStockUp();

        // サブにレアを１つ生成する
        SetSubRare();

        // minidropの画像を更新する
        minidropManager.SpriteChenge();

        // メインパネルのidを再設定する
        IDSet();

        // 初期化処理が終了したことを渡す
        initTrigger = true;

        yield break;
    }

    private IEnumerator InputCheck ()
    {
        isGetInputTrigger = false;
        updateTrigger = false;
        bool subTrigger = false;

        // 爆発座標をリセットする
        explosionIDPos.Clear();

        while (true)
        {
            yield return new WaitForSeconds(TouchDestroyDelay);
            // メインのドロップを落下させる
            MainPanelDownDrops();

            // メインパネルのidを再設定する
            IDSet();

            // ラインエフェクトを生成する
            yield return new WaitForSeconds(lineEffectDilay);
            InstanceLineEffect();

            // 移動処理終了まで待機
            yield return StartCoroutine(this.MoveFinCheck());

            // コンボが５回以上つづくたびに
            if (comboCount >= 5)
            {
                // サブにレアを１つ生成する
                SetSubRare();
            }

            while (true)
            {
                // ４個以上つながっていれば削除する
                if (!DropDestroyCheck()) 
                {
                    // 4個つながりがなかった場合
                    // サブトリガーをチェック
                    if (subTrigger)
                    {

                        // もしrareドロップが１度でも破壊されれば爆発を起こさせる
                        if (explosionIDPos.Count >= 1)
                        {
                            // ドロップを爆発させる
                            DropExplosion();
                            // ラインエフェクトを削除する
                            DestroyLineEffect();
                            // 爆発座標をリセットする
                            explosionIDPos.Clear();

                            subTrigger = false;
                            break;
                        }
                        // コルーチンを終了する
						// 
						instanceLineEffect.Clear();
                        updateTrigger = true;
                        yield break;
                    }
                }
                else
                {
                    // コンボエフェクトを表示する
                    comboCount++;
                    InstanceComboEffect();

                    // コンボエフェクトが終了するまで待機
                    yield return StartCoroutine(this.ComboEffectFinCheck());

                    subTrigger = false;
                    break;
                }
                // メインの空白にsubからdropを落下させる
                DownSubPanelDrops();

                // メインパネルのidを再設定する
                IDSet();

                // minidropの画像を更新する
                minidropManager.SpriteChenge();

                // ラインエフェクトを生成する
                yield return new WaitForSeconds(lineEffectDilay);
                InstanceLineEffect();

                // 移動が終了するまで待機
                yield return StartCoroutine(this.MoveFinCheck());

                // サブのdropを補充する
                SubPanelStockUp();

                // すでに1個以上レアドロップが存在しているかチェック
                if(!CheckSubRareOne())
                {
                    SetSubRare();
                }

                // minidropの画像を更新する
                minidropManager.SpriteChenge();

                // トリガーをセットする
                subTrigger = true;
            }
        }
    }

    private void InstanceComboEffect()
    {
        instanceComboEffectObj = Instantiate(ComboEffect) as GameObject;
        instanceComboEffectObj.transform.parent = panelObj.transform;
        instanceComboEffectObj.transform.localPosition = new Vector3(-41, 241, 0);
        instanceComboEffectObj.transform.localScale = Vector3.one;
    }

    // 移動処理をしている場合処理を止める処理
    private IEnumerator MoveFinCheck()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (DropsDownCheck()) { yield break; }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // コンボエフェクトが再生されている間処理を止める処理　
    private IEnumerator ComboEffectFinCheck()
    {
        yield return new WaitForSeconds(0.1f);

        // debug
        yield return new WaitForSeconds(0.5f);
        yield break;
        //
    }

    private void Update()
    {
        // 初期化処理が終了しているかを確認する
        if (initTrigger)
        {
            // 処理が終了していれば入力を認識できるようにする
            if (updateTrigger)
            {
                isGetInputTrigger = true;
                comboCount = 0;
                if (isInput)
                {
                    isInput = false;
                    StartCoroutine(InputCheck());
                }
            }
        }
    }



    private bool StartSubPanelComboCheck()
    {
        bool destroyTrigger = false;

        // 左下からメインパネルのドロップが４つ以上つながっていないかチェックする
        for (int y = 0; y < subPanelHeight; y++)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                // ドロップがなければスキップ
                if (subPanel[y * subPanelWidth + x] == null) { continue; }
                // すでにチェックされていればスキップする
                foreach (Vector2 vec2 in checkTriggerPos)
                {
                    if (vec2 == new Vector2(x, y))
                    {
                        continue;
                    }
                }

                // 現在選択されている色を取得する
                PuzzleDrop.DROP_TYPE currentType = subPanel[y * subPanelWidth + x].GetComponent<PuzzleDrop>().dropType;

                // 同じ色のdropを取得して数が４個以上なら削除する
                if (4 <= GetSubNodeColorCount(currentType, new Vector2(x, y)))
                {                    
                    destroyTrigger = true;
                }
                // カウントをリセットする
                destroyTargetArrayPos.Clear();
            }
        }
        checkTriggerPos.Clear();
        return destroyTrigger;
    }

    private void InstanceDropToSubPanel()
    {
        // すでに存在しているsubをすべて破壊する
        for (int y = 0; y < subPanelHeight; y++)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                if (subPanel[y * subPanelWidth + x] != null)
                {
                    // オブジェクトを削除する
                    Destroy(subPanel[y * subPanelWidth + x].gameObject); 
                    //subPanel[y * subPanelWidth + x].Destroy();
                    subPanel[y * subPanelWidth + x] = null;
                }
            }
        }

        // サブパネルにすべてランダムにDropを割り当てなおす
        for (int y = 0; y < subPanelHeight; y++)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                int random = Random.Range(0, drops.Length);
                subPanel[y * subPanelWidth + x] = Instantiate(drops[random].GetComponent<PuzzleDrop>());
                // panelの子供オブジェクトとする
                subPanel[y * subPanelWidth + x].transform.parent = panelObj.transform;
                subPanel[y * subPanelWidth + x].transform.localScale = Vector3.one;
                // 座標を渡す
                subPanel[y * subPanelWidth + x].GetComponent<PuzzleDrop>().pos =
                    new Vector2(SubPanelZeroPos.x + MainDropImageMargin * x, SubPanelZeroPos.y + MainDropImageMargin * y * -1);
                // idを渡す
                //subPanel[y * subPanelWidth + x].GetComponent<PuzzleDrop>().dropID = y * subPanelWidth + x;

            }
        }
    }

    private void DownSubPanelDrops()
    {
        // mainに空きがあるか確認
        // スペースの数をx軸ごとに保管
        int[] spaceCounts = new int[subPanelWidth];
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                int y = mainPanelHeight - 1;
                while (true)
                {
                    if (y == -1) { break; }
                    if (mainPanel[y * mainPanelWidth + x] != null) { break; }
                    spaceCounts[x]++;
                    y--;
                }
            }
        }

        // スペースが開いている数だけsubから落下させる

        // todo:数が多い順番で落下させる
        // 同時に落下させる
        // 縦の列ごとに値を渡す
        for (int x = 0; x < subPanelWidth; x++)
        {
            if (spaceCounts[x] > 0)
            {
                int subCount = 1;
                while (true)
                {
                    if (subCount > spaceCounts[x]) { break; }
                    subPanel[(subPanelHeight - subCount) * subPanelWidth + x].GetComponent<PuzzleDrop>().MoveDownSet(spaceCounts[x]);

                    subCount++;
                }
            }
        }

#if true

        if (true)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                if (spaceCounts[x] > 0)
                {

                    int subCount = 1;
                    while (spaceCounts[x] > 0)
                    {
                        // メインパネルにdropを移動
                        mainPanel[(mainPanelHeight - spaceCounts[x]) * mainPanelWidth + x]
                            = subPanel[(subPanelHeight - subCount) * subPanelWidth + x];



                        // サブパネルのdropを削除
                        subPanel[(subPanelHeight - subCount) * subPanelWidth + x] = null;

                        // カウント
                        spaceCounts[x]--;
                        subCount++;
                    }
                }
            }
        }
#endif
    }

    // すべて落下が完了したかどうかを返す true:落下完了 false:落下未完了
    private bool DropsDownCheck()
    {
        for (int y = 0; y < subPanelHeight; y++)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                if (subPanel[y * subPanelWidth + x] != null)
                {
                    if (subPanel[y * subPanelWidth + x].isMoving) { return false; }
                }

                if (mainPanel[y * mainPanelWidth + x] != null)
                {
                    if (mainPanel[y * mainPanelWidth + x].isMoving) { return false; }
                }
            }
        }
        return true;
    }

    // 空白になったsubPanelに新しくdropを渡す
    private void SubPanelStockUp()
    {
        for (int y = 0; y < mainPanelHeight; y++)
        {
            for (int x = 0; x < subPanelWidth; x++)
            {
                if (subPanel[y * subPanelWidth + x] == null)
                {
                    int random = Random.Range(0, drops.Length);
                    subPanel[y * subPanelWidth + x] = Instantiate(drops[random].GetComponent<PuzzleDrop>());
                    // panelの子供オブジェクトとする
                    subPanel[y * subPanelWidth + x].transform.parent = panelObj.transform;
                    subPanel[y * subPanelWidth + x].transform.localScale = Vector3.one;
                    // 座標を渡す
                    subPanel[y * subPanelWidth + x].GetComponent<PuzzleDrop>().pos =
                        new Vector2(SubPanelZeroPos.x + MainDropImageMargin * x, SubPanelZeroPos.y + MainDropImageMargin * y * -1);
                    // idを渡す
                    //subPanel[y * subPanelWidth + x].GetComponent<PuzzleDrop>().dropID = y * subPanelWidth + x;
                }
            }
        }
    }

    // メインパネル内の空白を下から順番に確認して、もし空白ならそのマスの上を見て一番初めに取得できたdropを入れて配列を整理する
    private void MainPanelDownDrops()
    {
        for (int y = 0; y < mainPanelHeight - 1; y++)
        {
            for (int x = 0; x < mainPanelWidth; x++)
            {
                // 空白を見つける
                if (mainPanel[y * mainPanelWidth + x] == null)
                {
                    // 空白があればその上にあるドロップ１つを現在地まで落とす
                    int count = 1;
                    while ((mainPanelHeight > y + count))
                    {
                        // 同じy軸の上のdropを確認
                        int targetY = y + count;
                        //int targetY = count - 1;
                        // dropが見つかれば
                        if (mainPanel[targetY * mainPanelWidth + x] != null)
                        {
                            // ブロックを移動させて
                            mainPanel[targetY * mainPanelWidth + x].GetComponent<PuzzleDrop>().MoveDownSet(count);
                            // 配列を入れ替える
                            mainPanel[y * mainPanelWidth + x] = mainPanel[targetY * mainPanelWidth + x];
                            // 前の配列を削除する
                            //Destroy(mainPanel[targetY * mainPanelWidth + x].gameObject);
                            mainPanel[targetY * mainPanelWidth + x] = null;
                            break;
                        }
                        count++;
                    }
                }
            }
        }
    }

    // 4個以上つながっているドロップを削除する true:つながりがある
    private bool DropDestroyCheck()
    {
        bool destroyTrigger = false;
        // チェックしたかどうかのトリガー
        //bool[] checkTrigger = new bool[mainPanelWidth * mainPanelHeight];

        // 左下からメインパネルのドロップが４つ以上つながっていないかチェックする
        for (int y = 0; y < mainPanelHeight; y++)
        {
            for (int x = 0; x < mainPanelWidth; x++)
            {
                // ドロップがなければスキップ
                if (mainPanel[y * mainPanelWidth + x] == null) { continue; }
                // すでにチェックされていればスキップする
                //if(checkTrigger[y * mainPanelWidth + x]) { continue; }
                foreach (Vector2 vec2 in checkTriggerPos)
                {
                    if (vec2 == new Vector2(x, y))
                    {
                        continue;
                    }
                }

                // チェックトリガーをオンにする
                //checkTrigger[y * mainPanelWidth + x] = true;
                // 現在選択されている色を取得する
                PuzzleDrop.DROP_TYPE currentType = mainPanel[y * mainPanelWidth + x].GetComponent<PuzzleDrop>().dropType;

                // 同じ色のdropを取得して数が４個以上なら削除する
                if (4 <= GetMainNodeColorCount(currentType, new Vector2(x, y)))
                {
                    foreach (Vector2 vec2 in destroyTargetArrayPos)
                    {
                        //Destroy(mainPanel[(int)vec2.y * mainPanelWidth + (int)vec2.x].gameObject);
						//mainPanel[(int)vec2.y * mainPanelWidth + (int)vec2.x].Destroy();
						mainPanel[(int)vec2.y * mainPanelWidth + (int)vec2.x].DropDestroy();
                        mainPanel[(int)vec2.y * mainPanelWidth + (int)vec2.x] = null;
                    }
                    destroyTrigger = true;
                }
                // カウントをリセットする
                destroyTargetArrayPos.Clear();
            }
        }
        checkTriggerPos.Clear();
        return destroyTrigger;
    }

    // mainPanelの同じ色のdropを取得する
    private int GetMainNodeColorCount(PuzzleDrop.DROP_TYPE type, Vector2 arrayPos)
    {
        foreach (Vector2 vec2 in checkTriggerPos)
        {
            if (vec2 == arrayPos) { return 0; }
        }
        checkTriggerPos.Add(arrayPos);
        if (mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x] == null) { return 0; }
        int count = 0;
        // 自身の色が目的の色と同じならカウントを増加させる
        PuzzleDrop drop = mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x];
        if (drop.dropType == type)
        {
            destroyTargetArrayPos.Add(arrayPos);
            count++;
        }
        else
        {
            return 0;
        }

        // 上方向をチェック
        if (!((int)arrayPos.y + 1 >= mainPanelHeight))
        {
            if (mainPanel[((int)arrayPos.y + 1) * mainPanelWidth + (int)arrayPos.x] != null)
            {
                if (mainPanel[((int)arrayPos.y + 1) * mainPanelWidth + (int)arrayPos.x].dropType == type)
                {
                    count += GetMainNodeColorCount(type, new Vector2(arrayPos.x, arrayPos.y + 1));
                }
            }
        }
        // 下方向をチェック
        if (!((int)arrayPos.y - 1 <= -1))
        {
            if (mainPanel[((int)arrayPos.y - 1) * mainPanelWidth + (int)arrayPos.x] != null)
            {
                if (mainPanel[((int)arrayPos.y - 1) * mainPanelWidth + (int)arrayPos.x].dropType == type)
                {
                    count += GetMainNodeColorCount(type, new Vector2(arrayPos.x, arrayPos.y - 1));
                }
            }
        }

        // 左方向をチェック
        if (!((int)arrayPos.x - 1 <= -1))
        {
            if (mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x - 1] != null)
            {
                if (mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x - 1].dropType == type)
                {
                    count += GetMainNodeColorCount(type, new Vector2(arrayPos.x - 1, arrayPos.y));
                }
            }
        }

        // 右方向をチェック
        if (!((int)arrayPos.x + 1 >= mainPanelWidth))
        {
            if (mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x + 1] != null)
            {
                if (mainPanel[(int)arrayPos.y * mainPanelWidth + (int)arrayPos.x + 1].dropType == type)
                {
                    count += GetMainNodeColorCount(type, new Vector2(arrayPos.x + 1, arrayPos.y));
                }
            }
        }

        return count;
    }

    // subPanelの同じ色のdropを取得する
    private int GetSubNodeColorCount(PuzzleDrop.DROP_TYPE type, Vector2 arrayPos)
    {
        foreach (Vector2 vec2 in checkTriggerPos)
        {
            if (vec2 == arrayPos) { return 0; }
        }
        checkTriggerPos.Add(arrayPos);
        if (subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x] == null) { return 0; }
        int count = 0;
        // 自身の色が目的の色と同じならカウントを増加させる
        PuzzleDrop drop = subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x];
        if (drop.dropType == type)
        {
            destroyTargetArrayPos.Add(arrayPos);
            count++;
        }
        else
        {
            return 0;
        }

        // 下方向をチェック
        if (!((int)arrayPos.y + 1 >= subPanelHeight))
        {
            if (subPanel[((int)arrayPos.y + 1) * subPanelWidth + (int)arrayPos.x] != null)
            {
                if (subPanel[((int)arrayPos.y + 1) * subPanelWidth + (int)arrayPos.x].dropType == type)
                {
                    count += GetSubNodeColorCount(type, new Vector2(arrayPos.x, arrayPos.y + 1));
                }
            }
        }

        // 上方向をチェック
        if (!((int)arrayPos.y - 1 <= -1))
        {
            if (subPanel[((int)arrayPos.y - 1) * subPanelWidth + (int)arrayPos.x] != null)
            {
                if (subPanel[((int)arrayPos.y - 1) * subPanelWidth + (int)arrayPos.x].dropType == type)
                {
                    count += GetSubNodeColorCount(type, new Vector2(arrayPos.x, arrayPos.y - 1));
                }
            }
        }

        // 左方向をチェック
        if (!((int)arrayPos.x - 1 <= -1))
        {
            if (subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x - 1] != null)
            {
                if (subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x - 1].dropType == type)
                {
                    count += GetSubNodeColorCount(type, new Vector2(arrayPos.x - 1, arrayPos.y));
                }
            }
        }

        // 右方向をチェック
        if (!((int)arrayPos.x + 1 >= subPanelWidth))
        {
            if (subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x + 1] != null)
            {
                if (subPanel[(int)arrayPos.y * subPanelWidth + (int)arrayPos.x + 1].dropType == type)
                {
                    count += GetSubNodeColorCount(type, new Vector2(arrayPos.x + 1, arrayPos.y));
                }
            }
        }

        return count;
    }

    // サブパネルにレアが存在しているかを確認 true...存在している
    bool CheckSubRareOne()
    {
        for (int y = 0; y < subPanelHeight; y++)
        {
            for(int x = 0; x < subPanelWidth; x++)
            {
                if(subPanel[y * subPanelWidth + x].isRare) { return true; }
            }
        }
        return false;
    }

    // サブパネルのドロップを１つレアにする
    void SetSubRare()
    {
        while(true)
        {
            int random = Random.Range(0, subPanelWidth * subPanelHeight);
            if(subPanel[random].isRare != true)
            {
                subPanel[random].isRare = true;
                break;
            }
        }
    }

    // メインパネルのドロップにIDを渡す
    void IDSet()
    {
        for(int y = 0; y < mainPanelHeight; y++)
        {
            for(int x = 0; x < mainPanelWidth; x++)
            {
                int id = y * mainPanelWidth + x;
                if(mainPanel[y * mainPanelWidth + x] != null)
                {
                    mainPanel[y * mainPanelWidth + x].dropID = id;
                    mainPanel[y * mainPanelWidth + x].isMain = true;
                }
            }
        }
    }

    // ドロップを爆破させる
    void DropExplosion()
    {
        foreach (int posID in explosionIDPos)
        {
            // 選択された位置を中心に縦方向のDropを削除する

            // 選択された位置を削除
            if(mainPanel[posID] != null)
            {
                mainPanel[posID].BombDestroy();
                mainPanel[posID] = null;
            }

            // 上方向を削除
            int count = 1;
            while(true)
            {
                int targetID = posID + (count * mainPanelWidth);
                if (targetID >= mainPanelWidth * mainPanelHeight) { break; }
                if(mainPanel[targetID] != null)
                {
                    mainPanel[targetID].BombDestroy();
                    mainPanel[targetID] = null;
                }
                count++;
            }
            // 下方向を削除
            count = 1;
            while (true)
            {
                int targetID = posID - (count * mainPanelWidth);
                if (targetID < 0) { break; }
                if (mainPanel[targetID] != null)
                {
                    mainPanel[targetID].BombDestroy();
                    mainPanel[targetID] = null;
                }
                count++;
            }
        }

    }

    void InstanceLineEffect()
    {
        List<int> instanceX = new List<int>();
        foreach (int idPos in explosionIDPos)
        {
            bool doNotInstanceTrigger = false;

            foreach(GameObject obj in instanceLineEffect)
            {
                int x = idPos % mainPanelWidth;
                if (obj != null)
                {
                    if (obj.transform.localPosition.x == (x * 80 + (-280)))
                    {
                        doNotInstanceTrigger = true;
                    }
                }
            }


            if (doNotInstanceTrigger == false)
            {
                // ラインエフェクトを生成する
                GameObject effect = Instantiate(lineEffect) as GameObject;
                effect.transform.parent = panelObj.transform;
                int x = idPos % mainPanelWidth;
                int y = idPos / mainPanelWidth;
                effect.transform.localPosition = new Vector3(x * 80 + (-280), -180, 0);
                effect.transform.localScale = Vector3.one;

                instanceLineEffect.Add(effect);

                instanceX.Add(x);
            }
        }
    }

    void DestroyLineEffect()
    {
        foreach(GameObject obj in instanceLineEffect)
        {
			if (obj != null)
			{
				Destroy (obj.gameObject);
			}
        }
    }

}