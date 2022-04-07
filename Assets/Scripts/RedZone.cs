using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RedZone : MonoBehaviour
{

    [SerializeField] private GameObject GameOverMenu;

    [SerializeField] private TextMeshProUGUI HighScoreTxt;
    [SerializeField] private TextMeshProUGUI ScoreTxt;

    private bool oneTime=true;


    private void OnTriggerStay(Collider other)
    {
        Cube cube = other.GetComponent<Cube>();
        if (cube != null)
        {
            if (!cube.IsMainCube && cube.CubeRigidbody.velocity.magnitude < 0.1f)
            {
                if (oneTime)
                {
                GameOverMenu.SetActive(true);
                HighScoreTxt.text = PlayerPrefs.GetInt("CoinCount").ToString();
                ScoreTxt.text = Coin.instance.score.ToString();
                oneTime = false;
                }
            }
        }
    }



}
