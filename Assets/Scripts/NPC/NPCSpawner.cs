using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct NPCRule
    {
        public GameObject instance;
        public bool[] criteria;
    }

    [SerializeField] List<StoryEventScriptableObject> storyEvents;
    [SerializeField] List<NPCRule> rules;

    GameObject currentActive;

    void Start()
    {
        ActivationLogic();
    }

    private void ActivationLogic()
    {
        foreach (NPCRule rule in rules)
        {
            if (currentActive != null)
            {
                rule.instance.SetActive(false);
                continue;
            }

            bool valid = true;
            for (int i = 0; i < rule.criteria.Length && i < storyEvents.Count; i++)
            {
                if (rule.criteria[i] != storyEvents[i].state)
                    valid = false;
            }

            if (!valid)
            {
                rule.instance.SetActive(false);
                continue;
            }

            currentActive = rule.instance;
        }

        if (currentActive == null)
        {
            // activate default
        }
        else 
        {
            currentActive.SetActive(true);
        }
    }

    private void OnValidate() 
    {
        if (storyEvents == null || storyEvents.Count < 1 || rules == null)
            return;

        for (int i = 0; i < rules.Count; i++) 
        {
            NPCRule rule = rules[i];
            if (rule.criteria.Length != storyEvents.Count)
                rule.criteria = new bool[storyEvents.Count];
        }
    }
}
