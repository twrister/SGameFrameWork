using System.Collections;
using System.Collections.Generic;
using System;

namespace SthGame
{
    public class DataStore
    {
        List<DataStoreSubscriber> subscriberList = new List<DataStoreSubscriber>();

        private DataStoreType tag;
        public DataStoreType Tag { get { return tag; } }

        public DataStore(DataStoreType inTag)
        {
            tag = inTag;
        }

        public virtual void Reset()
        {
        }

        public virtual void InitializeDataStore()
        {
        }

        public virtual void ReLogin()
        {
            subscriberList.Clear();
        }

        public void RegisterSubscriber(DataStoreSubscriber subscriber)
        {
            if (!subscriberList.Contains(subscriber))
            {
                subscriberList.Add(subscriber);
            }
        }

        public void UnRegisterSubscriber(DataStoreSubscriber Subscriber)
        {
            subscriberList.Remove(Subscriber);
        }

        public virtual void RefreshSubscribers(int index)
        {
            for (int i = 0; i < subscriberList.Count; i++)
            {
                DataStoreSubscriber Subscriber = subscriberList[i];
                if (Subscriber != null)
                {
                    Subscriber.NotifyDataStoreUpdated(this, index);
                }
            }
        }
    }
}
