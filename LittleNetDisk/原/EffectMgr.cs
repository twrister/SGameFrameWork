using System;
using BattleModule.TiledMap.M;
using DynamicAssetsModule.M;
using GLogic.M;
using LightUtility;
using UnityEngine;

namespace RenderModule.M
{
    /// <summary>
    /// 单局特效管理器，管理特效的播放
    /// 它需要能感知到当前正在播放中的特效数量，这样可以动态的设置特效播放质量调节性能
    /// </summary>
    public class EffectMgr : MBLogicNode
    {
        private static EffectMgr s_Instance = null;

        public static EffectMgr Instance => s_Instance;

        private void Awake()
        {
            if(GLog.IsLogInfoEnabled) GLog.LogInfo("EffectMgr.Awake");
            s_Instance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            s_Instance = null;
            if(GLog.IsLogInfoEnabled) GLog.LogInfo("EffectMgr.OnDestroy");
        }

        private bool CheckCanIgnore(GameObject effectPrefab)
        {
            Poolable poolable = effectPrefab.GetComponent<Poolable>();
            return poolable != null && poolable.CanIgnore;
        }

        /// <summary>
        /// 在某位置和方向上播放特效
        /// </summary>
        /// <param name="effectPrefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public Transform Play(GameObject effectPrefab, Vector3 pos, Quaternion rot)
        {
            if (effectPrefab == null) return null;
            //判断视野外不播放
            if (MMapStreamerUtil.CheckPosOutOfView(pos) && CheckCanIgnore(effectPrefab))
            {
                return null;
            }

            GameObject effect = PoolSpawner.Spawn(effectPrefab);
            if (effect != null)
            {
                effect.transform.SetPositionAndRotation(pos, rot);
            }

            return effect.transform;
        }

        /// <summary>
        /// 在某位置和方向上播放特效
        /// </summary>
        /// <param name="effectPrefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public Transform Play(PrefabRef effectPrefab, Vector3 pos, Quaternion rot)
        {
            return effectPrefab == null ? null : Play(effectPrefab.gameObject, pos, rot);
        }

        /// <summary>
        /// 在某个节点下播放特效，会将特效直接放置到parent节点下
        /// </summary>
        /// <param name="effectPrefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Transform Play(GameObject effectPrefab, Transform parent)
        {
            if (effectPrefab == null) return null;
            //判断视野外不播放
            if (MMapStreamerUtil.CheckPosOutOfView(parent.position) && CheckCanIgnore(effectPrefab))
            {
                return null;
            }

            GameObject effect = PoolSpawner.Spawn(effectPrefab, parent);
            return effect == null ? null : effect.transform;
        }

        /// <summary>
        /// 在某个节点下播放特效，会将特效直接放置到parent节点下
        /// </summary>
        /// <param name="effectPrefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public Transform Play(PrefabRef effectPrefab, Transform parent)
        {
            return effectPrefab == null ? null : Play(effectPrefab.gameObject, parent);
        }

        /// <summary>
        /// 强行停止特效，回池或者销毁
        /// </summary>
        /// <param name="effect"></param>
        public void Stop(GameObject effect)
        {
            if (effect != null)
            {
                PoolSpawner.DeSpawn(effect);
            }
        }
    }
}