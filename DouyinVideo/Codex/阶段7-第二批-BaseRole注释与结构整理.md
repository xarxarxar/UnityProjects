# 阶段 7 第二批：BaseRole 注释与结构整理

日期：2026-06-16  
目标：在不改变运行行为的前提下，让 `BaseRole` 更清楚、更适合后续小步瘦身。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批修改了 `BaseRole.cs`，但只做低风险结构整理：

- 增加中文 XML 注释。
- 增加 `#region` 分区。
- 清理未使用的 `using System.Collections.Generic;`。
- 显式标注私有 Unity 生命周期和协程方法。
- 保留原有字段，不删除疑似遗留字段。
- 保留关键逻辑顺序，不拆函数、不抽组件。

没有改动：

- 没有修改 `FixedUpdate` 的运动公式。
- 没有修改 `ApplyRandomForce` 的 3 秒延迟。
- 没有修改 `SetSpeed`、`SetCanGetWeapon`、`SetCanUseBig` 的协程顺序。
- 没有修改 `TakeDamage` 的扣血、死亡、广播、移除刚体/碰撞体、灰图和胜利通知顺序。
- 没有修改 `AddBig`、`UseBig`、`Big` 的调用顺序。
- 没有修改 `OnTriggerEnter2D` 吃大招球的 `AddBig -> PlaySFX -> Destroy` 顺序。
- 没有修改 Scene、Prefab、Inspector 绑定。

## 修改文件列表

| 文件 | 修改内容 | 是否改变玩法 | 风险 |
|---|---|---|---|
| `Assets/Scripts/BaseRole.cs` | 注释、region、格式和旧字段标注 | 否 | 低 |
| `Codex/阶段7-第二批-diff.patch` | 保存本批完整 diff | 否 | 低 |

## 主要整理内容

### 分区结构

新增分区：

- `常量`
- `Inspector 字段`
- `运行时状态`
- `Unity 生命周期`
- `速度控制`
- `状态限制`
- `生命值`
- `大招`
- `音频`
- `目标选择`
- `工具方法`
- `碰撞触发`

目的：

- 让后续第三批提取私有方法时更容易定位。
- 避免 `BaseRole` 继续变成所有逻辑混在一起的大文件。

### 注释整理

补充或修正的公开方法注释：

- `Start()`
- `StartGame()`
- `SetSpeed(float percent, float duration = 0)`
- `SetCanGetWeapon(bool CanGetWeapon, float duration = 0)`
- `SetCanUseBig(bool CanUseBig, float duration = 0)`
- `RecoverHp(int value)`
- `TakeDamage(int damage)`
- `AddBig()`
- `UseBig()`
- `Big()`
- `PlayAudio(AudioClip audioClip)`
- `GetRandomOther(BaseRole self)`

补充的重要私有方法注释：

- `FixedUpdate()`
- `SetSpeedCoro(float duration)`
- `SetCanGetWeaponCoro(float duration)`
- `SetCanUseBigCoro(float duration)`
- `ApplyRandomForce()`
- `GetRoleCanvas()`
- `GetNegativeEffectParent()`
- `RemoveEffectIcon(...)`
- `OnTriggerEnter2D(...)`
- `OnCollisionEnter2D(...)`

### 旧字段处理

本批没有删除以下字段：

| 字段 | 处理 |
|---|---|
| `CantSelected` | 保留 public 字段，不删除，避免外部或 Inspector 潜在依赖 |
| `lashiValue` | 保留私有字段，只标注为旧娱乐机制草稿字段 |

原因：

- `CantSelected` 是 public 字段，虽然当前代码搜索未发现引用，但不排除场景、Prefab、外部调试或未来机制使用。
- `lashiValue` 当前未接入正式厕所机制，但直接删除不是本批目标。

## 风险说明

本批风险为低。

原因：

- 没有改变任何数值。
- 没有改变任何协程等待时间。
- 没有改变任何事件、触发器、生命周期方法的调用顺序。
- 没有修改继承体系。
- 没有拆组件。

需要注意：

- `BaseRole.cs` 的源码结构变化较大，diff 看起来会比较长；但核心语句顺序保持不变。
- 本批只为后续第三批“函数级提取”做准备。

## 建议人工验证

| 验证点 | 预期结果 |
|---|---|
| 运行 `VideoScene` | 所有角色正常启动并在 3 秒后开始运动 |
| 角色碰撞 | 角色继续播放碰撞音效 |
| 角色吃大招球 | 大招点增加，吃球音效播放，球消失 |
| 角色受伤 | 血量 UI 更新，未死亡时闪红 |
| 角色死亡 | 战报、灰图、移除刚体/碰撞体、胜利判断表现不变 |
| 大招释放 | 满 3 点后继续调用对应英雄 `Big()` |

## 回滚建议

如本批出现问题，可按 `Codex/阶段7-第二批-diff.patch` 回滚：

- `Assets/Scripts/BaseRole.cs`

本批没有修改场景或 Prefab，回滚脚本即可恢复旧结构。
