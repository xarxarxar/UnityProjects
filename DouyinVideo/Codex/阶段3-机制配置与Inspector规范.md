# 阶段 3 第二批：机制配置与 Inspector 规范

日期：2026-06-16  
目标：在不修改游戏代码的前提下，明确未来新增机制时哪些配置可以放到 Inspector、哪些应该留在代码或文档中，并保护正常模式默认行为。

## 当前结论

本批不新增运行时代码。  
本批只定义未来机制的配置规范和 Inspector 暴露原则。

娱乐机制最小入口设计见：`Codex/阶段3-娱乐机制最小入口设计.md`。

核心结论：

1. 正常模式默认不启用娱乐机制。
2. 机制开关必须清晰，默认值必须保守。
3. Inspector 只暴露常用、必要、低误操作风险的配置。
4. 复杂规则不要全部塞进 Inspector。
5. 每个机制必须有 `Codex/` 文档说明配置含义、默认值和验证点。

## 配置分层

未来机制配置建议分成三层：

| 层级 | 放置位置 | 示例 | 适合内容 |
|---|---|---|---|
| 必要开关 | Manager Inspector | `enableToiletMechanism` | 是否启用机制、是否显示调试日志 |
| 少量核心数值 | Manager Inspector 或 Config | `damagePerTick`、`stayDuration` | 经常调、容易理解、误改风险低的数值 |
| 复杂规则 | 代码 + `Codex/` 文档 | 便意增长公式、随机事件权重组合 | 需要上下文说明、容易误改的规则 |

## Inspector 暴露原则

### 适合暴露

| 类型 | 示例 | 原因 |
|---|---|---|
| 启用开关 | `enableMechanism` | 方便快速开关机制 |
| Prefab 引用 | 厕所 Prefab、特效 Prefab | 必须由场景或资源绑定 |
| 父节点/区域引用 | 生成点父物体、场地范围 | 依赖场景对象 |
| 关键数值 | 扣血值、持续时间、生成间隔 | 经常调试，含义直接 |
| 调试开关 | `showDebugLog` | 方便录制前排查 |

### 不适合暴露

| 类型 | 原因 |
|---|---|
| 每个英雄的细碎开关 | 未来英雄多了以后配置爆炸 |
| 大量重复数值 | 维护困难，容易漏改 |
| 复杂规则组合 | Inspector 不适合表达复杂逻辑 |
| 正常模式核心规则 | 容易误改现有录制表现 |
| 协程时序核心参数 | 改动可能改变画面节奏 |
| 胜负条件相关参数 | 风险高，必须单独阶段处理 |

## 默认值规则

| 配置 | 默认值建议 | 原因 |
|---|---|---|
| 娱乐机制总开关 | `false` | 保护正常模式 |
| 单个娱乐机制开关 | `false` | 避免新机制默认影响旧玩法 |
| 调试日志 | `false` | 避免录制时刷屏 |
| 可视化调试 Gizmos | `false` | 避免干扰画面 |
| 正常模式基础规则 | 不通过新机制配置改动 | 避免误触 |

如果某个机制未来被决定“默认就是游戏玩法的一部分”，必须先修改机制文档和 `Codex/修改记录.md`，再改默认值。

## 字段命名规范

| 用途 | 推荐命名 | 示例 |
|---|---|---|
| 是否启用 | `enableXxx` | `enableToiletMechanism` |
| 持续时间 | `xxxDuration` | `stayDuration` |
| 间隔 | `xxxInterval` | `urgeIncreaseInterval` |
| 阈值 | `xxxThreshold` | `maxUrgeThreshold` |
| 每次伤害 | `damagePerXxx` | `damagePerTick` |
| Prefab | `xxxPrefab` | `toiletPrefab` |
| 父节点 | `xxxParent` | `toiletParent` |
| 生成区域 | `xxxSpawnArea` | `toiletSpawnArea` |

字段命名优先表达用途，不追求缩写。

## Tooltip 规范

Inspector 可见字段必须有中文 `[Tooltip]`，但不需要写太长。

推荐：

```csharp
[Tooltip("是否启用上厕所机制")]
[SerializeField] private bool enableToiletMechanism = false;

[Tooltip("便意达到上限后每次扣血值")]
[SerializeField] private int damagePerTick = 5;

[Tooltip("角色进入厕所后的强制停留时间")]
[SerializeField] private float stayDuration = 5f;
```

不推荐：

```csharp
public bool a;
public float t;
public int value;
```

## Config 使用建议

短期可以先用 Manager Inspector 字段。  
当一个机制配置超过约 8 个字段，或多个机制需要复用同一套配置时，再考虑独立 Config。

可选结构：

```csharp
[System.Serializable]
public class ToiletMechanismConfig
{
    [Tooltip("便意增长间隔")]
    public float urgeIncreaseInterval = 2f;

    [Tooltip("便意上限")]
    public int maxUrge = 100;

    [Tooltip("便意满后每次扣血值")]
    public int damagePerTick = 5;
}
```

注意：

1. 现在不需要提前创建 Config 代码。
2. Config 不要变成所有机制共享的大杂烩。
3. 每个 Config 应服务一个明确机制。

## 机制文档模板

每个新机制实现前，先在 `Codex/` 新建设计文档。

推荐字段：

| 项目 | 内容 |
|---|---|
| 机制名称 |  |
| 机制类型 | 正常机制 / 娱乐机制 / 待定 |
| 默认是否启用 | 是 / 否 |
| Inspector 开关 | 字段名和默认值 |
| Inspector 数值 | 字段名、默认值、含义 |
| 不放 Inspector 的规则 | 写清楚原因 |
| 影响对象 | 所有英雄 / 部分英雄 / 场景对象 |
| 依赖脚本 | Manager / 状态组件 / 区域组件 |
| 使用的 BaseRole 接口 | 例如 `TakeDamage`、`SetSpeed` |
| 使用的工具类 | 例如 `VideoGameCombatUtility` |
| 风险点 |  |
| 验证方式 |  |

## 上厕所机制配置示例

| 配置名 | 推荐位置 | 默认值 | 说明 |
|---|---|---|---|
| `enableToiletMechanism` | Inspector | `false` | 是否启用机制 |
| `toiletPrefab` | Inspector | 空 | 厕所对象 |
| `toiletParent` | Inspector | 空 | 厕所生成父节点 |
| `urgeIncreaseInterval` | Inspector 或 Config | `2f` | 便意增长间隔 |
| `maxUrge` | Inspector 或 Config | `100` | 便意上限 |
| `damagePerTick` | Inspector 或 Config | `5` | 便意满后的单次扣血 |
| `damageInterval` | Inspector 或 Config | `1f` | 扣血间隔 |
| `stayDuration` | Inspector 或 Config | `5f` | 进入厕所后的强制停留时间 |
| `showDebugLog` | Inspector | `false` | 是否打印机制日志 |

不建议放 Inspector：

| 规则 | 原因 |
|---|---|
| 每个英雄独立便意增长倍率 | 配置量过大，后续英雄越多越乱 |
| 复杂随机事件权重 | Inspector 难以表达，需要单独文档 |
| 是否改写死亡逻辑 | 高风险，不属于娱乐机制配置 |

## 新机制接入前检查

| 检查项 | 是否通过 |
|---|---|
| 默认不影响正常模式 |  |
| 有明确启用开关 |  |
| Inspector 字段都有中文 Tooltip |  |
| 只暴露少量关键配置 |  |
| 复杂规则已写进 `Codex/` 文档 |  |
| 没有把机制状态塞进 `BaseRole` |  |
| 没有修改武器刷新、能量球生成、胜负判断 |  |
| 已写建议验证点 |  |
| 已追加到 `Codex/修改记录.md` |  |

## 本批风险说明

| 风险点 | 等级 | 说明 |
|---|---|---|
| 只有文档改动 | 低 | 不参与 Unity 编译，不影响运行行为 |
| 规范可能随第一个机制调整 | 低 | 第一个机制落地时可以根据实际情况修订 |
| 未新增运行时配置系统 | 低 | 避免过早抽象，等机制真实出现再实现 |

## 验证结果

本批没有修改游戏代码，因此未运行 `dotnet build`。  
已做文档复核：确认文档位于 `Codex/`，并确认无冲突标记。
