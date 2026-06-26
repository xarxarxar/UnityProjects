# 阶段 7 第四批：BaseRole 状态图标逻辑整理

日期：2026-06-16  
目标：在不改变状态效果表现的前提下，整理 `BaseRole` 中拾取武器限制和大招限制的重复图标逻辑。  
主场景：`Assets/Scenes/VideoScene.unity`

## 本批结论

本批修改了 `BaseRole.cs`，只整理 `SetCanGetWeapon` / `SetCanUseBig` 的重复结构。

没有改动：

- 没有修改 public 字段。
- 没有修改 public 方法签名。
- 没有修改状态持续时间。
- 没有修改状态恢复时机。
- 没有修改图标 Prefab。
- 没有修改图标名称常量。
- 没有修改 Scene、Prefab、Inspector 绑定。

## 修改文件列表

| 文件 | 修改内容 | 是否改变玩法 | 风险 |
|---|---|---|---|
| `Assets/Scripts/BaseRole.cs` | 提取状态图标准备、创建、恢复辅助方法 | 否 | 中低 |
| `Codex/阶段7-第四批-diff.patch` | 保存本批完整 diff | 否 | 低 |

## 提取内容

新增私有方法：

| 方法 | 作用 |
|---|---|
| `PrepareAbilityIconParent(string iconName)` | 获取负面效果父节点，并移除旧状态图标 |
| `CreateAbilityIcon(GameObject iconPrefab, Transform effectParent)` | 创建新的状态图标 |
| `RestoreAbilityState(ref bool state, string iconName)` | 将状态恢复为 `true`，并移除状态图标 |

## 顺序保持说明

### 设置不能拾取武器

原顺序：

1. 获取 `负面效果` 父节点。
2. 移除旧的 `不能拾取武器(Clone)` 图标。
3. 设置 `canGetWeapon = CanGetWeapon`。
4. 实例化 `CantGetWeaponPrefab`。
5. 如果 `duration != 0`，启动恢复协程。

新顺序：

1. `PrepareAbilityIconParent(CantGetWeaponIconName)`。
2. 设置 `canGetWeapon = CanGetWeapon`。
3. `CreateAbilityIcon(VideoGameManager.instance.CantGetWeaponPrefab, effectParent)`。
4. 如果 `duration != 0`，启动恢复协程。

### 恢复拾取武器能力

原顺序：

1. 等待 `duration`。
2. 设置 `canGetWeapon = true`。
3. 移除 `不能拾取武器(Clone)` 图标。

新顺序：

1. 等待 `duration`。
2. `RestoreAbilityState(ref canGetWeapon, CantGetWeaponIconName)`。

`RestoreAbilityState` 内部仍然先设置状态为 `true`，再移除图标。

### 设置不能使用大招

原顺序：

1. 获取 `负面效果` 父节点。
2. 移除旧的 `KO压制UI(Clone)` 图标。
3. 设置 `canUseBig = CanUseBig`。
4. 实例化 `CantUseBigPrefab`。
5. 如果 `duration != 0`，启动恢复协程。

新顺序：

1. `PrepareAbilityIconParent(CantUseBigIconName)`。
2. 设置 `canUseBig = CanUseBig`。
3. `CreateAbilityIcon(VideoGameManager.instance.CantUseBigPrefab, effectParent)`。
4. 如果 `duration != 0`，启动恢复协程。

### 恢复大招使用能力

原顺序：

1. 等待 `duration`。
2. 设置 `canUseBig = true`。
3. 移除 `KO压制UI(Clone)` 图标。

新顺序：

1. 等待 `duration`。
2. `RestoreAbilityState(ref canUseBig, CantUseBigIconName)`。

`RestoreAbilityState` 内部仍然先设置状态为 `true`，再移除图标。

## 风险说明

本批风险为中低。

原因：

- 修改位置在 `BaseRole`，影响所有角色。
- 但只提取重复状态图标逻辑，没有改变状态规则。
- `canGetWeapon` 和 `canUseBig` 仍然是原 public 字段。
- 调用方 `VideoGameCombatUtility`、武器脚本、小刀逻辑不需要改。

重点验证：

- 维斯缴械后，目标不能拾取武器，持续时间结束后恢复。
- KO 压制后，目标不能使用大招，持续时间结束后恢复。
- 状态图标不会重复堆叠，旧图标仍会先移除。

## 回滚建议

如本批出现问题，可按 `Codex/阶段7-第四批-diff.patch` 回滚：

- `Assets/Scripts/BaseRole.cs`

本批没有修改场景或 Prefab，回滚脚本即可恢复。
