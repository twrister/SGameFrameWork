using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SthGame
{
    public interface IRedPointDelegate
    {
        void OnTipsChanged(ERedPointType redPointType, int number);
    }

    class RedPointDelegateInfo
    {
        public ERedPointType redPointType = ERedPointType.None;
        public List<IRedPointDelegate> delegates = new List<IRedPointDelegate>();

        public bool OnRecycle()
        {
            redPointType = ERedPointType.None;
            delegates.Clear();
            return true;
        }
    }

    public class RedPointManager : ClientSystem
    {
        public static RedPointManager Instance { get; private set; }

        Dictionary<ERedPointType, RedPointDelegateInfo> redPointDelegateDict = new Dictionary<ERedPointType, RedPointDelegateInfo>();

        RedPointDataStore redPointDS;

        public override void Init()
        {
            Instance = this;

            redPointDS = DataStoreManager.Instance.FindOrBindDataStore<RedPointDataStore>();
        }

        public bool RegisterRedPointDelegate(ERedPointType redPointType, IRedPointDelegate callback)
        {
            if (redPointType == ERedPointType.None || callback == null)
            {
                return false;
            }

            bool alreadyRegister = false;
            RedPointDelegateInfo delegateInfo = GetRedPointDelegate(redPointType);
            if (delegateInfo != null)
            {
                for (int i = 0; i < delegateInfo.delegates.Count; i++)
                {
                    if (delegateInfo.delegates[i] == callback)
                    {
                        alreadyRegister = true;
                        break;
                    }
                }
            }

            if (!alreadyRegister)
            {
                if (delegateInfo == null)
                {
                    delegateInfo = Pool<RedPointDelegateInfo>.Get();
                    delegateInfo.redPointType = redPointType;
                    redPointDelegateDict[redPointType] = delegateInfo;
                }
                delegateInfo.delegates.Add(callback);
            }

            return true;
        }

        public bool UnRegisterRedPointDelegate(ERedPointType redPointType, IRedPointDelegate callback)
        {
            if (redPointType == ERedPointType.None || callback == null)
            {
                return false;
            }

            bool result = false;

            RedPointDelegateInfo delegateInfo = GetRedPointDelegate(redPointType);
            if (delegateInfo != null)
            {
                result = delegateInfo.delegates.Remove(callback);
                if (delegateInfo.delegates.Count == 0)
                {
                    UnRegisterTipsDelegate(redPointType);
                }
            }

            return result;
        }

        public bool UnRegisterTipsDelegate(ERedPointType redPointType)
        {
            if (redPointType == ERedPointType.None) return false;

            bool result = false;

            RedPointDelegateInfo delegateInfo = GetRedPointDelegate(redPointType);
            if (delegateInfo != null)
            {
                result = redPointDelegateDict.Remove(redPointType);
                delegateInfo.OnRecycle();
                Pool<RedPointDelegateInfo>.Release(delegateInfo);
            }

            return result;
        }

        private RedPointDelegateInfo GetRedPointDelegate(ERedPointType redPointType)
        {
            if (redPointDelegateDict.ContainsKey(redPointType))
            {
                return redPointDelegateDict[redPointType];
            }
            return null;
        }

        public void SetRedPointNum(ERedPointType redPointType, int num)
        {
            redPointDS.SetRedPointNum(redPointType, num);
            SendRedPointNotify(redPointType);
        }

        public int GetRedPointNum(ERedPointType redPointType)
        {
            int result = 0;
            List<ERedPointType> subTypes = RedPointParentData.GetRedPointSubTypes(redPointType);
            if (subTypes == null || subTypes.Count == 0)
            {
                result = redPointDS.GetRedPointNum(redPointType);
            }
            else
            {
                result += redPointDS.GetRedPointNum(redPointType);
                for (int i = 0; i < subTypes.Count; i++)
                {
                    result += GetRedPointNum(subTypes[i]);
                }
            }

            return result;
        }

        public void SendRedPointNotify(ERedPointType redPointType)
        {
            RedPointDelegateInfo delegateInfo = GetRedPointDelegate(redPointType);
            if (delegateInfo != null)
            {
                int num = GetRedPointNum(redPointType);
                for (int i = 0; i < delegateInfo.delegates.Count; i++)
                {
                    if (delegateInfo.delegates[i] != null)
                    {
                        delegateInfo.delegates[i].OnTipsChanged(redPointType, num);
                    }
                }
            }

            ERedPointType parentType = RedPointParentData.GetRedPointParentType(redPointType);
            if (parentType != ERedPointType.None)
            {
                SendRedPointNotify(parentType);
            }
        }
    }
}
