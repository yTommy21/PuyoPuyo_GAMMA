using UnityEngine;

public class MiniDropManager2 : MonoBehaviour {

    public UISprite[] sprites = new UISprite[8];
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GetComponent<GameManager>();
        foreach(UISprite sp in sprites)
        {
            sp.spriteName = "";
        }
    }
    

    void Update()
    {
        //SpriteChenge();
    }
    
    public bool SpriteChenge()
    {
        int y = 5;
        int width = 8;
        for (int x = 0; x < 8; x++)
        {

            if(gameManager.subPanel[y * width + x] == null) { sprites[x].spriteName = ""; continue; }

            PuzzleDrop.DROP_TYPE currentType = gameManager.subPanel[y * width + x].dropType;



            string spriteName = "";
            switch (currentType)
            {
                case PuzzleDrop.DROP_TYPE.BLUE:
                    spriteName = "blueSDrop";
                    break;
                case PuzzleDrop.DROP_TYPE.GREEN:
                    spriteName = "greenSDrop";
                    break;
                case PuzzleDrop.DROP_TYPE.PURPLE:
                    spriteName = "purpleSDrop";
                    break;
                case PuzzleDrop.DROP_TYPE.RED:
                    spriteName = "redSDrop";
                    break;
                case PuzzleDrop.DROP_TYPE.YELLOW:
                    spriteName = "yellowSDrop";
                    break;
            }

            // スプライトの画像を更新する
            sprites[x].spriteName = spriteName;
        }

        return true;
    }

}
