using System.Collections;
using System.Collections.Generic;
using System;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Transform player;
    public int stickmanSayisi,DusmanStickmanSayisi;
    [SerializeField] private TextMeshPro txtSayisi;
    [SerializeField] private GameObject stickMan;
   
    //****************************************************

   [Range(0f,1f)] [SerializeField] private float MesafeFaktoru, Radius;
   
   //*********** playeri hareket Ettirme ********************
   
   public bool ekranaDokunma,oyunDurumu;
   private Vector3 fareBaslangicPos,playerBaslangicPos;
   public float playerHizi,yolHizi;
   private Camera camera;

   [SerializeField] private Transform yol;
   [SerializeField] private Transform enemy;
   private bool saldir;
   public static PlayerManager PlayerManagerInstance;
   public ParticleSystem kan;
   public GameObject SecondCam;
   public bool BitisCizgisi,kameraHareketEttir;


    
    void Start()
    {
        player = transform;
        
        stickmanSayisi = transform.childCount - 1;

        txtSayisi.text = stickmanSayisi.ToString(); // Player objesinin child sayisini texte yazdiriyoruz.
        
        camera = Camera.main;

        PlayerManagerInstance = this;

        oyunDurumu = false;
    }
    
    void Update()
    {
        if (saldir)
        {
            var dusmanYonlendirici = new Vector3(enemy.position.x,transform.position.y,enemy.position.z) - transform.position;

            for (int i = 1; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = 
                    Quaternion.Slerp( transform.GetChild(i).rotation,Quaternion.LookRotation(dusmanYonlendirici,Vector3.up), Time.deltaTime * 3f );
            }

            if (enemy.GetChild(1).childCount > 1)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var Uzaklik = enemy.GetChild(1).GetChild(0).position - transform.GetChild(i).position;

                    if (Uzaklik.magnitude < 1.5f)
                    {
                        transform.GetChild(i).position = Vector3.Lerp(transform.GetChild(i).position, 
                            new Vector3(enemy.GetChild(1).GetChild(0).position.x,transform.GetChild(i).position.y,
                                enemy.GetChild(1).GetChild(0).position.z), Time.deltaTime * 1f );
                    }
                }
            }

            else
            {
                saldir = false;
                yolHizi = 2f;
                
                StickManDagit();

                for (int i = 1; i < transform.childCount; i++)
                    transform.GetChild(i).rotation = Quaternion.identity;              



                enemy.gameObject.SetActive(false);
              
            }

            if (transform.childCount == 1)
            {
                enemy.transform.GetChild(1).GetComponent<enemyManager>().SaldiriDursun();
                gameObject.SetActive(false);
                

            }
            
        }
        else
        {
            KarakterHareketEt();
            
        }

        
        if (transform.childCount == 1 && BitisCizgisi)
        {
            oyunDurumu = false;
            
        }
        


        if (oyunDurumu)
        {
          yol.Translate(yol.forward * Time.deltaTime * yolHizi);
            
           // for (int i = 1; i < transform.childCount; i++)
           // {
           //     if (transform.GetChild(i).GetComponent<Animator>() != null)
           //         transform.GetChild(i).GetComponent<Animator>().SetBool("run",true);
           //    
           // }
        }

        if (kameraHareketEttir && transform.childCount > 1)
        {
            var cinemachineTransposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
              .GetCinemachineComponent<CinemachineTransposer>();

          var cinemachineComposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
              .GetCinemachineComponent<CinemachineComposer>();

          cinemachineTransposer.m_FollowOffset = new Vector3(4.5f, Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y,
              transform.GetChild(1).position.y + 2f, Time.deltaTime * 1f), -5f);
          
          cinemachineComposer.m_TrackedObjectOffset = new Vector3(0f,Mathf.Lerp(cinemachineComposer.m_TrackedObjectOffset.y,
              4f,Time.deltaTime * 1f),0f);
          
        }
       
    }
    
    void KarakterHareketEt()
    {
        if (Input.GetMouseButtonDown(0) && oyunDurumu)
        {
            ekranaDokunma = true;
            
            var plane = new Plane(Vector3.up, 0f);

            var ray = camera.ScreenPointToRay(Input.mousePosition);
            
            if (plane.Raycast(ray,out var distance))
            {
                fareBaslangicPos = ray.GetPoint(distance + 1f);
                playerBaslangicPos = transform.position;
            }

        }
        
        if (Input.GetMouseButtonUp(0))
        {
            ekranaDokunma = false;
            
        }
        
        if (ekranaDokunma)
        { 
            var plane = new Plane(Vector3.up, 0f);
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            
            if (plane.Raycast(ray,out var distance))
            {
                var farePos = ray.GetPoint(distance +  1f);
                   
                var move = farePos - fareBaslangicPos;
                   
                var control = playerBaslangicPos + move;//playerin x ekseninde sinirlandirilmasini kontrol ediyoruz.


                if (stickmanSayisi > 50)
                    control.x = Mathf.Clamp(control.x, -0.7f, 0.7f);
                else
                    control.x = Mathf.Clamp(control.x, -1.1f, 1.1f);

                transform.position = new Vector3(Mathf.Lerp(transform.position.x,control.x,Time.deltaTime * playerHizi)
                    ,transform.position.y,transform.position.z);
                  
            }
            
        }
    }

    public void StickManDagit()
    {
        for (int i = 1; i < player.childCount; i++)
        {
            var x = MesafeFaktoru * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);// -1 ile 1 arasinda deger donderir.
            var z = MesafeFaktoru * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);
            
            var NewPos = new Vector3(x,-0.55f,z);

            player.transform.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);// DOLocalMove Seçili nesneyi Local Eksende hareket ettirir
            //Buradaki Ease.OutBack objenin arkadan öne doðru gelirmiþ gibi görünmesini saðlar.
        }
    }

    public void StickManYap(int number)
    {
        for (int i = stickmanSayisi; i < number; i++)
        {
            Instantiate(stickMan, transform.position, quaternion.identity, transform);//Player Objesinin trasform degeriyle ayni yerde dogacaklar.
        }

        stickmanSayisi = transform.childCount - 1;
        txtSayisi.text = stickmanSayisi.ToString();
        StickManDagit();
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("gate"))
        {
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false; // kapi 1
            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false; // kapi 2

            var gateManager = other.GetComponent<GateManager>();

            stickmanSayisi = transform.childCount - 1;

            if (gateManager.carpma)
            {
                StickManYap(stickmanSayisi * gateManager.randomSayi);
            }
            else
            {
                StickManYap(stickmanSayisi + gateManager.randomSayi);

            }
        }

        if (other.CompareTag("enemy"))
        { 
            enemy = other.transform;
            saldir = true;

            yolHizi = 0.5f;
            
            other.transform.GetChild(1).GetComponent<enemyManager>().SaldiriYap(transform);

            StartCoroutine(UpdateTheEnemyAndPlayerStickMansNumbers());

        }
        

        if (other.CompareTag("Finish"))
        {

            SecondCam.SetActive(true);
            BitisCizgisi = true;
            Tower.TowerInstance.CreateTower(transform.childCount - 1);
            transform.GetChild(0).gameObject.SetActive(false);
            
        }
    }

    IEnumerator UpdateTheEnemyAndPlayerStickMansNumbers()
    {

        DusmanStickmanSayisi = enemy.transform.GetChild(1).childCount - 1;
        stickmanSayisi = transform.childCount - 1;

        while (DusmanStickmanSayisi > 0 && stickmanSayisi > 0)
        {
            DusmanStickmanSayisi--;
            stickmanSayisi--;

            enemy.transform.GetChild(1).GetComponent<enemyManager>().TxtSayisi.text = DusmanStickmanSayisi.ToString();
            txtSayisi.text = stickmanSayisi.ToString();
           

            yield return null;
        }

        if (DusmanStickmanSayisi == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).rotation = Quaternion.identity;
            }
           


        }
    }
}
