using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region Pool
    private class Pool
    {
        public GameObject Prefab { get; private set; }
        public Transform Root { get; private set; }
        public int Size { get; private set; }
        public int Max { get; private set; }

        private readonly HashSet<GameObject> _activePool = new();
        private readonly List<GameObject> _deactivePool = new();

        public Pool(GameObject prefab, int size, int max)
        {
            Prefab = prefab;
            Root = new GameObject($"{Prefab.name}_Root").transform;
            Max = max;

            for (int i = 0; i < size; i++)
            {
                var go = Create();
                if (go == null)
                {
                    break;
                }

                PushToDeactiveContainer(go);
            }
        }

        public bool Push(GameObject go)
        {
            if (!_activePool.Remove(go))
            {
                return false;
            }

            PushToDeactiveContainer(go);
            return true;
        }

        public GameObject Pop(Transform parent)
        {
            GameObject go;

            int lastIndex = _deactivePool.Count - 1;
            if (lastIndex >= 0)
            {
                go = _deactivePool[lastIndex];
                _deactivePool.RemoveAt(lastIndex);
            }
            else
            {
                go = Create();
                if (go == null)
                {
                    return null;
                }
            }

            go.SetActive(true);
            go.transform.SetParent(parent == null ? Root : parent);
            _activePool.Add(go);
            return go;
        }

        public void Clear()
        {
            if (Root != null)
            {
                foreach (var go in _activePool)
                {
                    Object.Destroy(go);
                }

                foreach (var go in _deactivePool)
                {
                    Object.Destroy(go);
                }

                Object.Destroy(Root.gameObject);
            }

            _activePool.Clear();
            _deactivePool.Clear();
        }

        private GameObject Create()
        {
            if (Size == Max)
            {
                return null;
            }

            Size++;
            var go = Object.Instantiate(Prefab);
            go.name = Prefab.name;
            return go;
        }

        private void PushToDeactiveContainer(GameObject go)
        {
            go.transform.SetParent(Root);
            go.SetActive(false);
            _deactivePool.Add(go);
        }
    }
    #endregion

    /// <summary>
    /// If a pool does not exist when the Pop method is called, a pool is automatically created.
    /// </summary>
    public static bool AutoCreate { get; set; } = true;

    private readonly Dictionary<string, Pool> _pools = new();
    private Transform _root;

    public void Init()
    {
        _root = Util.FindOrInstantiate("Pool_Root").transform;
    }

    public void CreatePool(GameObject prefab, int size = 1, int max = -1)
    {
        if (prefab == null)
        {
            Debug.Log($"[PoolManager/CreatePool] Prefab is null.");
            return;
        }

        if (_pools.ContainsKey(prefab.name))
        {
            Debug.Log($"[PoolManager/CreatePool] {prefab.name} pool already exist.");
            return;
        }

        var pool = new Pool(prefab, size, max);
        pool.Root.SetParent(_root);
        _pools.Add(prefab.name, pool);
    }

    public bool Push(GameObject go)
    {
        if (go == null)
        {
            Debug.Log($"[PoolManager/Push] GameObject is null.");
            return false;
        }

        if (!_pools.ContainsKey(go.name))
        {
            Debug.Log($"[PoolManager/Push] {go.name} pool no exist.");
            return false;
        }

        _pools[go.name].Push(go);
        return true;
    }

    public GameObject Pop(GameObject prefab, Transform parent = null)
    {
        if (!_pools.TryGetValue(prefab.name, out var pool))
        {
            if (AutoCreate)
            {
                CreatePool(prefab);
                pool = _pools[prefab.name];
            }
            else
            {
                Debug.Log($"[PoolManager/Pop] {prefab.name} pool no exist.");
                return null;
            }
        }

        return pool.Pop(parent);
    }

    public void ClearPool(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Clear();
            _pools.Remove(name);
        }
        else
        {
            Debug.Log($"[PoolManager/ClearPool] {name} pool no exist.");
        }
    }

    public void Clear()
    {
        foreach (var kvp in _pools)
        {
            kvp.Value.Clear();
        }

        _pools.Clear();
    }
}
