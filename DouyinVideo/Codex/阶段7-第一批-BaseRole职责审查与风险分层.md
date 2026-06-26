# 阶段 7 第一批：BaseRole 职责审查与风险分层

日期：2026-06-16  
目标：在不修改游戏代码的前提下，审查 `BaseRole` 当前承担的职责、依赖关系和后续瘦身风险。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批只做审查和分层，不修改游戏代码。

结论：

- `BaseRole` 当前是所有英雄的共同父类，也是自动乱斗的核心角色运行层。
- 19 个英雄脚本继承 `BaseRole`，全部覆盖 `Big()`，没有发现子类覆盖 `StartGame()` 或 `UseBig()`。
- `BaseRole` 不能直接拆组件；阶段 7 后续应先做注释、分区、私有方法提取，再考虑更大的拆分。
- `BaseRole` 里有少量疑似遗留字段，但涉及 public/序列化风险，不能一刀切删除。

产出文件：

- `Codex/阶段7-第一批-BaseRole职责审查与风险分层.md`
- `Codex/阶段7-第一批-diff.patch`

## BaseRole 当前职责

| 职责 | 代码位置 | 说明 | 风险 |
|---|---|---|---|
| 角色基础数据 | `BaseRole.cs:15-27` | `RoleName`、`HP`、头像、UI、受伤音频、大招音频、角色颜色 | 高 |
| 运行时组件缓存 | `BaseRole.cs:29-33`、`52-59` | 缓存 `Rigidbody2D`、`AudioSource`、`Collider2D`、`Canvas`、负面效果父节点 | 中 |
| 全局运动驱动 | `BaseRole.cs:66-92` | `FixedUpdate` 里根据全局球速和 `speedPercent` 重设速度 | 高 |
| 初始随机推动 | `BaseRole.cs:57`、`301-306` | 启动 3 秒后给角色随机方向冲量 | 高 |
| 速度状态 | `BaseRole.cs:99-135` | `SetSpeed` 和协程恢复速度 | 高 |
| 拾取/大招禁用状态 | `BaseRole.cs:142-198` | `SetCanGetWeapon`、`SetCanUseBig`、UI 图标、延迟恢复 | 高 |
| 血量与治疗 | `BaseRole.cs:201-212` | `RecoverHp`、血量上限、UI 刷新 | 高 |
| 受伤与死亡 | `BaseRole.cs:217-256` | 扣血、死亡广播、移除刚体/碰撞体、灰图、通知管理器 | 极高 |
| 大招点与大招入口 | `BaseRole.cs:262-290` | `AddBig`、满 3 点释放、`UseBig`、调用抽象 `Big()` | 极高 |
| 音频播放 | `BaseRole.cs:292-296`、`355-358` | 角色自身 `AudioSource.PlayOneShot` 和碰撞音效 | 中 |
| 目标选择 | `BaseRole.cs:41`、`308-311` | `otherBaseRole` 调用 `VideoGameTargetUtility` | 中 |
| UI 层级查找 | `BaseRole.cs:313-339` | 查找 `Canvas`、`负面效果`，移除状态图标 | 中 |
| 大招球拾取 | `BaseRole.cs:342-352` | 触发 `BigBall` 后加大招点、播放音效、销毁球 | 极高 |

## 子类覆盖点

实际搜索结果：

- 19 个英雄脚本继承 `BaseRole`。
- 19 个英雄脚本都实现 `public override void Big()`。
- 没有发现子类覆盖 `StartGame()`。
- 没有发现子类覆盖 `UseBig()`。

继承 `BaseRole` 的英雄：

| 英雄脚本 |
|---|
| `Assets/人物信息/霓虹/NiHongRole.cs` |
| `Assets/人物信息/钱包/QianBaoRole.cs` |
| `Assets/人物信息/钢索/GangsuoRole.cs` |
| `Assets/人物信息/迪迦/DiJiaRole.cs` |
| `Assets/人物信息/维斯/WeiSiRole.cs` |
| `Assets/人物信息/源氏/YuanShiRole.cs` |
| `Assets/人物信息/KO/KORole.cs` |
| `Assets/人物信息/烟男/YanNanRole.cs` |
| `Assets/人物信息/绊线保安/BanXianBaoAnRole.cs` |
| `Assets/人物信息/捷风/JieFengRole.cs` |
| `Assets/人物信息/gay保安/GayBaoAnRole.cs` |
| `Assets/人物信息/猎枭/LieXiaoRole.cs` |
| `Assets/人物信息/炸弹妹/BoomSisterRole.cs` |
| `Assets/人物信息/狂鼠/KuangShuRole.cs` |
| `Assets/人物信息/夜露/YeLuRole.cs` |
| `Assets/人物信息/火墙火男/FireWallFireManRole.cs` |
| `Assets/人物信息/后羿/HouYiRole.cs` |
| `Assets/人物信息/奶妈/NaiMaRole.cs` |
| `Assets/人物信息/丢帽子保安/DropHatBaoAnRole.cs` |

## 外部依赖关系

| 模块 | 依赖内容 | 说明 |
|---|---|---|
| `VideoGameManager` | `circleRoles`、`roleUI`、`roleColor`、`RoleDie`、`ballSpeed` | 启动时给角色绑定 UI 和颜色，死亡时移出列表 |
| `RoleUI` | `TakeDamage`、`AttackOther`、`ClearBig` | 血量显示、大招点显示、流光动画入口 |
| 英雄脚本 | `Big()`、`otherBaseRole`、`SetSpeed`、`TakeDamage`、`RecoverHp` | 大多数英雄大招直接依赖 BaseRole 公共 API |
| 武器脚本 | `canGetWeapon`、`otherBaseRole`、`SetSpeed`、`TakeDamage` | 标配、奥丁、小刀等会读取或调用角色能力 |
| `VideoGameCombatUtility` | `TakeDamage`、`SetSpeed`、`SetCanGetWeapon`、`SetCanUseBig` | 战斗工具层封装 BaseRole 调用 |
| `AudioManager` | `PlaySFX("吃到球")`、`PlaySFX("撞击")` | 吃球和碰撞音效 |
| `Broadcast` | 淘汰战报 | 死亡时播报 |
| DOTween | `DOColor`、`DOKill` | 受伤闪红、死亡停止动画 |
| 娱乐机制试点 | `TakeDamage`、`SetSpeed`、`circleRoles` | 默认未挂载，但代码层依赖 BaseRole |

## 绝对不能直接动的区域

以下区域后续阶段 7 不应直接重构，除非单独立项并做逐项验证。

| 区域 | 原因 |
|---|---|
| `FixedUpdate` 运动逻辑 | 每个物理帧都会执行，直接决定自动弹球表现 |
| `ApplyRandomForce` 启动时机 | 启动 3 秒后给角色初始运动，改变会影响开局节奏 |
| `SetSpeed` 和 `SetSpeedCoro` 顺序 | 多个英雄、武器、娱乐机制依赖速度变化和恢复 |
| `TakeDamage` 死亡流程 | 涉及血量、UI、广播、刚体/碰撞体销毁、胜利判定 |
| `AddBig` 与 `UseBig` | 决定大招点、满 3 点释放、UI 清空和英雄大招入口 |
| `OnTriggerEnter2D` 的 `BigBall` 逻辑 | 直接影响吃球、加大招点和能量球销毁 |
| `canGetWeapon`、`canUseBig` public 字段 | 武器和状态效果直接读取 |
| `roleUI`、`roleColor` public 字段 | `VideoGameManager` 和大量播报逻辑依赖 |
| `Canvas` / `负面效果` 层级字符串 | 依赖角色 Prefab 内部层级结构 |

## 可以低风险整理的区域

这些内容可以作为阶段 7 第二批候选，但仍应保持小步改动。

| 候选 | 建议 | 风险 |
|---|---|---|
| 文件结构 | 增加 `#region` 分区：字段、生命周期、运动、状态、血量、大招、工具、碰撞 | 低 |
| 注释 | 给重要公开方法补中文 XML 注释，修正参数说明 | 低 |
| 格式 | 统一空格、空行、`private` 显式标注 | 低 |
| 私有协程命名 | 只在不影响调用的情况下整理注释，不急着改名 | 低 |
| `StartGame()` 注释 | 标注当前未被子类覆盖，保留扩展口 | 低 |
| `lashiValue` 注释 | 标注为旧娱乐机制草稿字段，先不删除 | 低 |
| `CantSelected` 注释 | 标注当前未发现代码引用，先不删除 public 字段 | 低 |

## 可以函数级整理的区域

这些内容适合阶段 7 第三批之后做，不建议第二批立刻动。

| 候选 | 可提取方向 | 注意 |
|---|---|---|
| `TakeDamage` | 提取 `HandleDeath()`、`PlayHitFlash()`、`DisablePhysicsOnDeath()` | 必须保持扣血、UI、死亡判断顺序 |
| `SetCanGetWeapon` / `SetCanUseBig` | 提取状态图标显示/移除共用方法 | 不能改变图标实例化和恢复协程顺序 |
| `AddBig` | 提取大招点上限和释放判断私有方法 | 不能改变 `roleUI.AttackOther` 与 `canUseBig` 判断顺序 |
| `FixedUpdate` | 只可提取 `UpdateMoveVelocity()` 私有方法 | 不改随机偏移和速度公式 |
| `OnTriggerEnter2D` | 提取 `HandleBigBallTrigger()` | 不改 `AddBig -> PlaySFX -> Destroy` 顺序 |

## 暂不建议做的拆分

以下方向是长期目标，不适合阶段 7 前几批直接改。

| 拆分方向 | 暂缓原因 |
|---|---|
| `RoleMovement` 组件 | 会改变运动初始化、FixedUpdate 执行位置和 Prefab 组件结构 |
| `RoleHealth` 组件 | 死亡流程牵涉 UI、Broadcast、VideoGameManager、DOTween |
| `RoleUltimate` 组件 | 大招点和 `Big()` 抽象入口直接绑定全部英雄 |
| `RoleStatusEffect` 组件 | 状态 UI 图标依赖角色层级和当前 public 字段 |
| `RolePickup` 组件 | 大招球拾取属于玩法触发链路，和阶段 6 审查结论一致，应谨慎 |

## 发现的可疑点

| 项 | 代码位置 | 判断 | 处理建议 |
|---|---|---|---|
| `lashiValue` | `BaseRole.cs:45-50` | 私有字段，当前未发现使用；像旧娱乐机制草稿 | 第二批可只加注释，不直接删 |
| `CantSelected` | `BaseRole.cs:43` | public 字段，当前未发现代码引用 | 因 public 可能被 Inspector 或外部使用，暂不删 |
| `StartGame()` | `BaseRole.cs:61-64` | 当前没有子类覆盖 | 暂保留，作为英雄初始化扩展口 |
| `count` 类似字段 | 无 | `BaseRole` 没有发现类似计数遗留 | 无 |
| 受伤 Debug 日志 | `BaseRole.cs:225`、`250` | 会产生 Console 输出，但可能用于调试 | 不能在第一批处理，可作为后续低风险日志候选 |

## 阶段 7 后续建议

| 批次 | 内容 | 是否改游戏代码 | 风险 |
|---|---|---|---|
| 第二批 | `BaseRole` 注释、region、格式整理，标注旧字段，不删 public 字段 | 是 | 低 |
| 第三批 | `TakeDamage`、`AddBig`、`OnTriggerEnter2D` 等长逻辑提取私有方法 | 是 | 中低 |
| 第四批 | 状态效果图标逻辑整理，减少重复 `Instantiate/RemoveEffectIcon` | 是 | 中 |
| 第五批 | 组件化拆分决策文档，判断是否进入 `RoleMovement/RoleHealth` 等长期拆分 | 先文档 | 中高 |

## 本批验证

本批未修改游戏代码，不需要编译或运行。

已完成的检查：

- 已读取 `Assets/Scripts/BaseRole.cs` 全量代码。
- 已搜索所有 `BaseRole` 子类。
- 已搜索 `StartGame`、`UseBig`、`Big` 覆盖情况。
- 已搜索 `TakeDamage`、`SetSpeed`、`SetCanGetWeapon`、`SetCanUseBig`、`AddBig` 等公共 API 调用点。

## 总结

`BaseRole` 可以瘦身，但不能从“拆组件”开始。当前最安全的路径是先让它变清楚，再让它变小：

1. 先整理注释和区域。
2. 再提取私有方法。
3. 再收敛状态效果和 UI 辅助逻辑。
4. 最后才讨论组件化拆分。
