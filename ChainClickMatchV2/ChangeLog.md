# ChangeLog

## 2026-10-14

### 新增
- `Assets/Scripts/CardGridLayout.cs`：卡牌棋盘排列组件
  - 挂载到 CardRoot 后，按 m×n（rows × columns）自动实例化卡牌 prefab 并排列成棋盘
  - 间隔 (spacingX, spacingY) 为相邻卡牌**中心间距**（世界单位）
  - 棋盘以 CardRoot 位置为中心居中，Z 统一为 0（2D 平面）
  - 运行期 Awake 自动生成；编辑器内可右键 CardRoot → CardGridLayout → 重新生成棋盘 手动重建
  - 重建时只清理本脚本生成的卡牌，不影响手动放置的子物体

### 验证
- 使用 Unity 2022.3.41f1 官方程序集（UnityEngine.CoreModule / UnityEngine + netstandard 2.1）离线编译通过：0 错误、0 警告
- 居中与间距公式抽样验算通过：4×4、spacing(1.2, 1.2) → 左上 (-1.8, 1.8)、右下 (1.8, -1.8)，相邻中心距 1.2
- 说明：Unity 编辑器当前处于打开状态，未执行 batchmode 全量导入；新脚本在编辑器重新聚焦后会自动编译，如控制台出现报错请反馈

### 更新
- `CardGridLayout.cs` 支持运行中实时调整显示：
  - 新增 `OnValidate`：Play 模式下在 Inspector 修改 rows / columns / spacingX / spacingY / cardPrefab 即实时同步棋盘
  - 数量不变时仅重排位置（快速路径，不销毁重建对象）；行/列数或 prefab 变化时自动重建
  - 新增公开方法 `RefreshBoard()`，供代码在运行时修改参数后手动同步（打包版本也可用）
  - 进入运行态时由 OnValidate 先行同步，Awake 做兜底判断，避免重复重建
- 验证：使用 Unity 2022.3.41f1 官方程序集离线编译通过（0 错误 0 警告）
- 说明：实时调整依赖编辑器 Inspector 的 OnValidate 机制，仅编辑器内（含 Play 模式）生效；打包后的正式版本需调用 `RefreshBoard()` 同步

### 更新（章节/关卡 + 卡牌种类随机）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - 新增卡牌身份字段：`chapter`（章节 1~5）、`level`（关卡）、`type`（种类 1~16）
  - `spriteRenderer` 未在 prefab 赋值时自动回退：优先取子物体 `icon` 的 SpriteRenderer，兜底取 sortingOrder 最高的 SpriteRenderer
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增公开字段 `chapter` / `level`，以及 `chapters`（章节 → 16 张种类 sprite 的配置，按 1~16 顺序）
  - 生成时给每张卡写入 SingleCard 身份，并按「章节图集 + 关卡种类池」随机分配 type 与 sprite
  - 关卡决定种类池大小，默认公式 `min(4 + level×2, 16)`（第 1 关 6 种 → 第 6 关起 16 种）
  - `RefreshBoard()` 新增章节/关卡变化检测：运行中修改章节或关卡会实时重建棋盘（重新随机种类）
- 验证：Unity 2022.3.41f1 官方程序集离线编译通过（0 错误 0 警告，两个脚本一起编译）

### 更新（点击卡牌打印种类与棋盘位置）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - 新增 `boardRow` / `boardCol`（棋盘行列号，0 起）字段
  - 新增 `OnMouseDown`：点击卡牌（依赖根物体 BoxCollider2D）时 Console 打印「种类、棋盘位置(行,列)」，如 `点击卡牌：种类=5，棋盘位置=(2,3)`
- `Assets/Scripts/CardGridLayout.cs`：生成卡牌时把行号 r / 列号 c 写入 `SingleCard.boardRow / boardCol`
- 验证：Unity 2022.3.41f1 官方程序集离线编译通过（0 错误 0 警告）

### 更新（Highlight Plus 2D 选中光效参数预设）
- `Assets/Prefabs/卡牌.prefab`：根物体 HighlightEffect2D 组件参数调整为「卡牌选中态」预设
  - 描边：金黄色 #FFD700、宽度 0.3、质量 Medium、开启边缘抗锯齿
  - 发光：强度 1.5、宽度 1.5、静止不呼吸、4 层光晕统一改为浅金黄 #FFEE88、开启边缘抗锯齿
  - 选中缩放：zoomScale = 1.08（选中时轻微放大 8%）
  - 关闭 Overlay 表面变色（overlay = 0），避免抢描边
  - 开启 Outline Exclusive（多卡相邻时描边不互相合并）
  - _highlighted 保持 0（未选中），运行时由代码控制开关
- 注意：场景内需通过菜单 GameObject → Effects → Highlight Plus 2D → Create Manager 创建 HighlightManager2D，光效才会渲染（本项需用户在 Unity 编辑器内手动完成，未改动场景文件）
- 本轮未改动 C# 脚本，无编译验证需求

### 更新（DOTween 左右摇摆抖动）
- `Assets/Scripts/卡牌/SingleCard.cs` 新增：
  - `ShakeSway(float strength, float duration = 0.5f, int vibrato = 12, float randomness = 90f)`：用 DOTween 的 `DOShakeRotation` 围绕 Z 轴左右摇摆抖动，`strength` 控制抖动力度（倾斜角度，度）
  - 只抖动 Z 轴（strength 以 Vector3(0,0,strength) 传入），2D 卡牌呈现纯左右摇摆
  - 重复触发先 `Kill()` 旧动画再播放，避免叠加；`OnDestroy` 自动清理动画
  - 保留用户新增的 HighlightPlus2D 字段与 `ChangeChooseEffect` 桩方法，未改动
- 验证：使用 Unity 2022.3.41f1 官方程序集 + 项目内 DOTween.dll 离线编译通过（0 错误 0 警告）

### 更新（待消除列表点击逻辑 + ShakeSway 复位）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - `ShakeSway` 增加守卫：`strength <= 0` 时不摇摆，直接恢复初始正着（直立）状态
  - `OnMouseDown` 不再打印，改为把点击交给 CardRoot 上的 `CardGridLayout.OnCardClicked(this)`
  - `ChangeChooseEffect` 改为 `public`（供 CardGridLayout 调用）；保留用户实现的 `ShakeSway + HighLightEffect` 组合
  - `HighLightEffect` 修正：`highlighted = strength > 0`（原 `>=0` 在 strength=0 时也会点亮光效，与“退出待消除列表”语义冲突）；补充 `highlightEffect` 空引用保护
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增待消除列表 `_pendingCards` 与 `OnCardClicked`：
    - 点击同类卡牌（或列表为空）→ 加入列表，列表内所有卡牌刷新 `ChangeChooseEffect(列表个数)`
    - 点击异类卡牌 → 旧卡牌 `ChangeChooseEffect(0)` 取消选中并恢复正着，清空列表，替换为当前点击卡牌
    - 重复点击已在列表中的同一张卡牌 → 忽略
  - 棋盘重建（ClearGenerated）时清空待消除列表
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）离线编译通过（0 错误 0 警告）

### 更新（ShakeSway 改为持续摇摆）
- `Assets/Scripts/卡牌/SingleCard.cs` 的 `ShakeSway`：
  - 由一次性 `DOShakeRotation` 改为**持续摇摆**：先摆到右侧 +strength，再 `DOLocalRotate` 在 +strength ~ -strength 之间来回摆动，`SetLoops(-1, LoopType.Yoyo)` + `Ease.InOutSine` 无限循环
  - 签名精简为 `ShakeSway(float strength, float duration = 0.5f)`（duration 为单边摆动耗时；原 vibrato/randomness 参数随 DOShakeRotation 移除）
  - 保留 `strength <= 0` 复位直立逻辑；连续调用先 Kill 旧动画再播新的
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）离线编译通过（0 错误 0 警告）

### 更新（ShakeSway 改作用 shakeTransform + 新增 InitCard 初始化）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - `ShakeSway` 不再作用于卡牌自身 `transform`，改为作用于 `shakeTransform`（用户已定义字段；未赋值时兜底用自身，避免空引用）
  - 新增 `InitCard()` 初始化方法：`HighLightEffect(0)` + `ShakeSway(0)`（关闭选中光效、恢复正着）
  - `Awake` 重构：spriteRenderer 兜底解析后统一调用 `InitCard()`（生成即初始化为干净状态）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）离线编译通过（0 错误 0 警告）

### 更新（消除倒计时 + Slider 进度显示）
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增公开字段 `countdownDuration`（默认 5 秒）与 `countdownSlider`（UnityEngine.UI.Slider，Inspector 拖入）
  - 待消除列表 **>= 2 张**时触发倒计时；列表新增一张时重置回 `countdownDuration` 秒
  - `Update` 实时把 Slider 值设为剩余时间占比（1 → 0）；未倒计时（列表不足 2 张 / 已消除 / 棋盘重建）时隐藏 Slider
  - 倒计时结束：列表中所有卡牌 `ChangeChooseEffect(0)` 并 `SetActive(false)`（消失），列表清空，停止倒计时
  - 点击异类卡牌替换列表（变 1 张）时自动停止倒计时并隐藏 Slider
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（卡牌掉落补位 + 对象池）
- 新增 `Assets/Scripts/CardPool.cs`：卡牌对象池（`Get` 取卡激活复用 / `Release` 回池禁用 / `Clear` 销毁闲置），避免频繁 Instantiate/Destroy
- `Assets/Scripts/CardGridLayout.cs`：
  - 生成棋盘改用对象池取卡；消除（倒计时归零）改为回收到对象池
  - 新增掉落补位 `FillGapsAfterElimination`：逐列处理——空位上方存活卡牌向下掉落（DOTween `DOLocalMove`，`fallDuration`/`fallEase` 可调，设为 0 则瞬移），列顶空位从对象池补新卡，种类仍在章节 sprite 数组 + 关卡种类池内随机
  - 新增槽位辅助：`GetSlotLocalPosition`（行列→坐标，与 ApplyLayout 共用）、`UpdateSlot`/`GetSlotCard`（维护棋盘引用与卡牌 boardRow/boardCol）
  - 取卡时 `InitCard()` 复位（关光效、停摇摆、恢复正着）；回池时清理残留 DOTween
  - 棋盘重建（换 prefab/章节/数量）仍整体销毁并清空对象池
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（消除后掉落跨列）
- `Assets/Scripts/CardGridLayout.cs` `ReleaseCardToPool`：卡牌回池前先把其原槽位在 `_generated` 中的引用置 null（标记空位）
- 根因：消除卡牌回池后旧槽位引用未清除；补位时该卡被池复用到其他槽位并激活后，旧槽位仍引用它且 `activeSelf=true`，后续列处理把它误判为该列存活卡并再次移动（瞬移）到旧槽位 → 卡牌跨列移动，且该列顶部漏补新卡
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（列顶补的新卡也带掉落动画）
- `Assets/Scripts/CardGridLayout.cs` `FillGapsAfterElimination`：补位的新卡不再直接 `PlaceCardAt` 无动画放置，而是先放到列顶上方一格（`GetSlotLocalPosition(-1, c)`）作为起点，再 `MoveCardToSlot(..., true)` 动画掉落到目标槽位，模仿常见卡牌游戏的新卡掉落
- 掉落动画仍由 `fallDuration` / `fallEase` 控制（设 `fallDuration = 0` 则瞬移）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（列顶新卡顺势向上堆叠，不再挤在一起）
- `Assets/Scripts/CardGridLayout.cs` `FillGapsAfterElimination`：补位的新卡起点由固定"列顶上方一格"改为按目标行依次向上堆叠——`起点行 = 目标行 - 该列补卡数`，该列所有新卡下落距离相同、保持堆叠形状一起顺势落入棋盘
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（卡牌背景 tile 的 Order In Layer 按棋盘位置排序）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - `Awake` 对新增的 `tileSpriteRenderer`（卡牌背景）做兜底查找：未赋值时自动取子物体 `Tile` 的 SpriteRenderer
  - 新增 `SetTileSortingOrder(int order)`：设置 `tileSpriteRenderer.sortingOrder`，未赋值时静默跳过
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增 `RefreshSortingOrder()`：所有卡牌背景 Order In Layer 从 **-999** 开始，按棋盘**从左到右、从上到下**（行主序）依次 **+1**
  - 在生成棋盘（`GenerateBoard`）、消除掉落补位（`FillGapsAfterElimination`）、运行中重排（`ApplyLayout`）三处调用，保证每次棋盘变化后都重新设置
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（消除计分 + 分数 Text 实时显示）
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增字段：`baseScore`（2 张基础分，默认 10）、`scoreMultiplier`（每多一张 × 倍率，默认 1.5，无上限）、`scoreText`（总分 Text，Inspector 拖入）
  - `EliminatePendingCards` 结算得分：1 张不计分；2 张 = baseScore；3 张 = ×1.5；n 张 = baseScore × 1.5^(n-2)，累计到 `_totalScore`
  - `RefreshScoreText()` 实时显示 `分数：{score}`（向下取整）；`Awake` 时初始显示 0；`scoreText` 未赋值则跳过
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（分数向下取整）
- `Assets/Scripts/CardGridLayout.cs`：改为**每次得分先向下取整再累加**——`CalculateEliminateScore` 返回 `Mathf.FloorToInt` 取整后的本次得分；`_totalScore` 改为 `int`（实际总分即整数），`RefreshScoreText` 直接显示
- 例：4 张 = 10×1.5×1.5 = 22.5 → 本次得 22；两次 22.5 分别取整 22+22 = 44
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（步数系统 + 关卡成败）
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增字段：`baseSteps`（第 1 关步数，默认 10）、`extraStepsPerLevel`（每关 + 步数，默认 2）、`baseTargetScore`（第 1 关目标分，默认 50）、`extraTargetPerLevel`（每关 + 目标分，默认 30）
  - 生成棋盘时按关卡初始化：`remainingSteps`（第 L 关 = baseSteps + extra×(L-1)）、`targetScore`（第 L 关 = baseTarget + extra×(L-1)），并清除结束状态
  - 每次消除（倒计时归零）消耗 1 步；步数耗尽时 `CheckLevelResult` 结算：总分 ≥ 目标 → 成功，否则失败；结束后锁定棋盘（不再响应点击）并停止倒计时
  - `OnCardClicked` 增加 `_gameOver` 拦截
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（三个道具：步数+3 / 同种卡呼吸提示 / 随机洗牌）
- `Assets/Scripts/卡牌/SingleCard.cs`：
  - 新增 `BreathEffect(bool on)`：对卡牌自身 scale 在 `breathMinScale`~`breathMaxScale`（默认 0.9~1.1）之间 DOTween 无限 Yoyo 呼吸；off 时 Kill 并复位 scale=1
  - `InitCard` 同步复位呼吸；`OnDestroy` Kill 呼吸 tween
- `Assets/Scripts/CardGridLayout.cs` 新增三个 public 道具方法（供 Button onClick 绑定）：
  - `UseAddStepsProp()`：当前步数 +3 并刷新步数 Text
  - `UseHintProp()`：以当前待消除列表卡牌种类为基准，棋盘上所有同种卡牌呼吸提示（需先点击一张卡牌；无基准时 Log 提示）
  - `UseShuffleProp()`：Fisher-Yates 打乱棋盘上所有存活卡牌的种类并更新 sprite（种类构成不变）；同时清空待消除列表、停止倒计时、关闭呼吸
  - `OnCardClicked` 开头 `StopAllBreathEffects()`：玩家点击任意卡牌后提示呼吸自动消失
  - 游戏结束后（`_gameOver`）三个道具均不生效
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（提示道具改为提示状态 + 道具数量系统）
- `Assets/Scripts/CardGridLayout.cs`：
  - 提示道具改为**进入提示状态**（`_hintMode`，本关有效）：提示状态下每次点击卡牌，待消除列表中的卡牌照常 HighLight + ShakeSway，棋盘上**不在列表中但同种类**的卡牌呼吸提示（`UpdateHintBreath`）；非提示状态仍为点击后清除呼吸
  - 提示道具**每关只能使用一次**（`_hintUsedThisLevel`，GenerateBoard 重置）；进入提示状态时若列表已有卡，立即以其种类刷新呼吸
  - 新增道具数量：`addStepsPropCount` / `hintPropCount` / `shufflePropCount` 均默认 **3 个**，每关开局（GenerateBoard）重置为 3；数量为 0 时拒绝使用并 Log
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（待消除列表中的卡牌不呼吸）
- `Assets/Scripts/CardGridLayout.cs` `OnCardClicked`：呼吸刷新时机从"卡牌加入列表之前"移到"加入列表之后"——修复刚点击的卡牌（已入列表）也被呼吸的问题；现在仅**同种类且不在待消除列表**的卡牌呼吸，列表中的卡牌只有 HighLight + ShakeSway
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（呼吸特效作用于 shakeTransform）
- `Assets/Scripts/卡牌/SingleCard.cs` `BreathEffect`：呼吸缩放目标由卡牌自身 transform 改为 `shakeTransform`（未赋值时兜底用卡牌自身），与 ShakeSway 使用同一目标物体；旋转（摇摆）与缩放（呼吸）可同时作用不冲突
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（消除后呼吸特效残留）
- `Assets/Scripts/CardGridLayout.cs` `EliminatePendingCards`：列表清空后新增 `StopAllBreathEffects()`——消除只关闭了被消除卡的呼吸，棋盘上其他同种卡（提示刷新）的呼吸未关闭导致残留；现在消除后所有卡牌呼吸一并清除，提示状态下下一次点击卡牌会按新种类重新刷新
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（已呼吸的卡不重启动画）
- `Assets/Scripts/卡牌/SingleCard.cs` `BreathEffect`：开启呼吸前先判断 `_breathTween.IsActive()`——若该卡已处于呼吸状态（动画仍在播放）则直接保持现状、不 Kill 不重来；仅未呼吸的卡才从头开始呼吸
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（开始界面双 Page 滑动切换）
- 新增 `Assets/Scripts/PageSwitcher.cs`：两个物理容器（pageA/pageB）轮流装载多个页面内容（pages[]），左右按钮触发 DOTween 滑动切换（当前页滑出、下一页从对应侧滑入），切换过程带移动动画
  - `SwitchNext()` / `SwitchPrev()` / `Switch(int direction)` 三个 public 方法；`leftButton`/`rightButton` 拖入后自动绑定
  - 支持循环切换（默认 `loop = true`）与边界停止（`loop = false`）；动画时长/缓动可调；`pageWidth` 可手动指定，-1 时自动取容器宽度
  - 动画进行中忽略按钮点击（防重入）；完成后容器角色互换，可持续无限翻页
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 重构（PageSwitcher 改为纯双 Page 无限滚动）
- 移除 `pages[]` 数组依赖：改为直接滑动两个 Page 容器本身（pageA/pageB 各自内含一页内容），两容器无限交替滑入滑出，视觉效果为无限翻页
- 新增 `pageCount`（默认 5，虚拟页面总数）：`CurrentIndex` 在 0..pageCount-1 内取模循环供外部读取页码，不参与滑动本身；左右按钮始终可无限切换
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（PageSwitcher 支持两页间距）
- `Assets/Scripts/PageSwitcher.cs`：新增 `pageSpacing` 字段（两个 Page 之间的间距，Inspector 设置）；滑动距离 = 容器宽度 + pageSpacing，滑入起点/滑出终点相应外移间距
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（PageSwitcher 章节边界：默认第 1 章、边界按钮消失）
- `Assets/Scripts/PageSwitcher.cs`：由无限滚动改为**有限翻页**——默认显示第 1 章（CurrentIndex=0），最大第 n 章（pageCount=n）；`pageCount` 语义改为章节总数
- 边界按钮显隐：滑到第 1 章时左按钮消失（`leftButton.gameObject.SetActive(false)`），滑到第 n 章时右按钮消失；中间页两按钮都显示；切换完成后与初始化时刷新
- 边界外点击不响应切换（Switch 中 `nextIndex < 0 || >= pageCount` 直接返回）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（PageSwitcher 标题随章节变化）
- `Assets/Scripts/PageSwitcher.cs`：新增 `titleText` 字段（Text 组件）；`UpdateTitle()` 将文本设为"第X章"（X = CurrentIndex+1），初始化与每次切换完成后刷新
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（关卡选择 Panel：开始游戏跳转 + 顺序解锁）
- 新增 `Assets/Scripts/LevelChoosePanel.cs`（挂在 LevelChoose Panel 根物体上，Panel 初始隐藏）：
  - 点击"开始游戏"按钮 → 隐藏开始界面、显示本 Panel，并按 PageSwitcher 当前章节进入对应章节的关卡选择（也可直接调用 `OpenLevelChoose(int chapter)`）
  - `levelButtons[]` 10 个普通 Button 模拟 10 个关卡；脚本自动给每个按钮绑定点击回调（带关卡序号）
  - **顺序解锁**：第 1 关永远可玩；第 k 关需第 k-1 关已通关才可点击，未解锁按钮 `interactable = false`（变灰不可点）
  - 通关存档存 PlayerPrefs（`ChainClick_Chapter{chapter}_Cleared` = 已通关关卡数，每章独立）；提供 `SetClearedLevels(int)` 供通关流程调用；`debugClearedLevels`（>0 时覆盖存档）便于预览解锁效果
  - 点击已解锁关卡：Log"开始游戏：第X章 第Y关"（TODO 留待接入 CardGridLayout 生成棋盘）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（LevelChoosePanel 初始隐藏导致"开始游戏"按钮绑定失效）
- `Assets/Scripts/LevelChoosePanel.cs`：移除 `startGameButton` 字段与 Awake 自动绑定——Panel 初始隐藏时 Awake 不执行，拖字段永远无法绑定点击（死锁）；改为在"开始游戏"按钮的 Button.onClick 中手动添加 `OpenLevelChoose()`（public 方法，UnityEvent 序列化引用在隐藏时也能调用）
- 关卡按钮（levelButtons）自动绑定保留：首次打开本 Panel（SetActive(true)）时 Awake 执行，绑定有效
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（MainPanel 脚本 + 通用 PanelManager 切换）
- 新增 `Assets/Scripts/PanelManager.cs`（通用 Panel 切换，挂常驻物体）：`panels[]` 管理所有 Panel；`SwitchTo(GameObject)` / `SwitchTo(int)` 只指定"要显示哪个"，自动隐藏列表中其余全部 Panel、显示目标——调用方不关心当前是哪个 Panel、哪个需消失；初始尊重场景预设（记录已激活 Panel，全隐藏则激活第一个）
- 新增 `Assets/Scripts/MainPanel.cs`（挂在 MainPanel，Play 默认界面）：MainPanel 激活故 Awake 必执行，`playButton`（"开始游戏"按钮）+ `panelManager` + `levelChoosePanel` 拖入后自动绑定：点击 → `panelManager.SwitchTo(levelChoosePanel)`
- `Assets/Scripts/LevelChoosePanel.cs`：新增可选字段 `panelManager` + `gamePanel`，拖入后点击已解锁关卡自动 `SwitchTo(gamePanel)`（MainPanel → LevelChoosePanel → GamePanel 三 Panel 闭环切换）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（GamePanel 脚本 + 导航完善）
- 新增 `Assets/Scripts/GamePanel.cs`（挂在 GamePanel 根物体，关卡正式玩时的 UI 管理器）：
  - `StartLevel(chapter, level)`：记录当前章节/关卡，刷新标题"第X章 第Y关"（`titleText` 可选）
  - `BackToLevelChoose()`：返回关卡选择（走 PanelManager，无则回退直接切换）
  - `backButton` 字段（GamePanel 初始激活时自动绑定；隐藏时请在 onClick 手动绑定）
- `Assets/Scripts/LevelChoosePanel.cs`：
  - `OpenLevelChoose` 优先走 PanelManager 通用切换（无 PanelManager 时回退原逻辑）
  - 新增 `BackToMainPanel()`：LevelChoosePanel 可返回 MainPanel（返回按钮请在 onClick 手动绑定）
  - 点击关卡后：切到 GamePanel 并调用 `GamePanel.StartLevel(chapter, level)` 传递章节/关卡信息
- 导航闭环：MainPanel → LevelChoosePanel → GamePanel；GamePanel → LevelChoosePanel；LevelChoosePanel → MainPanel
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（完整游戏流程：进入关卡才开局 + 通关/失败面板 + 全量初始化）
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增 `StartLevel(chapter, level)`：设置章节/关卡并全量重置（总分清零、待消除列表清空、倒计时停止、棋盘重建、步数/目标/道具/提示状态/结束状态复位）
  - 新增 `CleanupLevel()`：退出当前局时清理现场（清空待消除列表并取消效果、停倒计时、停呼吸），供返回关卡选择时调用
  - 新增事件 `OnLevelCleared` / `OnLevelFailed`：`CheckLevelResult` 结算成功/失败时触发，供 UI 显示结果面板
  - `ClearGenerated` 增加 `DOTween.Kill(card.transform)` 清理残留动画
- 新增 `Assets/Scripts/ResultPanel.cs`（通关 Panel / 失败 Panel 共用，挂各自 Panel 上）：
  - `OnNextLevelClick()`（下一关）/ `OnReplayClick()`（重玩此关）/ `OnBackClick()`（返回关卡选择）——结果 Panel 初始隐藏，按钮请在 onClick 手动绑定
- `Assets/Scripts/GamePanel.cs`：
  - 新增字段 `cardGrid`（CardRoot 上的 CardGridLayout）、`winPanel` / `losePanel`（结果面板）
  - `StartLevel` 重写：记录章节/关卡、隐藏结果面板、调用 `cardGrid.StartLevel` 正式开局（**此时才开始卡牌生成/步数/分数**）
  - 新增 `StartNextLevel()`（下一关）/ `ReplayLevel()`（重玩此关），供 ResultPanel 调用
  - Awake 订阅 `OnLevelCleared`/`OnLevelFailed`：成功 → 记录通关（`LevelChoosePanel.RecordCleared` 解锁下一关）+ 显示通关 Panel；失败 → 显示失败 Panel
  - `BackToLevelChoose` 返回前调用 `cardGrid.CleanupLevel()` 清理当前局
- `Assets/Scripts/LevelChoosePanel.cs`：新增 `RecordCleared(chapter, level)`——写入通关存档（取旧值最大值），通关第 k 关即解锁第 k+1 关
- 流程闭环：Play → MainPanel（选章节）→ 开始游戏 → LevelChoosePanel（顺序解锁关卡）→ 点关卡 → GamePanel 开局 → 通关/失败弹出结果面板（下一关/重玩/返回）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（未进入关卡不再提前生成棋盘/可点击）
- `Assets/Scripts/CardGridLayout.cs`：
  - 移除 Awake 中的自动 GenerateBoard：默认**场景加载不再生成棋盘**（新增 `autoGenerateOnStart` 开关，默认 false，仅调试用）；正式流程由进入关卡时 `StartLevel` 显式开局
  - 新增 `_runtimeEntered`（Start 后置 true）：进入 Play 模式触发的首次 OnValidate 不再刷新棋盘（之前会导致进 Play 即生成），运行中（Start 后）修改参数仍实时同步
  - 新增 `_levelActive` 未开局锁定：`OnCardClicked` 与三个道具方法在未开局时直接返回——即使棋盘上有卡牌（如调试生成）也无法点击/消除/用道具；`StartLevel` 置 true、`CleanupLevel` 置 false
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 更新（通关判定：达标立即通关 + 星级系统）
- `Assets/Scripts/CardGridLayout.cs`：
  - 通关判定改为：**达到目标分数立即通关**（无需等步数用完）；步数耗尽仍未达标才失败
  - 星级按通关时剩余步数：0 步 0 星、1 步 1 星、2 步 2 星、≥3 步 3 星；无论几星都算通关
  - `OnLevelCleared` 事件签名改为 `Action<int>`（参数为星级 0~3）；原 `CheckLevelResult` 重构为 `FinishLevel(bool success)` + `GetStars(int)`
- `Assets/Scripts/GamePanel.cs`：`HandleLevelCleared(int stars)` 记录 `LastStars`；通关仍调用 `LevelChoosePanel.RecordCleared` 解锁下一关（与星级无关）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 修复（下一关/重玩后点击卡牌报 MissingReferenceException：卡牌改回池复用、不销毁）
- 根因：HighlightPlus2D 插件 OnDestroy 会无条件销毁卡牌材质，其中共享占位材质（static dummyMaterial）被任一卡牌销毁后，新一局卡牌会引用到已销毁材质，点击（glow>0）即报错；仅在"销毁过旧卡牌的局"（下一关/重玩）出现
- 处理：**不修改外部插件**（长期规则：绝不改插件代码），改为工程内方案——`CardGridLayout.ClearGenerated` 不再 Destroy 卡牌，而是**全部回收到对象池复用**（SetActive(false)）；复用 GameObject 完全不触发插件 OnDestroy，问题绕开
- `Assets/Scripts/CardPool.cs`：新增 `Prefab` 属性（暴露池对应 prefab，用于检测 prefab 切换）
- `Assets/Scripts/CardGridLayout.cs`：
  - `ClearGenerated`：棋盘清空改为回池复用（先 EnsurePool 再逐张 ReleaseCardToPool）；保留清空待消除列表/停倒计时
  - `EnsurePool`：池缺失或 prefab 切换时才重建（清池销毁仅发生在手动改 prefab 的调试场景）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）

### 新增（GameManager + DataManager 数据持久化）
- 新增 `Assets/Scripts/DataManager.cs`（单例、常驻）：本机持久化唯一入口（**PlayerPrefs + JSON**，key `ChainClick_GameSave`）
  - 存档内容：各章节通关进度（`cleared`）+ 每关星级（`stars[10]`）+ 三个道具剩余数量（默认各 3）
  - API：`GetClearedLevels/SetClearedLevels/RecordCleared`、`GetStars/RecordStars`（取最高不降级）、`SavePropCounts`、`Load/SaveNow`
  - 说明：微信小游戏平台 PlayerPrefs 由适配层映射本地存储；后续如需原生 WX 后端只需替换本类内部实现（唯一读写点）
- 新增 `Assets/Scripts/GameManager.cs`（单例、常驻）：当前章节/关卡、最近通关星级；`RecordWin`（进度+星级持久化封装）、`SavePropCounts`（道具持久化转发）
- `Assets/Scripts/LevelChoosePanel.cs`：存档读写全部改为走 DataManager（删除直接 PlayerPrefs 与 GetClearedKey）；`debugClearedLevels` 调试覆盖保留
- `Assets/Scripts/GamePanel.cs`：开局同步 `GameManager.SetCurrentLevel`；通关改调 `GameManager.RecordWin`（不再直接调 LevelChoosePanel）
- `Assets/Scripts/CardGridLayout.cs`：开局道具数量**从存档读取（跨关保留，首次无存档默认 3）**，不再硬编码重置；三个道具使用后 `PersistPropCounts()` 写回存档
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（完成 SingleLevelUI 脚本）
- `Assets/Scripts/关卡/SingleLevelUI.cs`：
  - 文件由 GBK 编码修复为 UTF-8，乱码注释还原为中文原文
  - 字段：`chapter`/`levelDifficulty`(LevelDifficulty)/`level`/`startCount`（-2 未解锁、-1 已解锁未通关、≥0 星级）/`lockObject`/`starObject[3]`/`levelButton`；属性 `HASPASSED`/`HASUNLOCKED`
  - 完成 `Init()`：未解锁显示锁 + 按钮不可点；已通关按星级显示前 N 颗星（0 星通关不显示星）；已解锁可点击
  - 新增便捷方法：`SetState(int)`/`SetLocked()`/`SetUnlocked()`/`SetPassed(int)`
- `Assets/Scripts/GameManager.cs`：新增 `LevelDifficulty` 枚举（Normal/Hard/ExtremelyHard，用户添加）与三维解锁规则注释
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（SingleLevelUI 全参 Init + 三维解锁体系）
- `Assets/Scripts/关卡/SingleLevelUI.cs`：`Init` 改为全参 `Init(int chapter, LevelDifficulty difficulty, int level, int starCount)`（赋值 + `RefreshUI()`）；内部刷新提取为 `RefreshUI()`
- `Assets/Scripts/DataManager.cs`：星级存储扩展为**难度×关卡**（`stars[3][10]`，-1 未通关 / 0~3 星级）
  - `GetStars(chapter, difficulty, level)`：无记录返回 -1（未通关），0 星通关存 0
  - `RecordStars(chapter, difficulty, level, stars)`：取最高不降级
  - `Load` 时 `RepairStars` 修复旧存档/缺行（旧版一维星级存档自动转默认未通关，`cleared` 进度保留）
- `Assets/Scripts/GameManager.cs`：新增 `CurrentDifficulty`；`SetCurrentLevel(chapter, difficulty, level)`、`RecordWin(chapter, difficulty, level, stars)` 带难度
  - 新增三维解锁判定 `GetLevelState(chapter, difficulty, level)` → -2 未解锁 / -1 已解锁未通关 / 0~3 星级：
    章节解锁（前一章节 3 难度×10 关全 3 星）、难度解锁（上一难度同关 3 星）、关卡解锁（同难度前一关已通关）
  - `IsChapterAllThreeStars(chapter)`：章节满星判定
- `Assets/Scripts/GamePanel.cs`：`StartLevel(chapter, difficulty, level)` 带难度；`StartNextLevel`（同难度下一关）/`ReplayLevel`/`HandleLevelCleared` 均带难度
- `Assets/Scripts/LevelChoosePanel.cs`：旧版 10 按钮面板点击关卡以 Normal 难度开局（三维难度选择由新版关卡选择界面承担）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（LevelChoosePanel 改用 SingleLevelUI[] + 难度切换按钮）
- `Assets/Scripts/LevelChoosePanel.cs`：
  - `levelButtons` 类型由 `Button[]` 改为 `SingleLevelUI[]`（关卡按钮 UI 组件，含锁/星星/关卡按钮）
  - 新增 `difficultyButtons`（3 个难度按钮，按顺序 普通/困难/极难 → Normal/Hard/ExtremelyHard），Awake 自动绑定点击 → `SetDifficulty`
  - `RefreshLevelButtons`：按当前章节×难度用 `GameManager.GetLevelState` 计算状态（-2/-1/星级），对每个 SingleLevelUI 调全参 `Init(chapter, difficulty, level, state)`；`debugClearedLevels > 0` 时调试覆盖（前 N 关视为 3 星已通关）
  - `OnLevelButtonClicked`：以 `_currentDifficulty` 兜底校验解锁后调 `GamePanel.StartLevel(chapter, difficulty, level)`
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 新增（编辑器菜单：清空持久化数据）
- 新增 `Assets/Editor/ChainClickEditorTools.cs`（Editor 程序集）：
  - 顶部菜单栏新增 **ChainClickMatch → 清空持久化数据（测试）**
  - 点击弹出确认对话框（提示将重置关卡进度/星级/道具数量），确认后删除 `DataManager.SaveKey`（`ChainClick_GameSave`）并落盘，Console 打印结果
  - 防误点：取消则不执行
- 顶部菜单栏新增 **ChainClickMatch → 查看当前持久化数据**：读取存档 JSON 完整打印到 Console（可复制），弹窗预览（超长自动截断）
- 验证：运行时程序集（Assembly-CSharp）与 Editor 程序集（Assembly-CSharp-Editor，引用 UnityEditor.CoreModule）分步离线编译通过（0 错误 0 警告）

### 更新（LevelChoosePanel 默认难度 = 章节闯到的最高的关卡所在难度）
- `Assets/Scripts/GameManager.cs`：新增 `GetDefaultDifficulty(int chapter)`——从最高难度（极难）往下找第一个已解锁的难度（用该难度第 1 关是否解锁判定：难度内关卡顺序解锁，第 1 关解锁即代表已闯到该难度）；全部未解锁回退 Normal
- `Assets/Scripts/LevelChoosePanel.cs`：`OpenLevelChoose(chapter)` 打开时按 `GameManager.GetDefaultDifficulty` 设置默认难度并刷新关卡列表（如已闯到极难第 1 关则默认显示极难）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（LevelChoosePanel 每次打开都初始化：改为 OnEnable 统一刷新）
- `Assets/Scripts/LevelChoosePanel.cs`：
  - 新增 `OnEnable`：Panel 每次变为可见（SetActive(true)）时统一执行——章节（PageSwitcher 优先）→ 默认难度（`GameManager.GetDefaultDifficulty`）→ `RefreshLevelButtons`
  - 覆盖所有打开路径：从"开始游戏"进入、从 GamePanel/通关/失败面板返回，都会重新计算默认难度与解锁状态
  - `OpenLevelChoose(chapter)` 精简为只负责章节设置 + 切换显示（激活后由 OnEnable 完成初始化）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 修复（星级无法持久化：JsonUtility 不支持交错数组，stars 被静默丢弃）
- 现象：通关多关（三星）后退出 Play 重进，LevelChoose 仍只有第 1 关解锁——星级记录未写入存档
- 根因：`ChapterSave.stars` 用的是 `int[3][]`（交错数组），**Unity JsonUtility 不支持交错数组序列化**，`SaveNow()` 里 `JsonUtility.ToJson` 会把 stars 字段静默丢弃（不报错）——存档 JSON 里永远没有星级；运行中内存有值所以同会话正常，重进 Play 后 Load 读回无 stars → 全部回退 -1
- 修复（`Assets/Scripts/DataManager.cs`）：星级改为**一维平铺 `int[30]`**（索引 = 难度×10 + 关卡-1，JsonUtility 支持），`GetStars/RecordStars/RepairStars` 同步改为平铺索引；旧存档（无 stars / 长度不符）Load 时自动重建为全 -1
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）；索引范围人工验算（0~29 覆盖 3 难度×10 关）

### 修复（星级存档初始化错误：未通关误存为 0，导致 state 全为 0 / 默认难度错乱）
- 现象：通关多关（日志确认 3 星）后重进 Play，关卡选择里所有关卡 state 显示 0、默认难度异常、部分关卡未解锁
- 根因：`ChapterSave.stars` 字段初始化 `new int[30]` 默认全是 **0**，而"未通关"语义应为 **-1**；`RepairStars` 未把初始 0 填成 -1 → 未通关格子以 0 存档。由于 0 恰是"0 星通关"的合法值，`GetLevelState` 把未通关关卡误判为 0 星已通关（state 返回 0），`GetDefaultDifficulty` 也被误导（把未解锁的极难第 1 关当成已解锁 → 默认难度错乱）
- 修复（`Assets/Scripts/DataManager.cs`）：
  - `ChapterSave.stars` 默认改为 `null`，由 `RepairStars` 统一初始化为全 -1（不再依赖 `new int[30]` 的默认 0）
  - `RepairStars` 新增兼容迁移：null / 长度不符 / **全 0 数组** → 重建为全 -1（未通关）；非全 0 数组只做非法值校正（-1 与 0~3 保留，严格区分"未通关"与"0 星通关"）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 修复（默认难度误判：只在普通通关却默认显示困难，第 1 关 0 星、其余未解锁）
- 现象：存档正确（Normal 1-4 = 3 星、其余 -1）重进 Play，关卡选择默认难度却是困难——第 1 关"解锁 0 星"、第 2~10 关全部未解锁
- 根因：`GameManager.GetDefaultDifficulty` 用"该难度第 1 关**已解锁**"判断是否闯到该难度；而 Hard 第 1 关解锁只需 Normal 第 1 关 3 星（难度解锁规则），**不代表玩过 Hard**——只在普通通关 4 关的存档，默认难度被错误判成 Hard，展示的正是"Hard 1 已解锁未通关（0 星）、Hard 2-10 未解锁"
- 修复（`Assets/Scripts/GameManager.cs`）：`GetDefaultDifficulty` 改为从极难往下找第一个**该难度第 1 关已通关（星级 ≥ 0）**的难度；仅"解锁未玩"的难度不算闯到；全部未通关回退普通
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 修复（返回关卡选择后棋盘残留：关卡 2D 物体未随关卡结束消失）
- 现象：通关/失败后点"返回"回到关卡选择，CardRoot 下的整副卡牌棋盘依然显示（CardRoot 在场景根、不在 GamePanel 下，PanelManager 隐藏 GamePanel 不影响它）
- 根因：`CardGridLayout.CleanupLevel()` 只复位逻辑状态（`_levelActive`/待消除/倒计时/呼吸/提示），未清空棋盘——卡牌仍挂在 CardRoot 下可见
- 修复（`Assets/Scripts/CardGridLayout.cs`）：`CleanupLevel()` 开头调用 `ClearGenerated()`——整副棋盘逐张 `ReleaseCardToPool` 回池（`SetActive(false)`、`InitCard` 复位光效/摇摆/旋转、**不销毁**，规避插件 OnDestroy 连坐材质问题），内部同时清空待消除列表并停止倒计时；并复位 `_gameOver = false`（双保险）。步数/分数/目标/道具数量由下次 `StartLevel → GenerateBoard` 全量重置（既有逻辑，无需改）
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 修复（通关/失败面板"返回"未清理棋盘：OnBackClick 只切 Panel 不清场）
- 现象：通关/失败后点"返回"回到关卡选择，CardRoot 下卡牌棋盘依然存在、与关卡选择 UI 重叠显示
- 根因：通关/失败面板"返回"按钮绑定的是 `ResultPanel.OnBackClick()`——它只 `PanelManager.SwitchTo(levelChoosePanel)` 切面板，**从未调用 `GamePanel.BackToLevelChoose()`/`cardGrid.CleanupLevel()`**，因此上一轮加入的棋盘回池逻辑在这条路径上根本没被触发
- 修复（`Assets/Scripts/ResultPanel.cs`）：`OnBackClick()` 优先走 `gamePanel.BackToLevelChoose()`（内部已包含 `cardGrid.CleanupLevel()`：棋盘全部回池、停止倒计时/呼吸、复位 `_levelActive`/`_gameOver`，再切换 Panel）；`gamePanel` 为空时保留原兜底切换
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 新增（AudioManager：字符串播放音频 + AudioLibrary 映射表 + 独立音量/静音持久化）
- 新增 `Assets/Scripts/Audio/AudioLibrary.cs`（ScriptableObject）：预先配置"字符串 ↔ AudioClip"映射（CreateAssetMenu → ChainClick/Audio Library 创建资产），`GetClip(key)` 未配置时 LogWarning 提示
- 新增 `Assets/Scripts/Audio/AudioManager.cs`（单例，DontDestroyOnLoad）：
  - 字符串播放：`PlayBGM("BGM_关卡")`（同曲不重播、循环）/ `StopBGM()` / `PlaySFX("卡牌点击")` / `PlaySFX(key, volumeScale, pitch)`
  - BGM/SFX 分离：MusicSource（循环）+ SFX 轮转播放器（默认 4 个，常见音效不互相打断）
  - **音乐音量/音乐静音、音效音量/音效静音独立控制**（无全局静音）；音乐静音用 AudioSource.mute（保留进度），音效静音时 PlaySFX 直接不播
  - 音量/静音启动时从 DataManager 读取、修改时写回 DataManager 并落盘（符合"所有存档读写走 DataManager"规则，微信小游戏适配随存档）
- 修改 `Assets/Scripts/DataManager.cs`：`SaveData` 新增 `musicVolume`(0.8)/`sfxVolume`(1)/`musicMuted`/`sfxMuted`（旧存档无字段时回退默认值，兼容）；新增 `MusicVolume/SfxVolume/MusicMuted/SfxMuted` 属性与 `SetAudioSettings(...)`（内部 SaveNow）
- 使用：场景常驻物体挂 AudioManager → 拖入 AudioLibrary 资产（默认 BGM key 可选）；代码用字符串播放即可
- 验证：Unity 2022.3.41f1 官方程序集（含 UnityEngine.AudioModule.dll）+ DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（AudioManager 增加微信小游戏震动：轻/中/强三档）
- 新增三个 public 方法：`VibrateLight()`（wx.vibrateShort type=light）、`VibrateMedium()`（type=medium）、`VibrateStrong()`（wx.vibrateLong，400ms 长震、全机型震感最强）
- 实现：SDK（WX-WASM-SDK-V2）的 `WX.VibrateShort(VibrateShortOption)` 中 Option 类型在插件 dll 内，工程脚本不引入编译期依赖——在 `UNITY_WEBGL && !UNITY_EDITOR`（微信小游戏构建）下运行时**反射**查找 `WeChatWASM.WX` / `VibrateShortOption` / `VibrateLongOption` 并调用；编辑器/其他平台仅打日志模拟、不报错
- 验证：Unity 2022.3.41f1 官方程序集（含 UnityEngine.AudioModule.dll）+ DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（音频设置面板：三个 Toggle 数据驱动 + 震动开关持久化）
- 新增 `Assets/Scripts/Audio/AudioSettingsPanel.cs`：`musicToggle`/`sfxToggle`/`vibrateToggle` 三个 Toggle
  - **数据驱动**：OnEnable 每次显示时从 AudioManager（内部读 DataManager 存档）刷新 `isOn`——音乐/音效 Toggle 与"静音"取反（isOn=true 表示声音开启），震动 Toggle 直读开关；`_refreshing` 标志防止刷新反向触发写回
  - **点击写入**：onValueChanged 自动绑定 → `SetMusicMuted(!isOn)` / `SetSfxMuted(!isOn)` / `SetVibrateEnabled(isOn)`，由 AudioManager 持久化到 DataManager
- 修改 `Assets/Scripts/DataManager.cs`：`SaveData` 新增 `vibrateEnabled`（默认 true）；`SetAudioSettings` 增加 `vibrateEnabled` 参数；新增 `VibrateEnabled` 属性
- 修改 `Assets/Scripts/Audio/AudioManager.cs`：新增 `_vibrateEnabled` 字段与 `VibrateEnabled` 属性、`SetVibrateEnabled(bool)`；`VibrateLight/Medium/Strong` 在震动开关关闭时直接 return 不震动
- 验证：Unity 2022.3.41f1 官方程序集（含 UnityEngine.AudioModule.dll）+ DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll + JSONSerializeModule 离线编译通过（0 错误 0 警告）

### 更新（步数 Text / 目标分数 Text 关联显示）
- `Assets/Scripts/CardGridLayout.cs`：
  - 新增字段 `stepsText`（显示 `步数：{remainingSteps}`）、`targetText`（显示 `目标：{targetScore}`），Inspector 拖入，未赋值自动跳过
  - `Awake` 初始刷新；`GenerateBoard` 关卡开局刷新；`EliminatePendingCards` 消耗步数后刷新步数
- 验证：Unity 2022.3.41f1 官方程序集 + DOTween.dll + HighlightPlus2D（桩类型）+ UnityEngine.UI.dll 离线编译通过（0 错误 0 警告）
