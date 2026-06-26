# 阶段 8 第一批：flyLightPrefab 对象池试点

日期：2026-06-17  
目标：在不改变大招点 UI 表现和玩法逻辑的前提下，将 `flyLightPrefab` 从“每次创建销毁”改为“动画结束后回收复用”。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批修改了 `VideoGameManager.cs`，只处理 `PlayFlyEffect()` 内部的 UI 流光对象生命周期。

没有改动：

- 没有修改 `RoleUI.AttackOther()`。
- 没有修改 `BaseRole.AddBig()`。
- 没有修改 `bigBallPrefab`。
- 没有修改大招点增加规则。
- 没有修改流光数量，默认仍是 `count = 3`。
- 没有修改飞行路径、随机偏移、动画时长或 Ease。
- 没有修改 Prefab、Scene 或 Inspector 绑定。

## 修改文件列表

| 文件 | 修改内容 | 是否改变玩法 | 风险 |
|---|---|---|---|
| `Assets/Scripts/VideoGameManager.cs` | 新增 `flyLightPool`，将流光动画结束后的 `Destroy` 改为回收到池中 | 否 | 中低 |
| `Codex/阶段8-第一批-diff.patch` | 保存本批完整 diff | 否 | 低 |

## 原链路

```text
BaseRole.AddBig()
-> RoleUI.AttackOther(transform.position)
-> VideoGameManager.PlayFlyEffect(startPoint, endPoint)
-> Instantiate(flyLightPrefab, canvasTransform)
-> DOPath(..., 0.6f)
-> Destroy(flyCoin)
```

## 新链路

```text
BaseRole.AddBig()
-> RoleUI.AttackOther(transform.position)
-> VideoGameManager.PlayFlyEffect(startPoint, endPoint)
-> GetFlyLight()
   -> 池里有对象：取出并 SetActive(true)
   -> 池里无对象：Instantiate(flyLightPrefab, canvasTransform)
-> PrepareFlyLight(flyCoin, screenStart)
-> DOPath(..., 0.6f)
-> ReturnFlyLight(flyCoin)
   -> DOKill()
   -> SetActive(false)
   -> 放回池中
```

## 顺序保持说明

`PlayFlyEffect()` 保持以下顺序不变：

1. 每次循环先得到一个流光对象。
2. 设置起点为 `screenStart`。
3. 计算 `midPoint = (screenStart + screenEnd) / 2f`。
4. 使用原来的 `Random.Range(-100f, 100f)` 和 `Random.Range(100f, 200f)`。
5. 使用原来的三点路径 `{ screenStart, midPoint, screenEnd }`。
6. 使用原来的 `DOPath(path, 0.6f, PathType.CatmullRom)`。
7. 使用原来的 `SetEase(Ease.InOutQuad)`。
8. 动画完成回调仍在 `OnComplete` 中执行。

唯一变化是第 8 步中不再 `Destroy(flyCoin)`，而是 `ReturnFlyLight(flyCoin)`。

## 新增私有方法

| 方法 | 作用 |
|---|---|
| `GetFlyLight()` | 从池中取流光对象，池为空时才创建 |
| `PrepareFlyLight(GameObject flyCoin, Vector3 screenStart)` | 清理旧 Tween，重设父节点、旋转、缩放和起点 |
| `ReturnFlyLight(GameObject flyCoin)` | 停止旧 Tween，隐藏对象并放回池中 |

## 风险说明

本批风险为中低。

原因：

- `flyLightPrefab` 是 UI 表现对象，不参与碰撞、伤害、胜负和大招点计数。
- 修改点集中在 `VideoGameManager.PlayFlyEffect()`。
- `RoleUI.AttackOther()` 和 `BaseRole.AddBig()` 没有改。
- `bigBallPrefab` 仍按原逻辑创建和销毁。

需要关注：

- 如果未来 `flyLightPrefab` 增加脚本、粒子、音效或复杂状态，需要重新评估池化重置逻辑。
- 如果流光对象在动画未完成前被外部销毁，`ReturnFlyLight()` 会直接跳过空对象。

## 建议人工验证点

| 验证点 | 预期结果 |
|---|---|
| 角色吃到大招球 | 大招点 UI 仍增加 |
| 单次获得大招点 | 仍出现 3 条流光飞行动画 |
| 连续获得大招点 | 流光不会残留、卡住或瞬移到错误位置 |
| 大招点满 3 格 | 仍能触发英雄大招并清空 UI |
| 长时间录制 | `流光粒子(Clone)` 数量不再持续无限增长 |

## 回滚建议

如本批出现表现异常，可按 `Codex/阶段8-第一批-diff.patch` 回滚：

- 恢复 `Assets/Scripts/VideoGameManager.cs` 中的 `PlayFlyEffect()`。
- 删除本批新增的 `flyLightPool`、`GetFlyLight()`、`PrepareFlyLight()`、`ReturnFlyLight()`。

本批没有修改场景或 Prefab，回滚脚本即可恢复。
