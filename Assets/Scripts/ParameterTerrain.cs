using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ParameterTerrain : MonoBehaviour
{
   public static ParameterTerrain instance;

   public int seed = 1;
   public float scale = 100;
    public int octaves = 4;
    public float persistance = 0.378f;
    public float lacunarity = 2.5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

    }
   
}
