using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterLevel : MonoBehaviour
{
    MusicManager musicManaging;
    SpecialObjectives spObjectives;
    public GameObject musicManagerObj;
    public int currentLevel;
    public int endLevel;
    public bool hasPlayedMusic;
    [Header("Objectives")]
    public Text objectiveText;
    public Text objectiveLevelText;
    public Vector3 objectiveOgVec,objectiveNewVec;
    [Header("Spawning")]
    public float currentCooldown;
    //0 = current existing enemies in level, 1 = num enemies produced all time, 2 = num of enemies killed
    public int currentNumEnemies,numSpawnedEnemies,currentKillCount;
    public int randomLocIndex,randomSpawnIndex;
    [System.Serializable]
    public struct levelEnemy
    {
        public string objectiveDesp;
        public Transform[] spawnLocs;
        public int numEnemyCap,numEnemiesToBeat;
        public GameObject[] enemyPrefabs;
        public float minCooldown,maxCooldown;
        public bool specialCondition,showEnemyCounter,isChangeMusic;
        public int changeMusicIndex;
    }
    public levelEnemy[] levelEnemies;
    public bool isFirstSpMissionComplete;

    void Awake()
    {
        musicManaging = GameObject.FindObjectOfType<MusicManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        objectiveOgVec = objectiveText.transform.parent.localPosition;
        spObjectives = GameObject.FindGameObjectWithTag("SpecialObjectives").GetComponent<SpecialObjectives>();
    }

    // Update is called once per frame
    void Update()
    {
        IfLevelIsComplete();
        objectiveText.transform.parent.localPosition = Vector3.Lerp(objectiveText.transform.parent.localPosition,objectiveOgVec,Time.deltaTime*4);
        //spObjectives.enabled = levelEnemies[currentLevel].specialCondition;
        int showCurrentLevel = currentLevel + 1;
        objectiveLevelText.text = "#" + showCurrentLevel.ToString();
        //check if the mission needs enemy counter
        if(levelEnemies[currentLevel].showEnemyCounter && !levelEnemies[currentLevel].specialCondition)
        {
            objectiveText.text = "Eliminate " + currentKillCount.ToString() + "/" + levelEnemies[currentLevel].numEnemiesToBeat.ToString() + " enemies.";
        }
        else
        {
            objectiveText.text = levelEnemies[currentLevel].objectiveDesp;
        }

        if(levelEnemies[currentLevel].isChangeMusic && !hasPlayedMusic)
        {
            hasPlayedMusic = true;
            musicManaging.PlayNewMusic(levelEnemies[currentLevel].changeMusicIndex);
        }
        
        //checks if cooldown to produce
        //check if there is a special condition
        //check if produce do not exceed cap
        //check if the enemies do not exceed current wave enemies amount
        if(currentCooldown <= 0 && !levelEnemies[currentLevel].specialCondition && currentNumEnemies < levelEnemies[currentLevel].numEnemyCap && numSpawnedEnemies < levelEnemies[currentLevel].numEnemiesToBeat)
        {
            randomLocIndex = Random.Range(0,levelEnemies[currentLevel].spawnLocs.Length);
            randomSpawnIndex = Random.Range(0,levelEnemies[currentLevel].enemyPrefabs.Length);
            currentCooldown = Random.Range(levelEnemies[currentLevel].minCooldown,levelEnemies[currentLevel].maxCooldown);
            Instantiate(levelEnemies[currentLevel].enemyPrefabs[randomSpawnIndex],levelEnemies[currentLevel].spawnLocs[randomLocIndex].position,levelEnemies[currentLevel].spawnLocs[randomLocIndex].rotation);
            currentNumEnemies++;
            numSpawnedEnemies++;
        }
        else
        {
            currentCooldown = currentCooldown - Time.deltaTime;
        }
    }

    public void SpecialLevelComplete()
    {
        objectiveText.transform.parent.localPosition = objectiveNewVec+objectiveOgVec;
        currentNumEnemies = 0;
        numSpawnedEnemies = 0;
        currentKillCount = 0;
        currentCooldown = 0;
        hasPlayedMusic = false;
        if(levelEnemies[currentLevel+1].specialCondition)
        {
            if(isFirstSpMissionComplete)
            {
                spObjectives.levelIndex++;
            }
            isFirstSpMissionComplete = true;
        }

        //isFirstSpMissionComplete = true;
        currentLevel++;
    }

    void IfLevelIsComplete()
    {
        if(!levelEnemies[currentLevel].specialCondition && currentKillCount >= levelEnemies[currentLevel].numEnemiesToBeat)
        {
            SpecialLevelComplete();
        }
    }
}
