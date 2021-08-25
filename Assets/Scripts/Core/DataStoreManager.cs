using System.Collections;
using System.Collections.Generic;
using System;

namespace SthGame
{
    public class DataStoreManager : ClientSystem
    {
        public static DataStoreManager Instance { get; private set; }

        private Dictionary<DataStoreType, DataStore> dataStoreDict = new Dictionary<DataStoreType, DataStore>();
        private Dictionary<string, DataStoreType> dataStoreTypeTagDict = new Dictionary<string, DataStoreType>();

        public override void Init()
        {
            Instance = this;
        }

        public override void ShutDown()
        {

        }

        public DataStore FindDataStore(DataStoreType tag)
        {
            DataStore dataStoreResult = null;
            dataStoreDict.TryGetValue(tag, out dataStoreResult);
            return dataStoreResult;
        }

        DataStore BindDataStore(Type dataStoreType)
        {
            DataStore dataStore = CreateDataStore(dataStoreType);
            if (dataStore != null)
            {
                RegisterDataStore(dataStore);
                return dataStore;
            }
            return null;
        }

        public T FindOrBindDataStore<T>() where T : DataStore
        {
            Type type = typeof(T);
            string key = typeof(T).FullName;
            T result = null;
            if (dataStoreTypeTagDict.ContainsKey(key))
            {
                result = FindDataStore(dataStoreTypeTagDict[key]) as T;
            }
            else
            {
                result = BindDataStore(type) as T;
            }

            return result;
        }

        DataStore CreateDataStore(Type dataStoreType)
        {
            DataStore dataStore = Activator.CreateInstance(dataStoreType) as DataStore;
            if (dataStore != null)
            {
                dataStore.InitializeDataStore();
            }
            return dataStore;
        }

        bool RegisterDataStore(DataStore dataStore)
        {
            if (dataStore != null)
            {
                if (FindDataStore(dataStore.Tag) == null)
                {
                    dataStoreDict.Add(dataStore.Tag, dataStore);
                    dataStoreTypeTagDict.Add(dataStore.GetType().FullName, dataStore.Tag);
                    return true;
                }
            }
            return false;
        }

        public override void ReLogin()
        {
            foreach (var item in dataStoreDict)
            {
                item.Value.ReLogin();
            }
        }
    }

    public enum DataStoreType
    {
        UserInfo,
        Chat,
        PacMan,
        RedPoint,
    }
}
