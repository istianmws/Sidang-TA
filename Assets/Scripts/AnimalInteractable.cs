using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInteractable : MonoBehaviour
{
   public void Interact()
   {
      Debug.Log("Interacting with animal"+ gameObject.name);
   }
   public enum AnimalType
   {
      Kucing,
      Dog,
      Deer,
      Tiger,
      Chicken,
      Cat,
      Horse,
      Penguin,
   }
   public AnimalType animalType;
   public string animalText;   
}
