using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Label : MonoBehaviour
{
    public static Label instance;
    public Text text;

    public Transform Target
    {
        set
        {
            gameObject.SetActive(value != null);
            if (value != null)
                transform.position = value.position - transform.forward * 5;
        }
    }

    private void Start()
    {
        instance = this;
        transform.rotation = Camera.main.transform.rotation;
        text = GetComponentInChildren<Text>();
    }
}
