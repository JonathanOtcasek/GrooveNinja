using UnityEngine;

public class GrassScrollerScript : MonoBehaviour
{
    public float ySpeed;
    public Material myRend;

    // Start is called before the first frame update
    void Start()
    {
        myRend = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        myRend.mainTextureOffset = new Vector2(myRend.mainTextureOffset.x, (ySpeed * Time.time));
    }
}
