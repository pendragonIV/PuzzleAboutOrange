using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScene : MonoBehaviour
{
    public static LevelScene instance;

    [SerializeField]
    private Transform levelHolderPrefab;
    [SerializeField]
    private Transform levelsContainer;
    [SerializeField]
    private Transform starPrefab;

    public Transform sceneTransition;

    private const int MAX_STARS = 3;    

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        PrepareLevels();
    }
    public void PlayChangeScene()
    {
        sceneTransition.GetComponent<Animator>().Play("SceneTransitionReverse");
    }

    private void PrepareLevels()
    {
        for (int i = 0; i < LevelManager.instance.levelData.GetLevels().Count; i++)
        {
            Transform holder = Instantiate(levelHolderPrefab, levelsContainer);
            holder.name = i.ToString();
            Level level = LevelManager.instance.levelData.GetLevelAt(i);
            if (LevelManager.instance.levelData.GetLevelAt(i).isPlayable)
            {
                holder.GetComponent<LevelHolder>().EnableHolder();
                SetAchivement(holder, level);
                holder.GetComponent<LevelHolder>().SetHolderText();
            }
            else
            {
                holder.GetComponent<LevelHolder>().DisableHolder();
            }
        }
    }

    private void SetAchivement(Transform holder, Level levelData)
    {
        Transform achivementContainer = holder.GetChild(1);

        for(int i = 0; i < MAX_STARS; i++)
        {
            Transform star = Instantiate(starPrefab, achivementContainer);
            if(i < levelData.achivement)
            {
                star.GetChild(0).gameObject.SetActive(false);
            }   
        }
    }



}
