using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public static Coin instance;

    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] Animator coinAnim;

    public int score;
 
    private void Awake()
    {
        instance = this;
    }

    public int CoinCount = 0;

    private void Start()
    { 
        CoinCount=PlayerPrefs.GetInt("CoinCount");
        score = PlayerPrefs.GetInt("score");
        coinText.text = CoinCount.ToString();
    }

    public void IncreaseCoin(int val)
    {
        CoinCount += val;
        PlayerPrefs.SetInt("CoinCount", CoinCount);
        coinText.text = CoinCount.ToString();
        coinAnim.SetTrigger("collect");
        score += (int)(val* Random.Range(0f,1f));
    }


    [SerializeField] private GameObject GameOverMenu;

    public void GoNextLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }


    public void WatchAdAndContin()
    {
        // AD watcher
        GameOverMenu.SetActive(false);

    }

}
