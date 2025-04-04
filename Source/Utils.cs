using System.Collections.Generic;
using System;
using UnityEngine;

namespace Ardot.REPO.REPOverhaul;

public static class Utils
{
    public static Dictionary<string, GameObject> GetEnemies()
    {
        List<EnemySetup> enemyList = new ();
        enemyList.AddRange(EnemyDirector.instance.enemiesDifficulty1);
        enemyList.AddRange(EnemyDirector.instance.enemiesDifficulty2);
        enemyList.AddRange(EnemyDirector.instance.enemiesDifficulty3);

        Dictionary<string, GameObject> enemyObjects = new ();

        for(int x = 0; x < enemyList.Count; x++)
        {
            List<GameObject> spawnObjects = enemyList[x].spawnObjects;
            for (int y = 0; y < spawnObjects.Count; y++)
            {
                GameObject enemyObject = spawnObjects[y];

                if(enemyObject.GetComponent<EnemyParent>() is not EnemyParent enemyParent)
                    continue;

                string enemyName = enemyParent.enemyName.ToLower().Replace(' ', '-');

                if(enemyObjects.ContainsKey(enemyName))
                    continue;

                enemyObjects.Add(enemyName, enemyObject);
            }
        }

        return enemyObjects;
    }

    public static void ForObjectsInTree(GameObject root, Action<GameObject, int> action)
    {
        Stack<ValueTuple<Transform, int>> branches = new ();
        branches.Push((root.GetComponent<Transform>(), 0));
        
        while(branches.Count > 0)
        {
            ValueTuple<Transform, int> branch = branches.Pop();
            Transform transform = branch.Item1;
            int depth = branch.Item2;

            for(int x = 0; x < transform.childCount; x++)
                branches.Push((transform.GetChild(x), depth + 1));

            action(transform.gameObject, depth);
        }
    }

    public static List<HurtCollider> GetHurtColliders(GameObject root)
    {
        List<HurtCollider> hurtColliders = new ();
        
        ForObjectsInTree(root, (GameObject branch, int depth) => {
            if(branch.GetComponent<HurtCollider>() is HurtCollider hurtCollider)
                hurtColliders.Add(hurtCollider);
        });

        return hurtColliders;
    }

    public static Value Value(float min, float max)
    {
        Value value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = min;
        value.valueMax = max;
        return value;
    }
}
