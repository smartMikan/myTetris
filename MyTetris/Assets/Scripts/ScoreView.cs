using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    public static ScoreView instance;
    Text text;
    
    void Awake()
    {
        instance = this;
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    public void UpdateScore()
    {
        text.text = "Score: " + "\n" + GameController.instance.Score; 
    }
}
