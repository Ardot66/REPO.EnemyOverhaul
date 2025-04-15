using System.Collections.Generic;
using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;
using UnityEngine.AI;

namespace Ardot.REPO.REPOverhaul;

public static class Utils
{
    private record struct FieldKey(Type Type, string Field);

    private static Dictionary<FieldKey, FieldInfo> Fields = new ();
    private static Dictionary<MonoBehaviour, List<object>> Metadata = new ();

    public static object Get<O>(this O obj, string field)
    {
        return GetField<O>(field).GetValue(obj);
    }

    public static T Get<T, O>(this O obj, string field)
    {
        return (T)Get(obj, field);
    }

    public static void Set<T>(this T obj, string field, object value)
    {
        GetField<T>(field).SetValue(obj, value);
    }

    public static FieldInfo GetField<T>(string field)
    {
        FieldKey fieldKey = new (typeof(T), field);

        if(!Fields.TryGetValue(fieldKey, out FieldInfo fieldInfo))
        {
            fieldInfo = AccessTools.Field(typeof(T), field);
            Fields.Add(fieldKey, fieldInfo);
        }

        return fieldInfo;
    }

    public static void SetMetadata(this MonoBehaviour component, int index, object obj)
    {
        if(!Metadata.TryGetValue(component, out List<object> metadata))
        {
            metadata = new List<object> ();
            Metadata.Add(component, metadata);
        }

        while(metadata.Count <= index)
            metadata.Add(default);

        metadata[index] = obj;
    }

    public static T GetMetadata<T>(this MonoBehaviour component, int index, T defaultValue = default)
    {
        if(!Metadata.TryGetValue(component, out List<object> metadata) || metadata.Count <= index || metadata[index] == default)
            return defaultValue;

        return (T)metadata[index];
    }

    public static void CleanMetadata()
    {
        foreach(MonoBehaviour component in Metadata.Keys)
            if(component == null)
                Metadata.Remove(component);
    }

    public static bool FindNavPosition(Vector3 checkPosition, out Vector3 navPosition, float maxDistance = 5f, float maxGroundDistance = 5f)
    {
        NavMeshHit navHit;
        bool positionValid = 
            NavMesh.SamplePosition(checkPosition, out navHit, maxDistance, -1) && 
            Physics.Raycast(navHit.position, Vector3.down, maxGroundDistance, LayerMask.GetMask("Default"));

        navPosition = navHit.position;
        return positionValid;
    } 

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

    public static void ForObjectsInTree(Transform root, Action<Transform, int> action, int maxDepth = -1, IEnumerable<MonoBehaviour> forceInclude = null)
    {
        Stack<ValueTuple<Transform, int>> callStack = new ();
        Recurse(root, 0);

        while(callStack.Count > 0)
        {
            ValueTuple<Transform, int> callData = callStack.Pop();
            action(callData.Item1, callData.Item2);
        }

        bool Recurse(Transform transform, int depth)
        {
            bool forceIncluded = false;

            if(forceInclude != null)
            {
                foreach(MonoBehaviour component in forceInclude)
                {
                    if(transform.GetComponent(component.GetType()) == null)
                        continue;

                    forceIncluded = true;
                }
            }

            for(int x = transform.childCount - 1; x >= 0; x --)
                forceIncluded |= Recurse(transform.GetChild(x), depth + 1);

            if(depth < maxDepth || maxDepth == -1 || forceIncluded)
                callStack.Push((transform, depth));

            return forceIncluded;
        }
    }

    public static List<HurtCollider> GetHurtColliders(Transform root)
    {
        List<HurtCollider> hurtColliders = new ();
        
        ForObjectsInTree(root, (Transform branch, int depth) => {
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

    public static float WeightedDistance(Vector3 vectorA, Vector3 vectorB, float x = 1, float y = 1, float z = 1)
    {
        Vector3 difference = Vector3.Scale(vectorA - vectorB, new Vector3(x, y, z));
        return Vector3.Magnitude(difference);
    }

    public static bool IsHost()
    {
        return SemiFunc.IsMasterClientOrSingleplayer();
    }

    public static LevelPoint ChooseLevelPoint(Vector3 position, float idealRadius, float truckDistanceMultiplier = 0)
    {
        List<LevelPoint> levelPoints = LevelGenerator.Instance.LevelPathPoints;
        float[] levelPointScores = new float [levelPoints.Count];
        float totalScore = 0;

        for(int x = 0; x < levelPoints.Count; x++)
        {
            LevelPoint levelPoint = levelPoints[x];
            float distance = Mathf.Abs(idealRadius - Vector2.Distance(new Vector2(levelPoint.transform.position.x, levelPoint.transform.position.z), new Vector2(position.x, position.z)));
            
            float score = Mathf.Min(distance < 5 ? 1 : 1 / (distance * distance), 1);
            score += score * Vector3.Distance(LevelGenerator.Instance.LevelParent.transform.position, levelPoint.transform.position) * truckDistanceMultiplier;
            levelPointScores[x] = score;
            totalScore += score;
        }

        float chosen = UnityEngine.Random.Range(0, totalScore);
        LevelPoint destination = levelPoints[levelPoints.Count - 1];
        for(int x = 0; x < levelPoints.Count; x++)
        {
            chosen -= levelPointScores[x];
            if(chosen <= 0)
            {
                destination = levelPoints[x];
                break;
            }
        }

        return destination;
    }
}
