using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    public BlackBoard blackBoard = new BlackBoard();
    [SerializeField] private FireBall objectToInstantiate;
    [SerializeField] private HitParticle hitParticle;
    public DashBar dashBar;
    [SerializeField] private Animator sceneTransitioning;
    public bool init = false;

   
    private void Start()
    {
        SceneManager.sceneLoaded += ReadyInsantiations; 
    }

    private void ReadyInsantiations(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex != 0)
        {
            ObjectPooling.SetupPool("FireBall", objectToInstantiate);
            ObjectPooling.SetupPool("HitParticle", hitParticle);
        }

        if(scene.buildIndex == 1)
        {
            blackBoard.GetValue("PlayerController", out PlayerController _controller);
            _controller.transform.position = GameObject.Find("PlayerPosition").transform.position;
            dashBar = GameObject.Find("Canvas").transform.Find("DashBar").GetComponent<DashBar>();
            _controller.playerHealthBar = GameObject.Find("Canvas").transform.Find("Player HealthBar").GetComponent<PlayerHealthBar>();
        }
       
    }

    public void TransitionScenes(int buildIndex)
    {
        StartCoroutine(StartTransitioning(buildIndex));
    }


    private IEnumerator StartTransitioning(int buildIndex)
    {
        sceneTransitioning.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(buildIndex);
        sceneTransitioning.SetTrigger("Start");
        
    }

}
