using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance; // Singleton

    // Pool de bolas
    public GameObject ballPrefab;
    private List<GameObject> ballPool = new List<GameObject>();

    // Pool de bonuses
    public Dictionary<BonusType, List<GameObject>> bonusPools = new Dictionary<BonusType, List<GameObject>>();
    public GameObject bigRacketPrefab;
    public GameObject multiBallPrefab;
    public GameObject slowBallPrefab;
    public GameObject extraLifePrefab;
    public GameObject slowRacketPrefab;
    public GameObject fastRacketPrefab;

    public int poolSize = 40; // Tamaño inicial del pool

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        // Pool de bolas
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(ballPrefab);
            obj.SetActive(false);
            ballPool.Add(obj);
        }

        // Pool de bonuses
        foreach (BonusType type in System.Enum.GetValues(typeof(BonusType)))
        {
            bonusPools[type] = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(GetBonusPrefabForType(type));
                obj.SetActive(false);
                bonusPools[type].Add(obj);
            }
        }
    }

    private GameObject GetBonusPrefabForType(BonusType type)
    {
        switch (type)
        {
            case BonusType.BigRacket:
                return bigRacketPrefab;
            case BonusType.MultiBall:
                return multiBallPrefab;
            case BonusType.SlowBall:
                return slowBallPrefab;
            case BonusType.ExtraLife:
                return extraLifePrefab;
            case BonusType.SlowRacket:
                return slowRacketPrefab;
            case BonusType.FastRacket:
                return fastRacketPrefab;
            default:
                Debug.LogError($"Prefab no encontrado para el tipo de bonus: {type}");
                return null;
        }
    }

    public GameObject GetPooledBall()
    {
        foreach (GameObject obj in ballPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }

    public GameObject GetPooledBonus(BonusType type)
    {
        if (bonusPools.ContainsKey(type))
        {
            foreach (GameObject obj in bonusPools[type])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ResetPool()
    {
        foreach (var pool in bonusPools.Values)
        {
            foreach (GameObject obj in pool)
            {
                obj.SetActive(false);
            }
        }

        foreach (GameObject obj in ballPool)
        {
            obj.SetActive(false);
        }
    }
}