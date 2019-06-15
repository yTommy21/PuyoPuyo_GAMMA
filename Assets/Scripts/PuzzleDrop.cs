using UnityEngine;
using System.Collections;

public class PuzzleDrop : MonoBehaviour
{

    public enum DROP_TYPE
    {
        RED,
        GREEN,
        BLUE,
        YELLOW,
        PURPLE,
        CURE,

        NULL,

        NUM
    }


    private GameManager gameManager;

    public GameObject destroyEffect;
    private Transform instanceTransform;

    public DROP_TYPE dropType;
    public bool isBoom;
    public Vector2 pos;
    public Vector2 targetPos;
    public bool isMoving;
    public bool hasMove;
    //private float Downspeed = 7f;
    private bool clickTrigger = false;
    public float downSpeed = 1.0f;
	public GameObject effect;

    public bool isMain = false;

    [Header("Rare関係")]
    public bool isRare = false;
    public UISprite rareSprite;
    
    [Header("識別用id")]
    public int dropID;

    [Header("Input選択用")]
    public bool isInputSelectting = false;
    private TweenAlpha tweenAlpha;


    protected virtual bool DestroyCheck()
    {
        return true;
    }

    void Awake()
    {
        tweenAlpha = GetComponent<TweenAlpha>();
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        instanceTransform = this.transform.parent;
    }

    void Update()
    {
        MoveDown();
        transform.localPosition = pos;

        RareCheck();

        // 選択されていれば点滅させる
        if(isInputSelectting)
        {
            if (!tweenAlpha.enabled)
            {
                tweenAlpha.enabled = true;
            }
        }

    }

    void RareCheck()
    {
        if(isRare)
        {
            rareSprite.enabled = true;
        }
        else
        {
            rareSprite.enabled = false;
        }
    }

    // downCount分落下する
    public void MoveDownSet(int downCount)
    {
        float moveValue = 80f * downCount;
        targetPos = pos - (Vector2.up * moveValue);
        isMoving = true;
        hasMove = true;
    }

    // 移動落下する
    void MoveDown()
    {
        if (isMoving)
        {
            pos.y -= downSpeed * Time.deltaTime;
            if (pos.y < targetPos.y)
            {
                isMoving = false;
                pos.y = targetPos.y;
            }
        }
    }

    // クリックされたときの処理
    public void OnDropClick()
    {
        if (gameManager.isGetInputTrigger)
        {
            clickTrigger = true;

            // エフェクト再生
            InstanceEffect();

            // 削除
            Destroy(this.gameObject);
        }
    }

    // 4つ以上になって消える場合
    public void DropDestroy()
	{
		// エフェクト再生
        StartCoroutine("InstEffect");

        // 削除
        Destroy(this.gameObject, gameManager.destroyDelay);
	}
    
    public void BombDestroy()
    {
        // エフェクト再生
        InstanceEffect();

        // 削除
        Destroy(this.gameObject);
    }

    public void OnDestroy()
    {

        // もし自分がレアな場合自身のid座標をGameManagerに渡す
        if (isRare)
        {
            gameManager.explosionIDPos.Add(dropID);
        }

        if (clickTrigger)
        {
            clickTrigger = false;
            gameManager.isInput = true;

        }
    }

    IEnumerator InstEffect()
    {
        yield return new WaitForSeconds(gameManager.destroyDelay - 0.1f);
        InstanceEffect();
        yield break;
    }

    public void InstanceEffect()
    {
        // エフェクトを生成
        effect = Instantiate(destroyEffect) as GameObject;
        effect.transform.parent = instanceTransform;
        effect.transform.localPosition = this.transform.localPosition;
        effect.transform.localScale = this.transform.localScale;
        // ドロップの種類を渡す
        effect.GetComponent<dropDestroyEffectManager>().AnimationSet(dropType);
    }

	public void DestroyEffect()
	{
		if (effect != null)
		{
			Destroy (effect.gameObject);
		}
	}



}
