using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

public class GamePresenter : MonoBehaviour
{
    private static PhysicsMaterial trafficCarMaterial;

    [SerializeField] private GameObject carPrefab;
    [SerializeField] private GameObject[] carPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private TrafficLightPresenter trafficLight;
    [SerializeField] private float initialSpawnDelay = 1f;
    [SerializeField] private float minSpawnInterval = 3f;
    [SerializeField] private float maxSpawnInterval = 8f;
    [SerializeField] private float spawnCheckRadius = 2.5f;
    [SerializeField] private float minCarSpeed = 4f;
    [SerializeField] private float maxCarSpeed = 7f;
    [SerializeField] private LayerMask carLayer = 1 << 3;

    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("GamePresenter: spawnPoint is not assigned.", this);
            enabled = false;
            return;
        }

        if (!HasAnySpawnPrefab())
        {
            Debug.LogError("GamePresenter: no car prefabs assigned for spawning.", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        if (initialSpawnDelay > 0f)
        {
            yield return new WaitForSeconds(initialSpawnDelay);
        }

        while (enabled)
        {
            SpawnCar();

            float nextDelay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(nextDelay);
        }
    }

    private void SpawnCar()
    {
        if (Physics.CheckSphere(spawnPoint.position, spawnCheckRadius, carLayer, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        GameObject prefabToSpawn = GetRandomPrefab();
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("GamePresenter: random prefab selection returned null.", this);
            return;
        }

        var car = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        PrepareSpawnedCar(car);

        if (car.GetComponent<CarPresenter>() == null)
        {
            Debug.LogWarning("GamePresenter: spawned prefab has no CarPresenter.", car);
        }
    }

    private bool HasAnySpawnPrefab()
    {
        if (carPrefabs != null)
        {
            for (int i = 0; i < carPrefabs.Length; i++)
            {
                if (carPrefabs[i] != null)
                {
                    return true;
                }
            }
        }

        return carPrefab != null;
    }

    private GameObject GetRandomPrefab()
    {
        if (carPrefabs != null && carPrefabs.Length > 0)
        {
            int validCount = 0;

            for (int i = 0; i < carPrefabs.Length; i++)
            {
                if (carPrefabs[i] != null)
                {
                    validCount++;
                }
            }

            if (validCount > 0)
            {
                int targetIndex = Random.Range(0, validCount);

                for (int i = 0; i < carPrefabs.Length; i++)
                {
                    if (carPrefabs[i] == null)
                    {
                        continue;
                    }

                    if (targetIndex == 0)
                    {
                        return carPrefabs[i];
                    }

                    targetIndex--;
                }
            }
        }

        return carPrefab;
    }

    private void PrepareSpawnedCar(GameObject car)
    {
        if (car == null)
        {
            return;
        }

        car.layer = LayerMaskToLayerIndex(carLayer, car.layer);
        car.tag = "Car";

        Rigidbody body = car.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = car.AddComponent<Rigidbody>();
        }

        body.useGravity = true;
        body.isKinematic = false;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.linearDamping = 0.5f;
        body.angularDamping = 5f;
        body.mass = 1200f;

        EnsureTrafficCollidersExist(car);

        ApplyTrafficPhysicsMaterial(car);

        if (car.GetComponent<CarView>() == null)
        {
            car.AddComponent<CarView>();
        }

        if (car.GetComponent<CarPresenter>() == null)
        {
            car.AddComponent<CarPresenter>();
        }

        CarView carView = car.GetComponent<CarView>();
        if (carView != null)
        {
            carView.SetSpeed(Random.Range(minCarSpeed, maxCarSpeed));
        }
    }

    private void EnsureTrafficCollidersExist(GameObject car)
    {
        Collider[] existingColliders = car.GetComponentsInChildren<Collider>();
        if (existingColliders.Length > 0)
        {
            return;
        }

        AddFallbackCollider(car);
    }

    private void AddFallbackCollider(GameObject car)
    {
        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        BoxCollider collider = car.AddComponent<BoxCollider>();
        Vector3 lossyScale = car.transform.lossyScale;
        collider.center = car.transform.InverseTransformPoint(bounds.center);
        collider.size = new Vector3(
            SafeDivide(bounds.size.x, lossyScale.x),
            SafeDivide(bounds.size.y, lossyScale.y),
            SafeDivide(bounds.size.z, lossyScale.z));
    }

    private void ApplyTrafficPhysicsMaterial(GameObject car)
    {
        if (trafficCarMaterial == null)
        {
            trafficCarMaterial = new PhysicsMaterial("TrafficCarMaterial");
            trafficCarMaterial.dynamicFriction = 0.05f;
            trafficCarMaterial.staticFriction = 0.05f;
            trafficCarMaterial.bounciness = 0f;
            trafficCarMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            trafficCarMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }

        Collider[] colliders = car.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].material = trafficCarMaterial;
        }
    }

    private int LayerMaskToLayerIndex(LayerMask mask, int fallbackLayer)
    {
        int bits = mask.value;
        if (bits == 0)
        {
            return fallbackLayer;
        }

        for (int i = 0; i < 32; i++)
        {
            if ((bits & (1 << i)) != 0)
            {
                return i;
            }
        }

        return fallbackLayer;
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f))
        {
            return value;
        }

        return value / divisor;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        List<GameObject> foundPrefabs = new List<GameObject>();

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab == null)
            {
                continue;
            }

            foundPrefabs.Add(prefab);
        }

        foundPrefabs.Sort((left, right) => string.Compare(left.name, right.name, System.StringComparison.Ordinal));
        carPrefabs = foundPrefabs.ToArray();

        if ((carPrefab == null || System.Array.IndexOf(carPrefabs, carPrefab) < 0) && carPrefabs.Length > 0)
        {
            carPrefab = carPrefabs[0];
        }
    }
#endif
}
