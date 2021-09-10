using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SthGame
{

    public sealed class ObjectPool : MonoBehaviour
    {
        public enum StartupPoolMode { Awake, Start, CallManually };

        [System.Serializable]
        public class StartupPool
        {
            public int size;
            public GameObject prefab;
        }

        static ObjectPool _instance;
        static List<GameObject> tempList = new List<GameObject>();

        Dictionary<GameObject, Stack<GameObject>> pooledObjects = new Dictionary<GameObject, Stack<GameObject>>();
        Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

        public StartupPoolMode startupPoolMode;
        public StartupPool[] startupPools;

        bool startupPoolsCreated;

        void Awake()
        {
            _instance = this;
            if (startupPoolMode == StartupPoolMode.Awake)
                CreateStartupPools();
        }

        void Start()
        {
            if (startupPoolMode == StartupPoolMode.Start)
                CreateStartupPools();
        }

        public static void CreateStartupPools()
        {
            if (!instance.startupPoolsCreated)
            {
                instance.startupPoolsCreated = true;
                var pools = instance.startupPools;
                if (pools != null && pools.Length > 0)
                    for (int i = 0; i < pools.Length; ++i)
                        CreatePool(pools[i].prefab, pools[i].size);
            }
        }

        public static void CreatePool<T>(T prefab, int initialPoolSize) where T : Component
        {
            CreatePool(prefab.gameObject, initialPoolSize);
        }
        public static void CreatePool(GameObject prefab, int initialPoolSize)
        {
            if (prefab != null && !instance.pooledObjects.ContainsKey(prefab))
            {
                var stack = new Stack<GameObject>();
                instance.pooledObjects.Add(prefab, stack);

                if (initialPoolSize > 0)
                {
                    bool active = prefab.activeSelf;
                    prefab.SetActive(false);
                    Transform parent = instance.transform;
                    while (stack.Count < initialPoolSize)
                    {
                        var obj = (GameObject)Object.Instantiate(prefab);
                        obj.transform.SetParent(parent);
                        stack.Push(obj);
                    }
                    prefab.SetActive(active);
                }
            }
        }

        public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Vector3 position) where T : Component
        {
            return Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab, Transform parent) where T : Component
        {
            return Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }
        public static T Spawn<T>(T prefab) where T : Component
        {
            return Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
        }
        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            Stack<GameObject> stack;
            Transform trans;
            GameObject obj;
            if (instance.pooledObjects.TryGetValue(prefab, out stack))
            {
                obj = null;
                if (stack.Count > 0)
                {
                    while (obj == null && stack.Count > 0)
                    {
                        obj = stack.Pop();
                    }
                    if (obj != null)
                    {
                        trans = obj.transform;
                        trans.SetParent(parent);
                        trans.localPosition = position;
                        trans.localRotation = rotation;
                        obj.SetActive(true);
                        instance.spawnedObjects.Add(obj, prefab);
                        return obj;
                    }
                }
                obj = (GameObject)Object.Instantiate(prefab);
                trans = obj.transform;
                trans.SetParent(parent);
                trans.localPosition = position;
                trans.localRotation = rotation;
                instance.spawnedObjects.Add(obj, prefab);
                return obj;
            }
            else
            {
                obj = (GameObject)Object.Instantiate(prefab);
                trans = obj.GetComponent<Transform>();
                trans.SetParent(parent);
                trans.localPosition = position;
                trans.localRotation = rotation;
                return obj;
            }
        }
        public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
        {
            return Spawn(prefab, parent, position, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Spawn(prefab, null, position, rotation);
        }
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            return Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, null, position, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, null, Vector3.zero, Quaternion.identity);
        }

        public static void Recycle<T>(T obj) where T : Component
        {
            Recycle(obj.gameObject);
        }
        public static void Recycle(GameObject obj)
        {
            GameObject prefab;
            if (instance.spawnedObjects.TryGetValue(obj, out prefab))
                Recycle(obj, prefab);
            else
                Object.Destroy(obj);
        }
        static void Recycle(GameObject obj, GameObject prefab)
        {
            instance.pooledObjects[prefab].Push(obj);
            instance.spawnedObjects.Remove(obj);
            //obj.transform.parent = instance.transform;
            obj.transform.SetParent(instance.transform);
            obj.SetActive(false);
        }

        public static void RecycleAll<T>(T prefab) where T : Component
        {
            RecycleAll(prefab.gameObject);
        }
        public static void RecycleAll(GameObject prefab)
        {
            foreach (var item in instance.spawnedObjects)
                if (item.Value == prefab)
                    tempList.Add(item.Key);
            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);
            tempList.Clear();
        }
        public static void RecycleAll()
        {
            tempList.AddRange(instance.spawnedObjects.Keys);
            for (int i = 0; i < tempList.Count; ++i)
                Recycle(tempList[i]);
            tempList.Clear();
        }

        public static bool IsSpawned(GameObject obj)
        {
            return instance.spawnedObjects.ContainsKey(obj);
        }

        public static int CountPooled<T>(T prefab) where T : Component
        {
            return CountPooled(prefab.gameObject);
        }
        public static int CountPooled(GameObject prefab)
        {
            Stack<GameObject> stack;
            if (instance.pooledObjects.TryGetValue(prefab, out stack))
                return stack.Count;
            return 0;
        }

        public static int CountSpawned<T>(T prefab) where T : Component
        {
            return CountSpawned(prefab.gameObject);
        }
        public static int CountSpawned(GameObject prefab)
        {
            int count = 0;
            foreach (var instancePrefab in instance.spawnedObjects.Values)
                if (prefab == instancePrefab)
                    ++count;
            return count;
        }

        public static int CountAllPooled()
        {
            int count = 0;
            foreach (var list in instance.pooledObjects.Values)
                count += list.Count;
            return count;
        }

        public static int CountAllSpawned()
        {
            return instance.spawnedObjects.Count;
        }

        public static void DebugPoolInfo()
        {
            Logger.Log("total spawned count = {0}", CountAllPooled());
            foreach (var item in instance.pooledObjects)
            {
                Logger.Log("pool {0}'s count = {1} ", 
                    item.Key == null ? "null" : item.Key.name,
                    item.Value == null ? "null" : item.Value.Count.ToString());
            }
        }

        public static Stack<GameObject> GetPooled(GameObject prefab, Stack<GameObject> stack, bool appendStack)
        {
            if (stack == null)
                stack = new Stack<GameObject>();
            if (!appendStack)
                stack.Clear();
            Stack<GameObject> pooled;
            if (instance.pooledObjects.TryGetValue(prefab, out pooled))
            {
                while (pooled.Count > 0)
                {
                    stack.Push(pooled.Pop());
                }
            }
            return stack;
        }
        public static Stack<T> GetPooled<T>(T prefab, Stack<T> stack, bool appendStack) where T : Component
        {
            if (stack == null)
                stack = new Stack<T>();
            if (!appendStack)
                stack.Clear();
            Stack<GameObject> pooled;
            if (instance.pooledObjects.TryGetValue(prefab.gameObject, out pooled))
            {
                while (pooled.Count > 0)
                {
                    stack.Push(pooled.Pop().GetComponent<T>());
                }

            }
            return stack;
        }

        public static Stack<GameObject> GetSpawned(GameObject prefab, Stack<GameObject> stack, bool appendStack)
        {
            if (stack == null)
                stack = new Stack<GameObject>();
            if (!appendStack)
                stack.Clear();
            foreach (var item in instance.spawnedObjects)
                if (item.Value == prefab)
                    stack.Push(item.Key);
            return stack;
        }
        public static Stack<T> GetSpawned<T>(T prefab, Stack<T> stack, bool appendStack) where T : Component
        {
            if (stack == null)
                stack = new Stack<T>();
            if (!appendStack)
                stack.Clear();
            var prefabObj = prefab.gameObject;
            foreach (var item in instance.spawnedObjects)
                if (item.Value == prefabObj)
                    stack.Push(item.Key.GetComponent<T>());
            return stack;
        }

        public static void DestroyPooled(GameObject prefab)
        {
            Stack<GameObject> pooled;
            if (instance.pooledObjects.TryGetValue(prefab, out pooled))
            {
                while (pooled.Count > 0)
                    GameObject.Destroy(pooled.Pop());
                pooled.Clear();
                instance.pooledObjects.Remove(prefab);
            }
        }
        public static void DestroyPooled<T>(T prefab) where T : Component
        {
            DestroyPooled(prefab.gameObject);
        }

        public static void DestroyAll(GameObject prefab)
        {
            RecycleAll(prefab);
            DestroyPooled(prefab);
        }
        public static void DestroyAll<T>(T prefab) where T : Component
        {
            DestroyAll(prefab.gameObject);
        }

        public static ObjectPool instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = Object.FindObjectOfType<ObjectPool>();
                if (_instance != null)
                    return _instance;

                var obj = new GameObject("ObjectPool");
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
                _instance = obj.AddComponent<ObjectPool>();
                return _instance;
            }
        }
    }

    public static class ObjectPoolExtensions
    {
        public static void CreatePool<T>(this T prefab) where T : Component
        {
            ObjectPool.CreatePool(prefab, 0);
        }
        public static void CreatePool<T>(this T prefab, int initialPoolSize) where T : Component
        {
            ObjectPool.CreatePool(prefab, initialPoolSize);
        }
        public static void CreatePool(this GameObject prefab)
        {
            ObjectPool.CreatePool(prefab, 0);
        }
        public static void CreatePool(this GameObject prefab, int initialPoolSize)
        {
            ObjectPool.CreatePool(prefab, initialPoolSize);
        }

        public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
        {
            return ObjectPool.Spawn(prefab, parent, position, rotation);
        }
        public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return ObjectPool.Spawn(prefab, null, position, rotation);
        }
        public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component
        {
            return ObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
        }
        public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
        {
            return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
        }
        public static T Spawn<T>(this T prefab, Transform parent) where T : Component
        {
            return ObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }
        public static T Spawn<T>(this T prefab) where T : Component
        {
            return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
        {
            return ObjectPool.Spawn(prefab, parent, position, rotation);
        }
        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return ObjectPool.Spawn(prefab, null, position, rotation);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
        {
            return ObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
        }
        public static GameObject Spawn(this GameObject prefab, Vector3 position)
        {
            return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
        }
        public static GameObject Spawn(this GameObject prefab, Transform parent)
        {
            return ObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(this GameObject prefab)
        {
            return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
        }

        public static void Recycle<T>(this T obj) where T : Component
        {
            ObjectPool.Recycle(obj);
        }
        public static void Recycle(this GameObject obj)
        {
            ObjectPool.Recycle(obj);
        }

        public static void RecycleAll<T>(this T prefab) where T : Component
        {
            ObjectPool.RecycleAll(prefab);
        }
        public static void RecycleAll(this GameObject prefab)
        {
            ObjectPool.RecycleAll(prefab);
        }

        public static int CountPooled<T>(this T prefab) where T : Component
        {
            return ObjectPool.CountPooled(prefab);
        }
        public static int CountPooled(this GameObject prefab)
        {
            return ObjectPool.CountPooled(prefab);
        }

        public static int CountSpawned<T>(this T prefab) where T : Component
        {
            return ObjectPool.CountSpawned(prefab);
        }
        public static int CountSpawned(this GameObject prefab)
        {
            return ObjectPool.CountSpawned(prefab);
        }

        public static Stack<GameObject> GetSpawned(this GameObject prefab, Stack<GameObject> stack, bool appendStack)
        {
            return ObjectPool.GetSpawned(prefab, stack, appendStack);
        }
        public static Stack<GameObject> GetSpawned(this GameObject prefab, Stack<GameObject> stack)
        {
            return ObjectPool.GetSpawned(prefab, stack, false);
        }
        public static Stack<GameObject> GetSpawned(this GameObject prefab)
        {
            return ObjectPool.GetSpawned(prefab, null, false);
        }
        public static Stack<T> GetSpawned<T>(this T prefab, Stack<T> stack, bool appendStack) where T : Component
        {
            return ObjectPool.GetSpawned(prefab, stack, appendStack);
        }
        public static Stack<T> GetSpawned<T>(this T prefab, Stack<T> stack) where T : Component
        {
            return ObjectPool.GetSpawned(prefab, stack, false);
        }
        public static Stack<T> GetSpawned<T>(this T prefab) where T : Component
        {
            return ObjectPool.GetSpawned(prefab, null, false);
        }

        public static Stack<GameObject> GetPooled(this GameObject prefab, Stack<GameObject> stack, bool appendStack)
        {
            return ObjectPool.GetPooled(prefab, stack, appendStack);
        }
        public static Stack<GameObject> GetPooled(this GameObject prefab, Stack<GameObject> stack)
        {
            return ObjectPool.GetPooled(prefab, stack, false);
        }
        public static Stack<GameObject> GetPooled(this GameObject prefab)
        {
            return ObjectPool.GetPooled(prefab, null, false);
        }
        public static Stack<T> GetPooled<T>(this T prefab, Stack<T> stack, bool appendStack) where T : Component
        {
            return ObjectPool.GetPooled(prefab, stack, appendStack);
        }
        public static Stack<T> GetPooled<T>(this T prefab, Stack<T> stack) where T : Component
        {
            return ObjectPool.GetPooled(prefab, stack, false);
        }
        public static Stack<T> GetPooled<T>(this T prefab) where T : Component
        {
            return ObjectPool.GetPooled(prefab, null, false);
        }

        public static void DestroyPooled(this GameObject prefab)
        {
            ObjectPool.DestroyPooled(prefab);
        }
        public static void DestroyPooled<T>(this T prefab) where T : Component
        {
            ObjectPool.DestroyPooled(prefab.gameObject);
        }

        public static void DestroyAll(this GameObject prefab)
        {
            ObjectPool.DestroyAll(prefab);
        }
        public static void DestroyAll<T>(this T prefab) where T : Component
        {
            ObjectPool.DestroyAll(prefab.gameObject);
        }
    }
}