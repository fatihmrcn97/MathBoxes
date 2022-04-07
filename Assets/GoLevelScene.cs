using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoLevelScene : MonoBehaviour
{ 
    public void GoNextLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    
    
    

}
