using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    public void SceneReload()
    {
        SceneManager.LoadScene(0);
    }
}
