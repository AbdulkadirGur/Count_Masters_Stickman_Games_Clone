using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyManager : MonoBehaviour
{
    public TextMeshPro TxtSayisi;
    [SerializeField] private GameObject stickMan;
    [Range(0f,1f)] [SerializeField] private float MesafeFaktoru, Aci;

    public Transform enemy;
    public bool saldiri;
    void Start()
    {
        for (int i = 0; i < Random.Range(20,120); i++)
        {
            Instantiate(stickMan, transform.position, new Quaternion(0f, 180f, 0f, 1f), transform);
        }

        TxtSayisi.text = (transform.childCount - 1).ToString();

        FormatStickMan();
    }

    private void FormatStickMan()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var x = MesafeFaktoru * Mathf.Sqrt(i) * Mathf.Cos(i * Aci);
            var z = MesafeFaktoru * Mathf.Sqrt(i) * Mathf.Sin(i * Aci);
            
            var NewPos = new Vector3(x,-0.6f,z);

            transform.transform.GetChild(i).localPosition = NewPos;
        }
    }
    
    void Update()
    {
        if (saldiri && transform.childCount > 1)
        {
            var dusmanYonlendirici = enemy.position - transform.position;

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.Slerp(transform.GetChild(i).rotation,quaternion.LookRotation(dusmanYonlendirici,Vector3.up),
                    Time.deltaTime * 3f);

                if (enemy.childCount > 1)
                {
                    var uzaklik = enemy.GetChild(1).position - transform.GetChild(i).position;

                    if (uzaklik.magnitude < 1.5f)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position,
                            enemy.GetChild(1).position,Time.deltaTime * 2f);
                    } 
                }
              
            }
        }
    }

    public void SaldiriYap(Transform enemyForce)
    {
        enemy = enemyForce;
        saldiri = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("run",true); 
        }
    }

    public void SaldiriDursun()
    {
         PlayerManager.PlayerManagerInstance.oyunDurumu =  saldiri = false;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Animator>().SetBool("run",false);
        }
        
    }
}
