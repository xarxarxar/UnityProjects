# 阶段 6 第五批：flyLightPrefab 与 bigBallPrefab 池化审查

日期：2026-06-16  
目标：审查 `flyLightPrefab` 和 `bigBallPrefab` 的生成销毁链路，判断哪些能安全池化，哪些应该继续暂缓。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批只做审查和决策，不修改游戏代码。

产出文件：

- `Codex/阶段6-第五批-flyLight与bigBall池化审查.md`
- `Codex/阶段6-第五批-diff.patch`

结论：

| 对象 | 当前用途 | 池化建议 | 风险 |
|---|---|---|---|
| `flyLightPrefab` | UI 大招点飞行动画 | 可以作为后续安全池化试点 | 中低 |
| `bigBallPrefab` | 场景中的大招能量球，角色触发后获得大招点 | 暂缓池化 | 中 |

原因：

- `flyLightPrefab` 是纯 UI 表现对象，不参与碰撞、数值和胜负判定。
- `bigBallPrefab` 是玩法触发物，池化需要改 `BaseRole.OnTriggerEnter2D` 中的销毁逻辑，会触碰玩法链路。

## 代码证据

### `flyLightPrefab`

绑定位置：

- `Assets/Scenes/VideoScene.unity`
- `VideoGameManager.flyLightPrefab`
- guid：`c346281b58723484dadfeec7c197d9b9`
- 实际 Prefab：`Assets/Prefabs/UI/流光粒子.prefab`

Prefab 组件：

| 组件 | 说明 |
|---|---|
| `RectTransform` | UI 坐标对象 |
| `CanvasRenderer` | UI 渲染 |
| `Image` | 显示流光图片 |

没有发现：

- 自定义脚本。
- 碰撞体。
- 粒子系统。
- 音效组件。

生成销毁链路：

| 步骤 | 代码位置 | 行为 |
|---|---|---|
| 大招点增加 | `BaseRole.AddBig()` | 调用 `roleUI.AttackOther(transform.position)` |
| UI 找空位 | `RoleUI.AttackOther(Vector3 myPos)` | 找到第一个空大招点图标 |
| 播放流光 | `VideoGameManager.PlayFlyEffect(startPoint, endPoint)` | 默认生成 3 个流光对象 |
| 移动动画 | `VideoGameManager.PlayFlyEffect` | `DOPath(path, 0.6f, PathType.CatmullRom)` |
| 销毁对象 | `VideoGameManager.PlayFlyEffect` | `OnComplete(() => Destroy(flyCoin))` |

当前特点：

- 每次增加大招点会生成 3 个 UI 流光对象。
- 每个对象动画时间固定为 `0.6f`。
- 结束后销毁。
- 不影响 `bigCount`，只是表现。

### `bigBallPrefab`

绑定位置：

- `Assets/Scenes/VideoScene.unity`
- `VideoGameManager.bigBallPrefab`
- guid：`94b1dc43ce0dab345821cb37899de289`
- 实际 Prefab：`Assets/人物信息/吸球.prefab`

Prefab 组件：

| 组件 | 说明 |
|---|---|
| `Transform` | 世界坐标对象 |
| `CircleCollider2D` | 触发器，`m_IsTrigger: 1` |
| `SpriteRenderer` | 显示能量球图片 |

关键配置：

- Tag：`BigBall`
- `CircleCollider2D` 半径：`0.8`
- Prefab 缩放：`2.2, 2.2, 1`

生成销毁链路：

| 步骤 | 代码位置 | 行为 |
|---|---|---|
| 开始生成 | `VideoGameManager.Start()` | `Invoke(nameof(SpawnBigBall), 3)` |
| 循环生成 | `VideoGameManager.SpawnBigBallCoro()` | `while (true)` |
| 生成对象 | `VideoGameManager.SpawnBigBallCoro()` | 每 4 秒 `Instantiate(bigBallPrefab, randomPos, Quaternion.identity)` |
| 生成事件 | `VideoGameManager.SpawnBigBallCoro()` | `OnBallGenerate?.Invoke(tmpBall)` |
| 触发拾取 | `BaseRole.OnTriggerEnter2D(Collider2D collision)` | `collision.CompareTag("BigBall")` |
| 获得大招点 | `BaseRole.OnTriggerEnter2D` | `AddBig()` |
| 播放音效 | `BaseRole.OnTriggerEnter2D` | `AudioManager.Instance.PlaySFX("吃到球")` |
| 销毁对象 | `BaseRole.OnTriggerEnter2D` | `Destroy(collision.gameObject)` |

当前特点：

- 大招球是玩法对象，不是纯表现。
- 被角色触发后会立刻改变角色大招点。
- 当前没有发现 `OnBallGenerate` 的订阅者，但它是公开静态事件，未来可能被使用。
- 如果大招球一直没人吃，会持续留在场景中；池化不能解决未拾取对象持续积累的问题，除非新增生命周期上限，而这会改变玩法表现。

## 池化判断

### `flyLightPrefab`：建议后续可以池化

适合池化的原因：

- 纯 UI 表现对象。
- 动画结束后自然可以回收。
- 没有自定义脚本状态。
- 没有碰撞和玩法触发。
- 每次 `PlayFlyEffect` 默认生成 3 个，频率可能随着吃球和大招点增长而提高。

建议的安全实现边界：

- 只在 `VideoGameManager.PlayFlyEffect` 内部改。
- 不修改 `RoleUI.AttackOther`。
- 不修改 Prefab 和 Scene 绑定。
- 使用运行时队列缓存 `GameObject`。
- 从池中取出时：
  - 设置父级为 `canvasTransform`。
  - 设置 `transform.position = screenStart`。
  - 重置 `localScale` 和 `localRotation`。
  - `SetActive(true)`。
  - 调用 `DOKill()` 清理旧 Tween。
- 回收时：
  - `DOKill()`。
  - `SetActive(false)`。
  - 放回池中。

需要注意：

- DOTween 的旧回调必须清理，否则复用对象可能触发旧 `OnComplete`。
- 如果以后流光 Prefab 增加粒子、音效或脚本状态，池化前要重新评估。

### `bigBallPrefab`：建议暂缓池化

暂缓原因：

- 它直接参与 `BaseRole.OnTriggerEnter2D` 的玩法逻辑。
- 当前吃球后使用 `Destroy(collision.gameObject)`，如果改成回收，需要改 `BaseRole` 或新增球对象脚本。
- 从销毁改成立即 `SetActive(false)`，可能改变同一物理帧内多个角色同时触发时的边界表现。
- `OnBallGenerate` 是公开静态事件，虽然当前未发现订阅者，但未来订阅者可能依赖“新实例”语义。
- 未拾取的大招球会持续存在；池化只减少“被吃掉后再生成”的创建销毁，不解决未拾取球的积累。

如果未来要池化，建议先满足：

| 前置条件 | 原因 |
|---|---|
| 明确是否允许同一颗球同帧只被一个角色吃到 | 池化可能让对象更快失活 |
| 明确 `OnBallGenerate` 是否会被后续系统使用 | 避免改变事件对象生命周期 |
| 明确是否需要给大招球设置最大存在时间 | 否则未拾取球仍会积累 |
| 先做单独验证批次 | 这是玩法对象，不适合夹在其他优化里改 |

## 建议后续顺序

1. 如果继续阶段 6，下一批可以做 `flyLightPrefab` 对象池试点。
2. `bigBallPrefab` 继续暂缓，只保留审查结论。
3. 在 `flyLightPrefab` 池化验证稳定后，再考虑其它纯表现对象。
4. `bigBallPrefab` 只有在你接受玩法触发链路小改动时再做。

## 建议人工验证点

本批没有改游戏代码，无需运行验证。

如果后续池化 `flyLightPrefab`，建议验证：

| 验证点 | 预期结果 |
|---|---|
| 角色吃到大招球 | 大招点 UI 仍增加 |
| 流光飞行动画 | 仍从角色位置飞到大招点位置 |
| 连续吃球 | 流光不会卡住、残留或瞬移异常 |
| 长时间录制 | 流光克隆数量不会无限增长 |

如果未来池化 `bigBallPrefab`，建议验证：

| 验证点 | 预期结果 |
|---|---|
| 单个角色吃球 | 大招点、音效、球消失表现一致 |
| 多个角色同时靠近同一颗球 | 不出现重复加点或异常丢球 |
| 长时间无人吃球 | 行为与旧逻辑一致，除非明确加入生命周期规则 |
| 触发大招 | 三个大招点满后释放大招表现不变 |
