using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SthGame
{
    public class RedPointDataStore : DataStore
    {
        Dictionary<ERedPointType, int> redPointDict = new Dictionary<ERedPointType, int>();

        public RedPointDataStore()
            : base(DataStoreType.RedPoint)
        {
        }

        public int GetRedPointNum(ERedPointType redPointType)
        {
            int result = 0;
            redPointDict.TryGetValue(redPointType, out result);
            return result;
        }

        public void SetRedPointNum(ERedPointType redPointType, int num)
        {
            redPointDict[redPointType] = num;
        }

        public override void ReLogin()
        {
            base.ReLogin();

            redPointDict.Clear();
        }
    }
}
