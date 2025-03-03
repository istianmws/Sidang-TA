using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField seedInput;
    public TMP_InputField scaleInput;
    public TMP_InputField octavesInput;
    public TMP_InputField persistanceInput;
    public TMP_InputField lacunarityInput;
    public void ClosePopUp()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Play()
    {
        SaveChanges();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

    }
    public void Quit()
    {
        Application.Quit();
    }
    //back to scene main menu
    public void Back()
    {
        SaveChanges();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void SaveChanges()
    {
        ParameterTerrain.instance.seed = int.Parse(seedInput.text);
        ParameterTerrain.instance.scale = float.Parse(scaleInput.text);
        ParameterTerrain.instance.octaves = int.Parse(octavesInput.text);
        ParameterTerrain.instance.persistance = float.Parse(persistanceInput.text);
        ParameterTerrain.instance.lacunarity = float.Parse(lacunarityInput.text);
    }
}
