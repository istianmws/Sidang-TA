using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
      public void ClosePopUp()
      {
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
      }
      public void Quit()
      {
            Application.Quit();
      }
      //back to scene main menu
      public void Back()
      {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
      }
}
