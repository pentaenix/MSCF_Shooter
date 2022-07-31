using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    //Save layout to metadata file and change scene
    public void SetLayout(int layout) {
        File.WriteAllBytes(Application.dataPath + "/Levels/Layout.metadata", new byte[] { Convert.ToByte(layout) });
        SceneManager.LoadScene("Game");
    }
}
