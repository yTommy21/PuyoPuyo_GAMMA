using UnityEngine;

public class comboEffect : MonoBehaviour {

    private GameManager gameManager;
    public UILabel label;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        label.text = gameManager.comboCount.ToString();
    }

    public void TweenFin()
    {
        Destroy(this.gameObject);
    }

}
