using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DifficultyText : MonoBehaviour
{
    private Text difficultyText;
    private MapProperties mapProps;

    public float timeBeforeChange;

    public string startDifficulty;
    public string[] difficultyNames;

    private void Start()
    {
        difficultyText = GetComponent<Text>();
        mapProps = GameObject.FindObjectOfType<MapProperties>();
        difficultyText.text = startDifficulty;
    }

    public void ChangeText()
    {
        StartCoroutine(waitForChange());
        
    }

    IEnumerator waitForChange()
    {
        yield return new WaitForSeconds(timeBeforeChange);
        int mapsLen = mapProps.maps.Length;
        int difficulty = -1;
        for (int i = 0; i < mapsLen; i++)
        {
            if (mapProps.maps[i].Index == 0)
            {
                difficulty = mapProps.maps[i].Difficulty;
                difficultyText.text = difficultyNames[difficulty];
            }
        }
    }
}
