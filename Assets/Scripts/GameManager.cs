using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   public void ClosePopUp()
   {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
   }
   public void Quit(){
         Application.Quit();
   }
}
