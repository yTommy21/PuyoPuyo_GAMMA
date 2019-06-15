using UnityEngine;
using System.Collections;

public class dropDestroyEffectManager : MonoBehaviour
{

    private UISpriteAnimation animation;

    void Awake()
    {
        animation = GetComponent<UISpriteAnimation>();
    }

    public void AnimationSet(PuzzleDrop.DROP_TYPE type)
    {
        switch (type)
        {
            case PuzzleDrop.DROP_TYPE.BLUE:
                animation.namePrefix = "BlueDestroy";
                break;

            case PuzzleDrop.DROP_TYPE.GREEN:
                animation.namePrefix = "GreenDestroy";
                break;

            case PuzzleDrop.DROP_TYPE.PURPLE:
                animation.namePrefix = "PurpleDestroy";
                break;

            case PuzzleDrop.DROP_TYPE.RED:
                animation.namePrefix = "RedDestroy";
                break;

            case PuzzleDrop.DROP_TYPE.YELLOW:
                animation.namePrefix = "YellowDestroy";
                break;
        }
    }

    // Tween処理が終了したら自分を破壊する
    public void TweenFin()
    {
        Destroy(this.gameObject);
    }
}
