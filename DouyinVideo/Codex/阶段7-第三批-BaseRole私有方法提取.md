# 阶段 7 第三批：BaseRole 私有方法提取

日期：2026-06-16  
目标：在不改变运行行为的前提下，把 `BaseRole` 中部分长逻辑提取为私有方法，降低后续维护难度。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批修改了 `BaseRole.cs`，只做函数级局部提取。

没有改动：

- 没有修改任何 public 字段。
- 没有修改任何 public 方法签名。
- 没有修改 `FixedUpdate` 的执行入口。
- 没有修改任何数值、协程等待时间或判断条件。
- 没有修改 `TakeDamage` 的扣血、UI、死亡、受伤音效顺序。
- 没有修改 `AddBig` 的大招点增加、UI 飞光、禁用大招判断、满点释放顺序。
- 没有修改吃大招球的 `AddBig -> PlaySFX -> Destroy` 顺序。
- 没有修改 Scene、Prefab、Inspector 绑定。

## 修改文件列表

| 文件 | 修改内容 | 是否改变玩法 | 风险 |
|---|---|---|---|
| `Assets/Scripts/BaseRole.cs` | 提取运动、死亡、受伤、大招点、吃球私有方法 | 否 | 中低 |
| `Codex/阶段7-第三批-diff.patch` | 保存本批完整 diff | 否 | 低 |

## 提取内容

### 运动逻辑

原 `FixedUpdate()` 内部逻辑拆为：

- `StopMovementWhenGameEnded()`
- `UpdateMoveVelocity()`

保留顺序：

1. 如果 `isDie`，直接返回。
2. 如果 `gameEnd`，停止刚体并返回。
3. 否则按原公式更新移动速度。

### 受伤与死亡逻辑

原 `TakeDamage(int damage)` 内部逻辑拆为：

- `HandleDeath()`
- `DisablePhysicsOnDeath()`
- `PlayHitFlash()`

保留顺序：

1. 死亡角色直接返回。
2. 扣血。
3. 刷新 `roleUI`。
4. `HP <= 0` 时执行死亡流程。
5. 否则播放受伤闪红。
6. 最后播放 `getHurt` 音效。

死亡流程内部顺序保持：

1. 输出死亡日志。
2. `HP = 0`。
3. 播放淘汰战报。
4. 停止并销毁刚体。
5. 销毁碰撞体。
6. 停止头像颜色动画。
7. 头像变灰。
8. 标记 `isDie = true`。
9. 降低角色 Canvas 排序。
10. 通知 `VideoGameManager.RoleDie(this)`。

### 大招点逻辑

原 `AddBig()` 内部逻辑拆为：

- `ClampBigCountToMax()`
- `TryUseBigWhenReady()`

保留顺序：

1. `bigCount++`。
2. 限制最大值为 3。
3. 调用 `roleUI.AttackOther(transform.position)`。
4. 如果 `canUseBig == false`，直接返回。
5. 如果 `bigCount >= 3`，调用 `UseBig()`。
6. `bigCount = 0`。

### 大招球触发逻辑

原 `OnTriggerEnter2D(Collider2D collision)` 内部逻辑拆为：

- `HandleBigBallTrigger(Collider2D collision)`

保留顺序：

1. 判断 `collision.CompareTag("BigBall")`。
2. 调用 `AddBig()`。
3. 播放 `"吃到球"` 音效。
4. `Destroy(collision.gameObject)`。

## 风险说明

本批风险为中低。

原因：

- 这次动的是 `BaseRole` 核心脚本，影响所有英雄。
- 但修改方式是局部私有方法提取，没有改公共接口和玩法规则。
- 编译可以发现语法问题，但不能完全证明运行表现一致，所以仍建议人工验证一轮主场景。

## 建议人工验证

| 验证点 | 预期结果 |
|---|---|
| 运行 `VideoScene` | 所有角色正常启动并在 3 秒后开始运动 |
| 游戏结束 | 胜利后角色停止运动表现不变 |
| 角色受伤未死亡 | 血量 UI 更新，头像闪红，受伤音效播放 |
| 角色死亡 | 淘汰战报、刚体/碰撞体移除、头像变灰、胜利判断表现不变 |
| 角色吃大招球 | 大招点增加、吃球音效播放、球消失 |
| 满 3 个大招点 | 继续调用对应英雄 `Big()`，并清空 UI 大招点 |

## 回滚建议

如本批出现问题，可按 `Codex/阶段7-第三批-diff.patch` 回滚：

- `Assets/Scripts/BaseRole.cs`

本批没有修改场景或 Prefab，回滚脚本即可恢复。
