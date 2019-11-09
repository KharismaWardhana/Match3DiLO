using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instance sebagai global access
    public static GameManager instance;
    int playerScore;
    public Text scoreText;

    // singleton
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }else if(instance != null)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    //Update score dan ui
    public void GetScore(int point)
    {
        playerScore += point;
        scoreText.text = playerScore.ToString();
    }
}