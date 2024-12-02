using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossYLevelController : MonoBehaviour
{
    [SerializeField] private Transform centerPoint;
    [SerializeField] private List<Transform> middlePlatforms;
    [SerializeField] private List<Transform> sidePlatforms;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private List<Transform> vulnerablePoints;
    [SerializeField] private BossY bossY;
    [SerializeField] private Transform topDeadZone;
    public bool platformsReady = false;
    public bool isInReadySituation = false;

    public void SetPlatforms()
    {
        platformsReady = !platformsReady;
        isInReadySituation = true;
        if (platformsReady)
        {   
            topDeadZone.GetComponent<Collider2D>().enabled = false;
        }
        else
        {   
            topDeadZone.GetComponent<Collider2D>().enabled = true;
        }

        SetMiddlePlatforms();
        SetSidePlatforms();
        GenerateOrResetVulnerablePoint();

    }
    private void SetMiddlePlatforms()
    {
        foreach(var platform in middlePlatforms)
        {
            SpriteRenderer platformRenderer = platform.GetComponent<SpriteRenderer>();
            SpriteRenderer arrowRenderer = platform.GetComponent<SpriteRenderer>();
            if (platform.GetComponentInChildren<VulnerablePoint>() != null)
            {
                Debug.Log("Heeey");
                arrowRenderer = platform.transform.Find("Arrow").GetComponent<SpriteRenderer>();
            }  
           
            StartCoroutine(ChangeOpacity(platformRenderer,arrowRenderer,platform));

        }
    }

    private void SetSidePlatforms()
    {
        foreach(var platform in sidePlatforms)
        {
            platform.GetComponent<Collider2D>().enabled = !platform.GetComponent<Collider2D>().enabled;
            StartCoroutine(MovePlatforms(platform));
    
        }

    }

    private IEnumerator ChangeOpacity(SpriteRenderer platformRenderer,SpriteRenderer arrowRenderer, Transform platform)
    {
        Color platformColor = platformRenderer.color;
        Color arrowColor = arrowRenderer.color;

        if (platformsReady)
            platform.gameObject.SetActive(true);

        for (float i = 0; i < 1f; i += 0.1f)
        {   
            
            if (platformsReady)
            {
                platformColor.a = arrowColor.a = i;
                platform.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                platformColor.a = arrowColor.a = 1 - i;
                platform.gameObject.layer = 0;
                platform.GetComponent<Collider2D>().enabled = false;

            }
            platformRenderer.color = platformColor;
            arrowRenderer.color = arrowColor;
            yield return new WaitForSeconds(0.1f);
        }
        if (platformsReady)
        {
            platformColor.a = 1f;
            arrowColor.a = 1f;
            platform.gameObject.layer = LayerMask.NameToLayer("Ground");
            platform.GetComponent<Collider2D>().enabled = true;
        }

        else
        {
            platformColor.a = 0f;
            arrowColor.a = 0f;
            platform.gameObject.SetActive(false);

        }
            
        isInReadySituation = false;
    }

    private IEnumerator MovePlatforms(Transform platform)
    {
        float duration = 1f; 
        float elapsedTime = 0f;

        Vector2 startPos = platform.transform.position;
        Vector2 endPos;

        if (platformsReady)
        {
        
            endPos = platform.transform.position.x < centerPoint.transform.position.x
            ? new Vector2(platform.transform.position.x + 3, platform.transform.position.y)
            : new Vector2(platform.transform.position.x - 3, platform.transform.position.y);
            platform.gameObject.layer = LayerMask.NameToLayer("Ground");
        }
        else
        {
            endPos = platform.transform.position.x < centerPoint.transform.position.x ?
                new Vector2(platform.transform.position.x - 3, platform.transform.position.y) :
                new Vector2(platform.transform.position.x + 3, platform.transform.position.y);
            platform.gameObject.layer = 0;
        }

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; 
            platform.transform.position = Vector2.Lerp(startPos, endPos, t);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        
        platform.transform.position = endPos;
    }

    private void GenerateOrResetVulnerablePoint()
    {
        
        if (platformsReady)
        {
            int random = Random.Range(0, vulnerablePoints.Count);
            VulnerablePoint point = vulnerablePoints[random].GetComponentInChildren<VulnerablePoint>();
            point.isVulnerable = true;
           
        }
        else
        {
            foreach(var platform in vulnerablePoints)
            {
                platform.GetComponentInChildren<VulnerablePoint>().isVulnerable = false;
            }
 
        }   
       
    }
    
}
