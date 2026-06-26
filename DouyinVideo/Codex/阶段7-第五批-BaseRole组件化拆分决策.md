# 阶段 7 第五批：BaseRole 组件化拆分决策

日期：2026-06-16  
目标：判断 `BaseRole` 是否可以继续拆成 `RoleMovement`、`RoleHealth`、`RoleUltimate` 等组件。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批只做审查和决策，不修改游戏代码。

结论：

- 短期不建议把 `BaseRole` 直接拆成多个 `MonoBehaviour` 组件。
- `BaseRole` 目前应继续作为角色统一门面，保留 public 字段、public 方法和 `Big()` 抽象入口。
- 后续如果继续瘦身，应优先抽工具类、文档模板和私有辅助逻辑，不应先改 Prefab 组件结构。
- 真正组件化必须单独立项，且需要允许修改 Prefab / Scene / Inspector 绑定后才能安全推进。

## 代码证据

| 证据 | 位置 | 说明 |
|---|---|---|
| 19 个英雄继承 `BaseRole` | `Assets/人物信息/*/*Role.cs` | 所有英雄通过继承接入统一角色能力 |
| 19 个英雄都覆盖 `Big()` | `public override void Big()` 搜索结果 | 每个英雄的大招都依赖 `BaseRole.UseBig()` 调用抽象入口 |
| 未发现子类覆盖 `StartGame()` | `override void StartGame` 搜索结果为空 | 目前 `StartGame()` 是保留扩展口，不是实际多态核心 |
| 未发现子类覆盖 `UseBig()` | `override void UseBig` 搜索结果为空 | 当前大招释放顺序集中在 `BaseRole.UseBig()` |
| `BaseRole` public 字段集中 | `Assets/Scripts/BaseRole.cs:23`、`:26`、`:29`、`:32`、`:35`、`:38`、`:40`、`:55`、`:56`、`:60` | 这些字段可能被 Prefab、场景或外部脚本依赖，不能改名或改访问级别 |
| 运动由 `FixedUpdate` 驱动 | `Assets/Scripts/BaseRole.cs:92` | 每个物理帧重设角色速度，直接影响自动弹球表现 |
| 初始推动是协程 | `Assets/Scripts/BaseRole.cs:459` | 开局 3 秒后加随机冲量，改组件会影响启动顺序 |
| 速度状态被外部调用 | `Assets/Scripts/BaseRole.cs:146` | 英雄、武器、娱乐机制试点都会调用 `SetSpeed()` |
| 生命和死亡集中在 `TakeDamage()` | `Assets/Scripts/BaseRole.cs:302` | 牵涉扣血、UI、广播、DOTween、物理组件移除和胜负判断 |
| 大招入口集中在 `AddBig()` / `UseBig()` | `Assets/Scripts/BaseRole.cs:374`、`:409`、`:421` | 大招点、UI 流光、音效、清空大招点和英雄 `Big()` 顺序绑定很紧 |
| 大招球拾取在 `OnTriggerEnter2D()` | `Assets/Scripts/BaseRole.cs:516` | 直接决定吃球、加大招点、播放音效、销毁球 |
| `VideoGameManager` 直接写入角色 UI 和颜色 | `Assets/Scripts/VideoGameManager.cs:67-70` | `RoleName`、`roleImage`、`roleUI`、`roleColor` 是启动流程的一部分 |
| 武器直接读取 `canGetWeapon` | `Assets/人物信息/所有武器/标配/BiaoPei.cs:125`、`Assets/人物信息/所有武器/奥丁/AoDing.cs:130`、`Assets/Scripts/Knief.cs:69` | 拾取条件依赖 public 字段 |
| 钢索直接写 `canUseBig` | `Assets/人物信息/钢索/GangsuoRole.cs:47`、`:202` | 大招内部直接操作 `BaseRole` 状态字段 |

## 拆分决策表

| 拆分候选 | 当前决策 | 风险 | 原因 | 后续安全入口 |
|---|---|---|---|---|
| `RoleMovement` | 暂不拆 | 高 | 运动依赖 `Start()` 缓存、3 秒初始推动、`FixedUpdate` 顺序、`SetSpeed()` 协程和死亡时刚体销毁 | 只可先提取非 `MonoBehaviour` 的计算辅助方法，由 `BaseRole` 主动调用 |
| `RoleHealth` | 暂不拆 | 极高 | `TakeDamage()` 同时处理 HP、UI、死亡、广播、DOTween、Collider/Rigidbody 销毁、`VideoGameManager.RoleDie()` | 继续保持 `TakeDamage()` 为唯一入口，后续只做私有方法级别整理 |
| `RoleUltimate` | 暂不拆 | 极高 | `AddBig()`、`UseBig()`、`Big()` 与 19 个英雄脚本强绑定，改动会影响全部大招 | 继续通过新增英雄模板约束写法，不抽新组件 |
| `RoleStatusEffect` | 暂不拆组件 | 中高 | `canGetWeapon`、`canUseBig` 是 public 字段，状态图标依赖角色 Canvas 层级 | 可继续在 `BaseRole` 内提取私有方法或静态工具，不移动字段归属 |
| `RoleTargetSelector` | 已有工具层即可 | 低 | `otherBaseRole` 已委托 `VideoGameTargetUtility`，没有必要新增组件 | 后续新增目标选择规则时继续扩展工具类 |
| `RoleAudio` | 暂不拆 | 中 | `PlayAudio()` 被多个英雄直接调用，且 `BaseRole` 已 `[RequireComponent(typeof(AudioSource))]` | 可在不改 public API 的前提下做空引用保护或音频工具，但暂不需要 |
| `RoleIdentity` / `RoleView` | 暂不拆 | 高 | `RoleName`、`roleImage`、`roleColor`、`roleUI` 与 Prefab、启动 UI、战报强绑定 | 后续只允许新增只读访问器，不替换原字段 |
| `RoleBigBallPickup` | 暂不拆 | 高 | 大招球触发链路是玩法核心，且阶段 6 已暂缓 `bigBallPrefab` 池化 | 只在需要改大招球机制时单独审查 |

## 推荐长期结构

短期结构：

```text
BaseRole
├── 保留角色 public 字段和 public 方法
├── 保留 FixedUpdate / TakeDamage / AddBig / UseBig / OnTriggerEnter2D
├── 通过私有方法降低单个函数复杂度
└── 通过 Utility 降低英雄、武器重复代码
```

长期结构：

```text
BaseRole 仍作为门面
├── RoleMovementLogic（非 MonoBehaviour，纯逻辑辅助，BaseRole 主动调用）
├── RoleStatusIconUtility（状态图标辅助，不拥有状态字段）
├── VideoGameTargetUtility（继续负责目标选择）
├── VideoGameCombatUtility（继续负责伤害、播报、状态组合调用）
└── 英雄脚本只实现 Big()
```

不推荐的短期结构：

```text
BaseRole
├── RoleMovement : MonoBehaviour
├── RoleHealth : MonoBehaviour
├── RoleUltimate : MonoBehaviour
└── RoleStatusEffect : MonoBehaviour
```

原因是这种结构需要给所有英雄 Prefab 增加组件、迁移序列化字段、确认 Unity 生命周期顺序，并重测全部英雄和武器。它不是当前“玩法完全不变”的安全优化范畴。

## 后续允许做什么

可以继续做：

- 给新增英雄文档补充 `BaseRole` public API 使用规范。
- 给新增玩法机制文档补充“不要直接写 `canUseBig` / `canGetWeapon`，优先使用方法”的建议。
- 继续把重复逻辑抽成工具类，但由原调用点主动调用。
- 继续给 `BaseRole` 增加少量只读属性，但不能删除或替代现有 public 字段。

暂时不要做：

- 不要新增 `RoleMovement`、`RoleHealth`、`RoleUltimate` 并挂到 Prefab。
- 不要把 `HP`、`roleImage`、`roleUI`、`roleColor`、`canGetWeapon`、`canUseBig` 从 `BaseRole` 移走。
- 不要改变 `UseBig()` 调用 `Big()` 和 `roleUI.ClearBig()` 的顺序。
- 不要改变 `TakeDamage()` 内扣血、UI、死亡判断、音效播放的顺序。
- 不要把 `FixedUpdate` 移到其它组件。

## 下一步建议

阶段 7 可以在第五批后暂时收口。

如果仍要继续阶段 7，建议只做一个低风险文档批次：

| 批次 | 内容 | 是否改游戏代码 | 风险 |
|---|---|---|---|
| 阶段 7 第六批，可选 | `BaseRole` public API 使用规范，告诉后续新增英雄/武器/机制应该调用哪些方法、避免直接碰哪些字段 | 否 | 低 |

如果要进入代码优化，建议不要继续拆 `BaseRole`，而是回到更明确的局部目标，例如：

- 阶段 6 的 `flyLightPrefab` 对象池试点。
- 单个复杂英雄脚本的小步整理。
- 新增英雄/武器模板的实际落地。

## 本批验证

本批没有修改游戏代码，不需要编译。

已完成检查：

- 搜索 `BaseRole` 子类与 `Big()` 覆盖情况。
- 搜索 `StartGame()` / `UseBig()` 覆盖情况。
- 搜索 `BaseRole` public 字段和 public 方法的外部使用。
- 复核 `BaseRole` 当前核心入口行号。

## 回滚建议

本批只新增和修改 Markdown 文档。

如需回滚：

- 删除 `Codex/阶段7-第五批-BaseRole组件化拆分决策.md`。
- 恢复 `Codex/安全重构计划.md` 和 `Codex/修改记录.md` 中本批新增内容。
