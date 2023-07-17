EffectMgr开发笔记



目标
1、同意特效的接口，便于统一管理
2、支持参数传入
3、每次播放特效生成一个唯一ID，后续用该ID控制特效
4、需要配套一个EffectCtrl，负责控制特效的逻辑
5、特效需要有权重配置，便于后续在复杂的场合中按需显示特效。

特效分类
1、场景世界坐标下
2、跟随Transform（轨迹）
3、连线特效（小狐狸）
4、

行动
1、搭建场景
2、简易特效播放器

----梳理FxManager

-------在Transform下播放一个特效
public NextBoyanFx SpawnFx

参数：
int id,				// 特效id  一个特效prefab对应一个id
Transform parent = null,		// 父节点 null表示世界坐标
Vector3 pos, 			// 偏移
Quaternion rotation, 		// 旋转Vector3 scale,
bool follow = false, 
bool block = false, 
VActor owner = null, 		// 角色 这里可以是PlayerController
bool pauseWithCreator = false,
bool playOnGround = false)
{
}

--------- 根据ID拿特效
private NextBoyanFx GetFx(int id)