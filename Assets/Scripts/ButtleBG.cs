using UnityEngine;

public class ButtleBG : MonoBehaviour {

    public float speed = 0.1f;
    Renderer render;

    void Awake()
    {
        render = GetComponent<Renderer>();
    }

    void Update()
    {

        float x = Mathf.Repeat(Time.time * speed, 1);
        Vector2 offset = new Vector2(x, 0);
        render.sharedMaterial.SetTextureOffset("_MainTex", offset);
        //renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }

}
