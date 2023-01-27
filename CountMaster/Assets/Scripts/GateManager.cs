using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GateManager : MonoBehaviour
{
    public TextMeshPro KapiNo;
    public int randomSayi;
    public bool carpma;
    void Start()
    {
        if (carpma)// eger carpma true olursa 1,2 arasi deger uretir
        {
            randomSayi = Random.Range(1, 3);
            KapiNo.text = "X" + randomSayi;
        }
        else
        {
            randomSayi = Random.Range(10, 100);

            if (randomSayi % 2 != 0)
                randomSayi += 1;
            
            KapiNo.text = randomSayi.ToString();
        }
    }
    
}
