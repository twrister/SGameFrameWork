using System.Collections;
using System.Collections.Generic;

namespace SthGame
{
    public interface DataStoreSubscriber
    {
        void NotifyDataStoreUpdated(DataStore sourceDataStore, int index);
    }
}
