# 阶段 5 第五批：新增武器模板与 BaseWeapon 决策

日期：2026-06-16  
目标：在不修改游戏运行代码的前提下，整理新增武器的安全模板和 `BaseWeapon` 取舍结论，为后续新增很多武器降低复制成本和重构风险。

## 本批范围

本批只做文档和模板整理。

| 文件 | 作用 |
|---|---|
| `Codex/阶段5-第五批-新增武器模板与BaseWeapon决策.md` | 记录本批结论、模板边界和后续判断标准 |
| `Codex/模板-普通枪械武器.cs.txt` | 普通拾取连射武器模板，不参与编译 |
| `Codex/模板-特殊武器设计卡片.md` | 特殊武器设计前检查卡，不参与编译 |

没有改动：

1. 游戏代码。
2. Prefab、Scene、Inspector 绑定。
3. `VideoGameManager.Weapons` 刷新列表。
4. 现有武器继承结构。
5. `BaseRole`、`VideoGameBulletBase`、`VideoGameWeaponUtility`。

## BaseWeapon 当前决策

结论：**暂不引入 `BaseWeapon` 运行时代码。**

原因：

| 判断项 | 当前情况 | 结论 |
|---|---|---|
| 可刷新武器数量 | 当前只有标配、奥丁、小刀 3 个 | 数量不足以支撑继承体系收益 |
| 武器类型差异 | 标配/奥丁是枪械，小刀是旋转近战 | 强行继承容易把特殊逻辑塞进父类 |
| Prefab 绑定风险 | 现有武器依赖 `transform.parent`、子弹父物体和场景绑定 | 改父类可能牵动绑定和生命周期 |
| 已有工具类 | `VideoGameWeaponUtility`、`VideoGameCombatUtility`、`VideoGameBulletBase` 已能复用基础能力 | 先用工具函数和模板即可 |
| 用户目标 | 当前最重要是稳定流畅，后续再大量扩展 | 不应为抽象牺牲稳定性 |

## 什么时候重新考虑 BaseWeapon

满足以下任意两条，再考虑做 `BaseWeapon` 设计文档和试点：

| 条件 | 说明 |
|---|---|
| 普通枪械类可刷新武器达到 5 个以上 | 且都走拾取、挂载、瞄准、连射、回收流程 |
| 3 个以上武器出现完全相同字段 | 如 `ownerRole`、`targetRole`、`bulletCount`、`shootPos`、`bullet` |
| 3 个以上武器出现完全相同生命周期 | 如 `AttachToRole`、`Shoot`、`SetDefault` 结构几乎一致 |
| 新增武器经常忘记回收或刷新 | 模板不足以约束错误时再考虑父类 |
| 需要统一禁用、暂停、游戏结束回收 | 生命周期需求变强时可重新评估 |

重新评估时也不要一次迁移所有武器。建议先新增一个 `BaseWeapon` 试验脚本，再让一个全新武器继承它，不改标配、奥丁、小刀。

## 当前推荐路线

短期新增武器：

```text
复制 `Codex/模板-普通枪械武器.cs.txt`
→ 放到正式武器目录
→ 替换类名、武器名、数值、播报文本
→ 在 Inspector 绑定音源、音效、射击点、子弹
→ 加入 `VideoGameManager.Weapons`
→ 手动验证拾取、射击、命中、回收、刷新
→ 追加到 `Codex/修改记录.md`
```

特殊武器：

```text
先填写 `Codex/模板-特殊武器设计卡片.md`
→ 判断是否真的适合走普通枪械模板
→ 如果不适合，单独写脚本
→ 只复用工具方法，不强行继承或套模板
```

## 模板边界

### 普通枪械模板适合

| 场景 | 说明 |
|---|---|
| 拾取后挂到角色身上 | 和标配、奥丁一致 |
| 朝当前目标发射子弹 | 使用 `otherBaseRole` 或明确目标 |
| 固定发射次数 | 如 3 发、5 发、10 发 |
| 子弹命中后造成伤害 | 在 `OnBulletHitRole` 中处理 |
| 射击结束后回收并刷新武器 | 最后调用 `SpawnWeapons()` |

### 普通枪械模板不适合

| 场景 | 建议 |
|---|---|
| 旋转近战武器 | 参考小刀，但不要继承枪械模板 |
| 陷阱或地雷 | 单独写触发器和生命周期 |
| 召唤物 | 单独管理召唤物状态和回收 |
| 持续范围伤害 | 单独处理范围检测频率和结算顺序 |
| 改变全局节奏的武器 | 先写设计卡片，再决定脚本结构 |

## 新增武器文件位置建议

| 类型 | 建议位置 |
|---|---|
| 普通可刷新武器 | `Assets/人物信息/所有武器/武器名/武器脚本.cs` |
| 子弹表现 | `Assets/人物信息/所有武器/武器名/` 或复用通用子弹 |
| 英雄专属武器 | 放在对应英雄目录，不加入普通可刷新武器模板 |
| 文档和草稿 | `Codex/` |

## 新增武器命名建议

| 项 | 建议 |
|---|---|
| 类名 | 使用英文或拼音，避免和文件名不一致 |
| Inspector 显示 | 用 `[Tooltip("中文说明")]` |
| 播报文本 | 使用中文，直接服务视频观感 |
| 方法名 | 公开方法用清楚英文名；若沿用旧项目已有拼写，先不要随手改 |
| 常量 | 和玩法强相关的值先放脚本顶部，便于集中查看 |

## 使用现有工具的建议

| 能力 | 优先使用 |
|---|---|
| 武器根物体挂载 | `VideoGameWeaponUtility.AttachRootToRole` |
| 普通枪械瞄准和翻转 | `VideoGameWeaponUtility.AimAndGetShootDirection` |
| 隐藏武器根物体 | `VideoGameWeaponUtility.HideRootAndResetRotation` |
| 隐藏通用子弹根物体 | `VideoGameWeaponUtility.HideBulletRoot` |
| 命中后播报再伤害 | `VideoGameCombatUtility.BroadcastThenDamage` |
| 命中后播报、伤害、减速 | `VideoGameCombatUtility.BroadcastThenDamageAndSetSpeed` |
| 子弹命中角色 | `VideoGameBulletBase.OnBulletHitRole` |
| 子弹命中墙体 | `VideoGameBulletBase.OnBulletHitWall` |

## 风险说明

| 风险点 | 等级 | 说明 |
|---|---|---|
| 本批只新增 `Codex/` 文档和模板 | 低 | 不参与 Unity 编译，不影响运行 |
| 模板复制后需要人工替换 | 中低 | 若类名、文件名、Inspector 绑定漏改，会影响新武器 |
| 暂不引入 `BaseWeapon` | 低 | 保持现有稳定性，后续满足条件再评估 |

## 验证结果

本批没有修改游戏代码，因此不需要运行 `dotnet build`。  
已检查新增文档和模板均位于 `Codex/`，不参与当前项目编译。
