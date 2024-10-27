using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GenerateLevels : MonoBehaviour
{
    int currentLevelIndex = 0;
    [SerializeField] private List<Transform> objectsToBeGenerated;
    [SerializeField] private Transform player;
    [SerializeField] private float distanceForCreation;
    [SerializeField] private float distanceForDeletion;
    private List<Transform> generatedLevels = new List<Transform>();
    private Transform previousObject;
    private Transform start;
    private Transform end;


    // Start is called before the first frame update
    void Start()
    {
        start = objectsToBeGenerated[currentLevelIndex].Find("Start");
        end = objectsToBeGenerated[currentLevelIndex].Find("End");
        previousObject = Instantiate(objectsToBeGenerated[currentLevelIndex], objectsToBeGenerated[currentLevelIndex].position, Quaternion.identity,this.transform);
        generatedLevels.Add(previousObject);
        currentLevelIndex++;
        
    }

    void Update()
    {
        DeleteGeneratedLevels();
        
        if (Vector3.Distance(previousObject.position,player.position) < distanceForCreation)
        {
            GenerateLevel();
        }
        
    }

    private void DeleteGeneratedLevels()
    {
       
        foreach(Transform e in generatedLevels.ToList())
        {
           
            if (Vector3.Distance(player.position,e.position) > distanceForDeletion && e.position.x < player.position.x)
            {
                Destroy(e.gameObject);
                generatedLevels.Remove(e); 
            }
        }

    }

    private void GenerateLevel()
    {
        
        Transform newObject = Instantiate(objectsToBeGenerated[currentLevelIndex], end.position + (previousObject.position - start.position), Quaternion.identity, this.transform);
        generatedLevels.Add(newObject);
        previousObject = newObject;
        start.position = newObject.Find("Start").position;
        end.position = newObject.Find("End").position;
        currentLevelIndex++;
        Debug.Log(currentLevelIndex);
        if(objectsToBeGenerated.Count <= currentLevelIndex)
            currentLevelIndex = 0;
    }
}
