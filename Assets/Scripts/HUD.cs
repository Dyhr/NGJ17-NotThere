using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Transform health;
    public Text level;
    private float wid;

    private void Start()
    {
    }

    private void Update()
    {
        if(Guard.player != null)
            health.localScale = new Vector3(Guard.player.GetComponent<Human>().hp/100,1,1);
        level.text = "Level "+(ProgressTracker.instance.level+1)+"/3";
    }

    public void SetCardName(Text text)
    {
        text.text = "Access Card " + NetNode.Roman(GameObject.FindGameObjectWithTag("Player").GetComponent<Human>().level);
    }
}
