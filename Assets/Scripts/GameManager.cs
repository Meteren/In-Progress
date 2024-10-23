using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public BlackBoard blackBoard = new BlackBoard();
    [SerializeField] private FireBall objectToInstantiate;
    [SerializeField] private HitParticle hitParticle;
    public DashBar dashBar;

   
    private void Start()
    {
        ObjectPooling.SetupPool("FireBall",objectToInstantiate);
        ObjectPooling.SetupPool("HitParticle", hitParticle);
     
    }


}
