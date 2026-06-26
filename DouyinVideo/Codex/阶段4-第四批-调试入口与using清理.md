# 阶段 4 第四批：调试入口与 using 清理

日期：2026-06-16  
目标：在不改变英雄大招逻辑的前提下，小步整理中低风险英雄脚本的开发调试入口和明显无用 `using`。

## 本批范围

本批只做两类低风险清理：

1. 删除确认未使用的 `using`。
2. 给已有开发调试 `Update()` 加 `#region 开发调试入口`。

没有改动：

1. 大招数值。
2. `WaitForSeconds` 时间。
3. `Input` 调试按键。
4. `Update()` 方法体内容。
5. `StartCoroutine` / `StopCoroutine` 顺序。
6. 播报、伤害、状态结算顺序。
7. Prefab、Scene、Inspector 绑定。

本批刻意不整理 `GangsuoRole`、`FireWallFireManRole`、`BanXianBaoAnRole` 等高风险复杂英雄。

## 修改文件

| 文件 | 修改类型 | 是否改变玩法 | 风险 |
|---|---|---|---|
| `Assets/人物信息/gay保安/GayBaoAnRole.cs` | 无用 `using` 清理 | 否 | 低 |
| `Assets/人物信息/后羿/HouYiRole.cs` | 无用 `using` 清理 | 否 | 低 |
| `Assets/人物信息/源氏/YuanShiRole.cs` | 无用 `using` 清理 | 否 | 低 |
| `Assets/人物信息/KO/KORole.cs` | 无用 `using` 清理 | 否 | 低 |
| `Assets/人物信息/烟男/YanNanRole.cs` | 无用 `using` 清理 | 否 | 低 |
| `Assets/人物信息/丢帽子保安/DropHatBaoAnRole.cs` | 无用 `using` 清理/调试入口分区 | 否 | 低 |
| `Assets/人物信息/狂鼠/KuangShuRole.cs` | 无用 `using` 清理/调试入口分区 | 否 | 低 |
| `Assets/人物信息/猎枭/LieXiaoRole.cs` | 无用 `using` 清理/调试入口分区 | 否 | 低 |
| `Assets/人物信息/夜露/YeLuRole.cs` | 无用 `using` 清理/调试入口分区 | 否 | 低 |
| `Codex/阶段4-第四批-调试入口与using清理.md` | 新增文档 | 否 | 低 |
| `Codex/阶段4-第一批-英雄脚本结构审查与模板定稿.md` | 更新后续批次状态 | 否 | 低 |
| `Codex/新增英雄指南.md` | 补充清理记录引用 | 否 | 低 |
| `Codex/安全重构计划.md` | 更新阶段 4 状态 | 否 | 低 |
| `Codex/修改记录.md` | 追加改动记录 | 否 | 低 |

## using 清理明细

| 脚本 | 清理内容 | 保留原因 |
|---|---|---|
| `GayBaoAnRole.cs` | 删除 `System.Collections`、`System.Collections.Generic` | 该脚本不使用协程或集合 |
| `HouYiRole.cs` | 删除 `System.Collections.Generic` | 仍保留 `System.Collections`，因为使用 `IEnumerator` |
| `YuanShiRole.cs` | 删除 `System.Collections.Generic`、`System.Data` | 仍保留 `System.Collections`，因为使用 `IEnumerator` |
| `KORole.cs` | 删除 `System.Collections.Generic` | 仍保留 `DG.Tweening` 和 `System.Collections` |
| `YanNanRole.cs` | 删除 `System.Collections.Generic` | 仍保留 `System.Collections`，因为使用 `IEnumerator` |
| `DropHatBaoAnRole.cs` | 删除 `System.Collections.Generic` | 仍保留 `System.Collections` |
| `KuangShuRole.cs` | 删除 `System.Collections.Generic` | 仍保留 `System.Collections` |
| `LieXiaoRole.cs` | 删除 `UnityEngine.UI`、`DG.Tweening` | 仍保留 `System.Collections` |
| `YeLuRole.cs` | 删除 `System.Collections.Generic`、`UnityEngine.UI` | `CanvasGroup` 来自 `UnityEngine` |

## 调试入口整理明细

以下脚本只给原有 `Update()` 外层加了分区，没有改方法体。

| 脚本 | 原调试入口 | 本批处理 |
|---|---|---|
| `DropHatBaoAnRole.cs` | `Input.GetKeyDown(KeyCode.M)` 调用 `UseBig()` | 加 `#region 开发调试入口` |
| `KuangShuRole.cs` | `Input.GetKeyDown(KeyCode.M)` 调用 `UseBig()` | 加 `#region 开发调试入口` |
| `LieXiaoRole.cs` | `Input.GetMouseButtonDown(0)` 调用 `UseBig()` | 加 `#region 开发调试入口` |
| `YeLuRole.cs` | `Input.GetKeyDown(KeyCode.M)` 后 `StopAllCoroutines()` 再 `Big()` | 加 `#region 开发调试入口`，不改原逻辑 |

## Diff 摘要

```diff
~ GayBaoAnRole.cs
  - 删除未使用 using

~ HouYiRole.cs
~ YuanShiRole.cs
~ KORole.cs
~ YanNanRole.cs
  - 删除未使用 using

~ DropHatBaoAnRole.cs
~ KuangShuRole.cs
~ LieXiaoRole.cs
~ YeLuRole.cs
  - 删除未使用 using
  + 给原有 Update 调试入口加 #region 开发调试入口
  = 保留原调试按键和方法体

+ Codex/阶段4-第四批-调试入口与using清理.md
~ Codex/阶段4-第一批-英雄脚本结构审查与模板定稿.md
~ Codex/新增英雄指南.md
~ Codex/安全重构计划.md
~ Codex/修改记录.md
```

## 风险说明

| 风险点 | 等级 | 说明 |
|---|---|---|
| 删除无用 `using` | 低 | 编译可验证，不影响运行逻辑 |
| 添加 `#region` | 低 | 只是代码折叠标记，不参与运行 |
| 未改调试按键 | 低 | 保持原开发调试行为 |
| 夜露保留 `StopAllCoroutines()` | 中低 | 该写法本身较危险，但本批只标注，不改变 |

## 建议验证

| 验证项 | 说明 |
|---|---|
| Unity 编译 | 确认清理 using 后无编译错误 |
| 丢帽子/狂鼠调试入口 | 如使用调试，确认 `M` 键仍触发 |
| 猎枭调试入口 | 如使用调试，确认鼠标左键仍触发 |
| 夜露调试入口 | 如使用调试，确认 `M` 键行为仍与原来一致 |
| 正常自动流程 | 运行 `VideoScene`，确认英雄自动释放大招不受影响 |

## 回滚方式

1. 恢复本批修改过的 9 个英雄脚本。
2. 恢复本批修改过的 `Codex/` 文档。
