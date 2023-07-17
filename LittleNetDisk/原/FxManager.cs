using System;
using System.Collections.Generic;
using JahroConsole;
using JWwise;
using KH;
using KH.Define;
using Morefun.LockStep;
using Morefun.LockStep.Asset;
using Next.BoyanFX;
using UC.Runtime.Common;
using UC.Runtime.Effect.FxExtension;
using UC.Runtime.Log;
using UC.Runtime.ResourcesManager;
using UC.Runtime.Units;
using UC.Runtime.Units.Render;
using Unity.Mathematics;
using UnityEngine;

namespace UC.Runtime.Effect
{
    
    /// <summary>
    /// 待播放得特效，Spawn的特效先进入队列，在LateUpdate播放，为了保证特效在所有Transform更新之后
    /// </summary>
    struct FxToPlayItem
    {
        public NextBoyanFx fx;
        public bool follow;
        public VActor owner;
        public float terrainY;
        public bool playOnGround;

        public void Play()
        {
            if (fx)
            {
                fx.Play(owner,follow);
            }
        }

        public void UpdatePosition()
        {
            if (fx == null) return;
            if (follow == false)
            {
                fx.transform.SetParent(null);
                if (playOnGround)
                {
                    //贴地处理
                    FScalar terrainHeight = FScalar.zero;
                    var worldPos = fx.transform.position;
#if !CP_PROJECT
                    KH.KHEngine.Current.Scene.Terrain.GetTerrainY(worldPos.ToFVector3(), ref terrainHeight);
#endif
                    worldPos.y = terrainHeight.ToFloat() + terrainY;
                    fx.transform.position = worldPos;
                }
            }else if (playOnGround)
            {
                fx.SetForceGrounded(true,terrainY);
            }
        }
    }

    struct LinkFxReference
    {
        public VActor fromVActor;
        public VActor toVActor;
        public NextBoyanFx fx;

        public bool IsReference(VActor actor)
        {
            return actor == fromVActor || actor == toVActor;
        }

        public bool IsCreator(VActor actor)
        {
            return fx.Creator == actor;
        }
        
    }

    public class FxManager : SingletonMono<FxManager>
    {
        public bool EnableSpawn => m_enableSpawn;

        private Transform m_effectContainer;
        private List<NextBoyanFx> m_boyanFxes;
        private List<FxToPlayItem> m_toPlayList = new List<FxToPlayItem>(32);
        private List<string> m_effectPaths;
        private List<LinkFxReference> m_LinkFxReferences = new List<LinkFxReference>(16);

        private Dictionary<int, Stack<NextBoyanFx>> m_cacheFx;

        //存放 特效和 UnityDisplayObject的关联,业务上需要指定移除
        private Dictionary<NextBoyanFx, VActor> m_fxActorRefrence;
        private int m_curPlayCount;
        private int m_autoIncId;
        private Action<NextBoyanFx> m_completeCallback;
        private Action<NextBoyanFx> m_linkFxCompleteCallback;
        private Action<UCGhostShadowRoot> m_shadowFadeComplete;

        private bool m_enableSpawn = true;

        protected override void InitSingleton()
        {
            GameObject effectContainerGameObject = DontDestroyContainerManager.Instance.GetContainer(GetFxContainer());
            m_effectContainer = effectContainerGameObject.transform;
            //effectContainerGameObject.SetActive(false);
            m_boyanFxes = new List<NextBoyanFx>(256);
            m_effectPaths = new List<string>(256);
            m_cacheFx = new Dictionary<int, Stack<NextBoyanFx>>(256);
            m_fxActorRefrence = new Dictionary<NextBoyanFx, VActor>(256);
            m_completeCallback = OnFxPlayComplete;
            m_linkFxCompleteCallback = OnLinkFxComplete;
            m_shadowFadeComplete = OnShadowFadeComplete;
        }

        [JahroCommand("生成特效", "测试功能，打开和关闭特效生成", "特效")]
        public static void SetEnable(bool flag)
        {
            Instance.m_enableSpawn = flag;
        }

        /// <summary>
        /// 在 指定 VActor 相对位置播放一个特效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="vActor"></param>
        /// <param name="bindPoint"></param>
        /// <param name="bindPath"></param>
        /// <param name="follow"></param>
        /// <param name="block">如果被击方处于格挡</param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public NextBoyanFx SpawnFx(int id, Vector3 pos, Quaternion rotation, Vector3 scale, VActor vActor,
            int bindPoint, string bindPath, bool follow, bool block = false, string tag = null,
            bool needPauseWithCreator = false, bool playOnGround = false)
        {
            Transform bindTarget = UCFxUtils.GetBindNode(vActor, bindPoint, bindPath);

            if (bindTarget == null) bindTarget = vActor.transform;
            var fx = SpawnFx(id, pos, rotation, scale, bindTarget, follow, block, vActor, needPauseWithCreator,
                playOnGround);
            if (fx)
            {
                fx.fxTag = tag;
                if (follow)
                {
                    m_fxActorRefrence.Add(fx, vActor);
                }
            }

            return fx;
        }

        /// <summary>
        /// 创建一个链接效果
        /// </summary>
        /// <returns></returns>
        public NextBoyanFx SpawnLinkFx(int id,VActor caster, VActor fromTarget, int fromPoint, VActor toTarget, int toPoint)
        {
            var casterBind = UCFxUtils.GetBindNode(fromTarget, fromPoint, "");
            var targetBind = UCFxUtils.GetBindNode(toTarget, toPoint, "");
            var fx = SpawnFx(id, Vector3.zero, quaternion.identity, Vector3.one, casterBind,owner:caster);
            if (fx)
            {
                var linkTarget = fx.GetExtension<UCLinkTarget>();
                if (linkTarget)
                {
                    linkTarget.LinkTargets(casterBind, targetBind);
                    //覆盖掉普通特效的回调
                    fx.SetOnFinishAction(m_linkFxCompleteCallback);
                    m_LinkFxReferences.Add(new LinkFxReference()
                    {
                        fx = fx,
                        fromVActor = fromTarget,
                        toVActor = toTarget
                    });
                }
            }

            return fx;
        }
        


        /// <summary>
        /// 暂停特效，一般用于随源得顿帧
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="effectId"></param>
        public void PauseFx(int groupId, int effectId)
        {
            foreach (var boyanFx in m_boyanFxes)
            {
                if (boyanFx.groupId == groupId && boyanFx.id == effectId)
                {
                    boyanFx.Pause();
                    return;
                }
            }
        }


        public NextBoyanFx GetFxByFlag(string flagid, int effectId)
        {
            foreach (var boyanFx in m_boyanFxes)
            {
                if (boyanFx.fxTag == flagid && boyanFx.id == effectId)
                {
                    return boyanFx;
                }
            }

            return null;
        }


        /// <summary>
        /// 恢复特效
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="effectId"></param>
        public void ResumeFx(int groupId, int effectId)
        {
            foreach (var boyanFx in m_boyanFxes)
            {
                if (boyanFx.groupId == groupId && boyanFx.id == effectId)
                {
                    boyanFx.Resume();
                    return;
                }
            }
        }

        /// <summary>
        /// 在 transform 相对位置播放一个特效，null表示世界坐标
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="parent"></param>
        /// <param name="follow"></param>
        /// <param name="block">如果被击方处于格挡</param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public NextBoyanFx SpawnFx(int id, Vector3 pos, Quaternion rotation, Vector3 scale, Transform parent = null,
            bool follow = false, bool block = false, VActor owner = null, bool pauseWithCreator = false,
            bool playOnGround = false)
        {
            NextBoyanFx fx = GetFx(id);
            if (fx)
            {
                if (follow == false)
                {
                    //这里follow传入true，实际在LateUpdate 移除Follow,为了保证特效位置在所有位移更新之后
                    UpdateFxInfo(fx, pos, rotation, scale, parent, true);
                    var item = new FxToPlayItem()
                    {
                        fx = fx,
                        follow = follow,
                        playOnGround = playOnGround,
                        terrainY = pos.y,
                        owner = owner
                    };
                    m_toPlayList.Add(item);
                }
                else
                {
                    UpdateFxInfo(fx, pos, rotation, scale, parent, follow);
                }

                fx.block = block;
                fx.id = id;
                fx.SetOnFinishAction(m_completeCallback);
                fx.SetNeedPausedWithCreator(pauseWithCreator);
                fx.Play(owner,follow);

                m_curPlayCount++;
                m_boyanFxes.Add(fx);

                //播放音效
                AudioManager.GetInstantiation().PlayEffect(id, owner);
            }

            return fx;
        }

        private void LateUpdate()
        {
            try
            {
                foreach (var playItem in m_toPlayList)
                {
                    playItem.UpdatePosition();
                }
            }
            catch (Exception e)
            {
                ViewDebug.LogWarning(e.Message);
            }

            m_toPlayList.Clear();
        }

        /// <summary>
        /// 给特效分配group id
        /// </summary>
        /// <param name="fx"></param>
        /// <param name="groupId"></param>
        public void SetGroupId(NextBoyanFx fx, int groupId)
        {
            fx.groupId = groupId;
        }

        /// <summary>
        /// 停止指定组得特效
        /// </summary>
        /// <param name="groupId"></param>
        public void StopFxGroup(int groupId)
        {
            for (int i = this.m_curPlayCount - 1; i >= 0; i--)
            {
                if (this.m_boyanFxes[i].groupId == groupId)
                {
                    StopFx(this.m_boyanFxes[i]);
                }
            }
        }

        /// <summary>
        /// 停止指定组得特效
        /// </summary>
        /// <param name="groupId"></param>
        public void StopFxGroup(int groupId, ItemDisappearReason reason)
        {
            BaseShell.FadeOutType fadeOutType = BaseShell.FadeOutType.ALL;
            switch (reason)
            {
                case ItemDisappearReason.AUTO:
                    fadeOutType = BaseShell.FadeOutType.NORMAL;
                    break;
                case ItemDisappearReason.OWNER_DEAD:
                case ItemDisappearReason.ACTION_CHANGE:
                    //中断
                    fadeOutType = BaseShell.FadeOutType.BREAK;
                    break;
                case ItemDisappearReason.BREAK:
                    //打破
                    fadeOutType = BaseShell.FadeOutType.BLOCK;
                    break;
            }

            for (int i = this.m_curPlayCount - 1; i >= 0; i--)
            {
                if (this.m_boyanFxes[i].groupId == groupId)
                {
                    StopFx(this.m_boyanFxes[i], fadeOutType);
                }
            }
        }

        private void StopFx(NextBoyanFx fx, BaseShell.FadeOutType fadeOutType = BaseShell.FadeOutType.NORMAL)
        {
            if (fx.IsPlaying())
            {
                fx.Cease(fadeOutType);
            }
            else
            {
                //可能刚创建，还没播放出来
                for (int i = m_toPlayList.Count - 1; i >= 0; i--)
                {
                    if (m_toPlayList[i].fx == fx)
                    {
                        m_toPlayList.RemoveAt(i);
                        OnFxPlayComplete(fx);
                        break;
                    }
                }
            }
        }

        public int GetAutoIncId()
        {
            return ++m_autoIncId;
        }

        private void UpdateFxInfo(NextBoyanFx fx, Vector3 pos, Quaternion rotation, Vector3 scale,
            Transform parent = null, bool follow = false)
        {
            var trans = fx.transform;
            bool parentIsNull = parent == null;
            fx.gameObject.SetActive(true);
            trans.SetParent(parentIsNull ? m_effectContainer : parent);
            trans.localPosition = pos;
            trans.localScale = scale;
            trans.localRotation = rotation;

            if (follow == false && !parentIsNull)
            {
                trans.SetParent(m_effectContainer);
            }
        }

        /// <summary>
        /// 更新特效信息
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="vActor"></param>
        /// <param name="bindPoint"></param>
        /// <param name="bindPath"></param>
        /// <param name="follow"></param>
        /// <param name="fxTag"></param>
        /// <param name="groupId"></param>
        public void UpdateFxInfo(Vector3 pos, Quaternion rotation, Vector3 scale, VActor vActor,
            int bindPoint, string bindPath, bool follow, string fxTag = null, int groupId = -1)
        {
            if (vActor != null && string.IsNullOrEmpty(fxTag) == false)
            {
                foreach (var boyanFx in m_boyanFxes)
                {
                    if (boyanFx.fxTag == fxTag && boyanFx.Creator == vActor)
                    {
                        Transform bindTarget = UCFxUtils.GetBindNode(vActor, bindPoint, bindPath);

                        UpdateFxInfo(boyanFx, pos, rotation, scale, bindTarget, follow);

                        //更新 随源标记
                        var srcFollow = m_fxActorRefrence.ContainsKey(boyanFx);
                        boyanFx.groupId = groupId;
                        if (follow != srcFollow)
                        {
                            if (follow)
                            {
                                m_fxActorRefrence.Add(boyanFx, vActor);
                            }
                            else
                            {
                                m_fxActorRefrence.Remove(boyanFx);
                            }
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 停止指定随源特效
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="id"></param>
        public void StopReferenceFx(VActor actor, int id)
        {
            //调用 cease
            for (int i = m_curPlayCount - 1; i >= 0; i--)
            {
                var fx = m_boyanFxes[i];
                if (fx.id == id && m_fxActorRefrence.TryGetValue(fx, out var target) && target == actor)
                {
                    StopFx(fx);
                }
            }
        }

        /// <summary>
        /// 或者指定的随源特效
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public NextBoyanFx GetReferenceFx(VActor actor, int id)
        {
            foreach (var fx in m_boyanFxes)
            {
                if (fx.id == id && m_fxActorRefrence.TryGetValue(fx, out var target) && target == actor)
                {
                    return fx;
                }
            }

            return null;
        }

        /// <summary>
        /// 停止指定随源特效
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="fxTag"></param>
        public void StopReferenceFx(VActor actor, string fxTag)
        {
            for (int i = m_curPlayCount - 1; i >= 0; i--)
            {
                var fx = m_boyanFxes[i];
                if (fx.fxTag == fxTag && m_fxActorRefrence.TryGetValue(fx, out var target) && target == actor)
                {
                    StopFx(fx);
                }
            }
        }

        /// <summary>
        /// 创建一个残影
        /// </summary>
        /// <returns>GhostShadowRoot 可以k帧控制</returns>
        public UCGhostShadowRoot SpawnGhostShadow(VRenderComponent target, Material material, bool useHQMesh,
            bool useMainTexture, float duration,
            float fadeTime, float distortionFadeTime, float fadeScale)
        {
            var ghostShadow = GetGhostShadowRoot();
            // GameObject go = poolMgr.Get(path);
            if (ghostShadow != null)
            {
                ghostShadow.Init(target, material, duration, fadeTime, distortionFadeTime, fadeScale, useMainTexture,
                    useHQMesh, m_shadowFadeComplete);
            }

            return ghostShadow;
        }

        private UCGhostShadowRoot GetGhostShadowRoot()
        {
            string path = PathDefine.GHOST_SHADOW_ROOT;
            var poolMgr = GameObjectPoolManager.Instance;
            if (poolMgr.HasPool(path) == false)
            {
                poolMgr.PreCreateGameObjectToPool(path, 10, path, KHResource.GetObject);
                //item的缓存池也创建
                var itemPath = PathDefine.GHOST_SHADOW_ITEM;
                poolMgr.PreCreateGameObjectToPool(itemPath, 10, itemPath, KHResource.GetObject);
            }

            GameObject go = poolMgr.Get(path);
            if (go) return go.GetComponent<UCGhostShadowRoot>();
            return null;
        }

        public UCGhostShadowRoot SpawnGhostShadow(SkinnedMeshRenderer skinnedMeshRenderer,Material material,float duration,
            float fadeTime,float distortionFadeTime, float fadeScale)
        {
            var ghostShadow = GetGhostShadowRoot();
            if (ghostShadow != null)
            {
                ghostShadow.Init(skinnedMeshRenderer,material,duration,fadeTime,distortionFadeTime,fadeScale,m_shadowFadeComplete);
            }

            return ghostShadow;
        }

        private void OnShadowFadeComplete(UCGhostShadowRoot ghostShadowRoot)
        {
            ghostShadowRoot.gameObject.SetActive(false);
            GameObjectPoolManager.Instance.Recycle(ghostShadowRoot.gameObject, PathDefine.GHOST_SHADOW_ROOT);
        }

        /// <summary>
        /// 停止指定随源特效
        /// </summary>
        /// <param name="actor"></param>
        public void StopReferenceFx(VActor actor)
        {
            for (int i = m_curPlayCount - 1; i >= 0; i--)
            {
                var fx = m_boyanFxes[i];
                if (m_fxActorRefrence.TryGetValue(fx, out var target) && target == actor)
                {
                    StopFx(fx);
                }
            }
        }

        /// <summary>
        /// 清除所有
        /// </summary>
        public void ClearAll()
        {
            for (int i = m_boyanFxes.Count - 1; i >= 0; i--)
            {
                var fx = m_boyanFxes[i];
                //不再回调
                fx.SetOnFinishAction(null);
                fx.Stop(true);

                // TODO 避免报错导致退出游戏失败，添加try catch
                try
                {
                    if (fx.gameObject != null)
                        GameObject.Destroy(fx.gameObject);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }

            m_curPlayCount = 0;
            m_boyanFxes.Clear();

            foreach (var item in m_cacheFx.Values)
            {
                while (item.Count > 0)
                {
                    var fx = item.Pop();
                    fx.Stop(true);
                    if (fx)
                    {
                        GameObject.Destroy(fx.gameObject);
                    }
                }
            }

            m_cacheFx.Clear();

            var poolMgr = GameObjectPoolManager.Instance;
            foreach (var path in m_effectPaths)
            {
                poolMgr.Clear(path);
            }

            m_effectPaths.Clear();
            m_LinkFxReferences.Clear();

            m_fxActorRefrence.Clear();
            m_toPlayList.Clear();
        }

        /// <summary>
        /// 特效播放结束，回池操作
        /// </summary>
        /// <param name="fx"></param>
        private void OnFxPlayComplete(NextBoyanFx fx)
        {
            if (fx == null) return;
            //播放消失音效
            AudioManager.GetInstantiation().StopEffect(fx.id, fx.Creator as VActor);

            //
            m_curPlayCount--;
            m_boyanFxes.Remove(fx);
            m_cacheFx.TryGetValue(fx.id, out var effects);
            if (m_fxActorRefrence.ContainsKey(fx))
            {
                m_fxActorRefrence.Remove(fx);
            }

            fx.groupId = 0;

            if (effects == null)
            {
                effects = new Stack<NextBoyanFx>(4);
                m_cacheFx.Add(fx.id, effects);
            }

            effects.Push(fx);
            fx.gameObject.SetActive(false);
            fx.transform.SetParent(m_effectContainer);
        }

        private void OnLinkFxComplete(NextBoyanFx fx)
        {
            OnFxPlayComplete(fx);
            //从链接关系列表里面移除
            for (int i = m_LinkFxReferences.Count - 1; i >= 0; i--)
            {
                if (m_LinkFxReferences[i].fx == fx)
                {
                    m_LinkFxReferences.RemoveAt(i);
                    break;
                }
            }
        }
        
        /// <summary>
        /// 移除所有身上的链接特效
        /// </summary>
        /// <param name="actor"></param>
        public void StopLinkFxAll(VActor actor)
        {
            for (int i = m_LinkFxReferences.Count - 1; i >= 0; i--)
            {
                if (m_LinkFxReferences[i].IsCreator(actor) || m_LinkFxReferences[i].IsReference(actor))
                {
                    var fx = m_LinkFxReferences[i].fx;
                    m_LinkFxReferences.RemoveAt(i);
                    if( fx ) fx.Cease();
                }
            }
        }

        /// <summary>
        /// 移除自己创建的链接特效
        /// </summary>
        /// <param name="actor"></param>
        public void StopLinkFxCreated(VActor actor)
        {
            for (int i = m_LinkFxReferences.Count - 1; i >= 0; i--)
            {
                if (m_LinkFxReferences[i].IsCreator(actor))
                {
                    var fx = m_LinkFxReferences[i].fx;
                    m_LinkFxReferences.RemoveAt(i);
                    if( fx ) fx.Cease();
                }
            }
        }

        /// <summary>
        /// 移除自身别人创建的链接特效
        /// </summary>
        /// <param name="actor"></param>
        public void StopLinkFxOtherCreated(VActor actor)
        {
            for (int i = m_LinkFxReferences.Count - 1; i >= 0; i--)
            {
                if (m_LinkFxReferences[i].IsCreator(actor) == false && m_LinkFxReferences[i].IsReference(actor))
                {
                    var fx = m_LinkFxReferences[i].fx;
                    m_LinkFxReferences.RemoveAt(i);
                    if( fx ) fx.Cease();
                }
            }
        }

        private NextBoyanFx GetFx(int id)
        {
            if (!m_enableSpawn) return null;
            
            m_cacheFx.TryGetValue(id, out var cache);
            if (cache != null && cache.Count > 0)
            {
                return cache.Pop();
            }
            else
            {
                string path = PathDefine.GetEffectResPath(id.ToString());
                var poolMgr = GameObjectPoolManager.Instance;
                if (poolMgr.HasPool(path) == false)
                {
                    m_effectPaths.Add(path);
                    poolMgr.PreCreateGameObjectToPool(path, 1, path, KHResource.GetObject);
                }

                GameObject effectObj = poolMgr.Get(path);
                if (effectObj != null)
                {
                    return effectObj.GetComponent<NextBoyanFx>();
                }
            }

            return null;
        }

        /// <summary>
        /// 预加载特效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        public void Preload(int id, int count)
        {
            string path = PathDefine.GetEffectResPath(id.ToString());
            var poolMgr = GameObjectPoolManager.Instance;
            if (poolMgr.HasPool(path) && poolMgr.GetPool(path).GetObjectCountInPool() > count)
            {
                //有足够缓存
                return;
            }

            if (m_effectPaths.Contains(path) == false)
            {
                m_effectPaths.Add(path);
            }

            GameObjectPoolManager.Instance.PreCreateGameObjectToPool(path, count, path, KHResource.GetObject);
            GameObject effectObj = GameObjectPoolManager.Instance.Get(path);

            NextBoyanFx fx = effectObj.GetComponent<NextBoyanFx>();
            if (fx)
            {
                //TODO:unity新版本待测,把texture和mesh都touch一下
                TryWarmTexturesAndMeshes(effectObj.transform);
                GameObjectPoolManager.Instance.Recycle(effectObj, path);
            }
            else
            {
                Debug.LogError("fx_找不到特效脚本:" + path);
            }
        }

        private static void TryWarmTexturesAndMeshes(Transform root)
        {
            Renderer rootRenderer = root.GetComponent<Renderer>();
            MeshFilter meshFilter = root.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
#pragma warning disable
                Mesh m = meshFilter.sharedMesh;
#pragma warning restore
            }

            if (rootRenderer != null && rootRenderer.sharedMaterials != null)
            {
                int numMats = rootRenderer.sharedMaterials.Length;

                for (int i = 0; i < numMats; ++i)
                {
                    Material mat = rootRenderer.sharedMaterials[i]; /* Silence MonoBehaviour Warnings*/
                    TouchTexturesFromMaterial(mat);
                }
            }

            int numChildren = root.childCount;

            for (int j = 0; j < numChildren; ++j)
            {
                TryWarmTexturesAndMeshes(root.GetChild(j));
            }
        }

        //目前用这种方式强制把shader中的贴图都加载进来。
        private static readonly string[] s_shaderTexturePropertyNames = new string[]
        {
            "_BaseMap",
            "_Mask",
        };

        private static void TouchTexturesFromMaterial(Material material)
        {
            if (material == null)
            {
                return;
            }

            int numProps = s_shaderTexturePropertyNames.Length;

            for (int i = 0; i < numProps; ++i)
            {
                if (material.HasProperty(s_shaderTexturePropertyNames[i]))
                {
#pragma warning disable
                    Texture t = material.GetTexture(s_shaderTexturePropertyNames[i]); /* Silence MonoBehaviour Warnings*/
#pragma warning restore
                }
            }
        }

        private string GetFxContainer()
        {
            return "Fx Cache Container";
        }
    }
}