using UnityEngine;
using UnityEngine.SceneManagement;

public class menuManager : MonoBehaviour
{
  [SerializeField] private GameObject startMenuObj;
  [SerializeField] private GameObject panelOBJ;
  [SerializeField] private GameObject RestartOBj;


    public void StartTheGame()
  {
        panelOBJ.SetActive(false);
        startMenuObj.SetActive(false);
    PlayerManager.PlayerManagerInstance.oyunDurumu = true;
    
    PlayerManager.PlayerManagerInstance.player.GetChild(1).GetComponent<Animator>().SetBool("run",true);
  }


    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
