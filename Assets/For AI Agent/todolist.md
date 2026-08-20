# Todo List

## 2026-05-24

- [ ] 修复相机相册 1:1 裁剪问题：`thumbPrefab` UI 框改为 16:9，消除检测范围与显示范围不一致
- [x] 手账画布持久化：为 `ScrapBookDataManager` 补充 JSON 存档逻辑，保存元素位置、角度、缩放（待 Unity 实测）
  - [x] Step 1 — 新建 `ScrapBookElementMeta.cs`，存 type + id
  - [x] Step 2 — `ScrapBookTextBox.cs` 加 `Text` 只读属性
  - [x] Step 3 — `ScrapBookCanvas.cs` 三个创建方法各自挂 Meta，AddPhotoElement 加 fileName 参数
  - [x] Step 4 — `ScrapBookDraggable.cs` OnEndDrag 传 fileName 给 AddPhotoElement
  - [x] Step 5 — `ScrapBookDataManager.cs` 加数据模型 + SavePhotos / LoadPhotos / SaveCanvas / LoadCanvas
  - [x] Step 6 — `ScrapBookUIManager.cs` OnDisable 存盘，OnEnable 读盘重建

- [x] 回响系统第 0 步（共享基础）：拍照时把触发的 Echo 数据绑定到照片 fileName
  - [x] Step 0.1 — 新建 Echo 存储（fileName → List\<string\> scrapbookContent），JSON 存盘，参考 `CameraPhotoStorage`
  - [x] Step 0.2 — `AddPhotoFromTexture` 返回生成的 fileName，让拍照流程能拿到它
  - [x] Step 0.3 — `CameraSystemManager.CaptureFlow` 收集 captureSnapshot 各 `EchoTrigger.scrapbookContent`，按 fileName 写入存储

- [x] 回响系统第 2 层（照片标识 / 手账文字呈现）：带 Echo 的照片在相册与手账 topbar 中高亮
  - [x] Step 2.1 — 定方案：只读文本块、作为照片子节点、从 EchoPhotoStorage 派生、一侧竖排选边避越界
  - [x] Step 2.2 — `ScrapBookCanvas.AddPhotoElement` 生成只读 Echo 文本块（拖入/重建两条路径自动覆盖）
  - [x] Step 2.3 — 派生方案下无需单独存档：标签是照片子节点，重建照片时自动重生成
  - [x] Step 2.4 — 工具栏照片条目：有 Echo 则加发光描边提示（Outline 组件）
  - [x] Step 2.5 — 相册照片条目：有 Echo 则加高亮提示
  - [x] Step 2.6 — 明确手账 Echo 文本定位：照片拖入手账后显示 `EchoTrigger.scrapbookContent`，作为主角手账内容；爷爷日记便签只承载客观线索连接，不替代手账内容

- [ ] 回响系统第 3 层（日记便签）：爷爷日记 Tab 支持固定便签位
  - [x] Step 3.1 — 设计日记页/便签数据结构：日记页ID、爷爷原文、固定便签位ID、触发来源 EchoTrigger、便签内容、解锁状态
  - [x] Step 3.2 — 新建「已解锁便签」存储，JSON 存盘，记录哪些便签位已出现
  - [x] Step 3.3 — 拍照时登记 Echo 对应的便签位，而不是只汇总 scrapbookContent 文本
  - [x] Step 3.4 — `PlayerPanelUIManager.ShowGrandpaDiary` 渲染爷爷原文 + 已解锁便签；主角文字只显示在便签上，不写进爷爷旧日记本体
  - [x] Step 3.5 — 便签交互方案：优先静态贴纸；鼠标悬浮便签时降低透明度，移开恢复，显示便签下方老日记内容
  - [x] Step 3.6 — 配置第一关值班记录本：拍摄后解锁靠近“厂北山路 / 山顶灯光”段落的固定便签

- [ ] 日记字体配置
  - [ ] 爷爷旧日记字体：使用参考图方向的旧时代手写感字体（需确认授权后导入/替换）
  - [ ] 主角便签字体：选择稍现代、简洁、可读的字体，用于便签内容
  - [ ] 在爷爷日记 Tab 中分别绑定两套 TMP Font Asset

- [ ] 爷爷日记翻页功能
  - [x] Step N1 — 设计日记页数据结构：pageId、pageIndex、爷爷原文、页标题/日期（可选）、该页包含的固定便签位列表
  - [x] Step N2 — 新建 JSON 内容配置与解析：UI 上预设多个带 markerId 的 TextArea，JSON 中按 markerId 写内容，游戏开始时分发到对应 TextArea
  - [x] Step N3 — 在 `PlayerPanelUIManager` 中加入当前页索引、上一页/下一页按钮引用、页码文本引用
  - [x] Step N4 — 实现 `ShowGrandpaDiaryPage(index)`：刷新当前页爷爷原文、页码、翻页按钮可用状态
  - [x] Step N5 — 打开爷爷日记 Tab 时默认显示第一页，或显示最近一次停留的页（当前实现：本次会话内保留上次页，初始为第一页）
  - [ ] Step N6 — UI 配置：在 `GrandpaDiaryTab` 下放置日记正文 TMP、上一页按钮、下一页按钮、页码 TMP
  - [x] Step N7 — 与日记便签系统预留接口：每页可以查询并显示该页已解锁的便签位；未做便签前先只显示爷爷原文
  - [ ] Step N8 — 测试：第一页时上一页不可用，最后一页时下一页不可用；切换 Tab 后页内容不串；关闭/打开玩家面板后页状态符合 Step N5 选择

- [ ] 回响收集进度提示
  - [ ] 为每个关卡配置回响物品总数
  - [ ] 统计当前关卡已收集 / 未收集回响数量
  - [ ] 在合适 UI 中清晰提示“当前关卡还有多少个回响物品未收集”

- [ ] 回响系统第 1 层（即时反馈）小修
  - [ ] `HandleConfirmedHighlights` 的 `return` 改为遍历全部 Echo
  - [x] `EchoPopupUI` 补自动隐藏
  - [x] 右上角提示改为统一 `Popup Message`，不直接显示 `EchoTrigger.scrapbookContent`

- [x] 内容：把值班记录本触发的便签文字定稿，确认便签位，填进 story-flow.md，并给出场景 `EchoTrigger.scrapbookContent` 配置文本

---

## Unity 配置检查清单（测试当前已完成步骤）

> 用途：确认 todolist 中已完成的手账持久化、Echo 绑定照片、手账 Echo 呈现、工具栏 Echo 描边是否在 Unity 场景里配置到位。

### 1. 场景基础对象

- [ ] 场景中有且只有一个 `ScrapBookDataManager` 组件对象
  - 作用：管理手账照片列表、装饰列表、画布布局 JSON 存档
  - Inspector：无需要拖拽的字段

- [ ] 场景中有 `CameraSystemManager`
  - [ ] `Camera UI Root` → 拖相机 UI 根节点
  - [ ] `Camera Tweaking UI Manager` → 拖 `CameraTweakUIManager`
  - [ ] `Camera Photo Preview UI Manager` → 拖 `CameraPhotoPreviewUIManager`
  - [ ] `Camera Photo Album UI Manager` → 拖 `CameraPhotoAlbumUIManager`
  - [ ] `Camera Photo Capture` → 拖 `CameraPhotoCapture`
  - [ ] `Highlight Object Detector` → 拖 `CameraHighlightObjectDetector`

- [ ] 场景中有 `EchoSystemManager`
  - [ ] `Camera System Manager` → 拖同场景的 `CameraSystemManager`
  - [ ] `Echo Popup UI` → 拖 `EchoPopupUI`

- [ ] 场景中有 `EchoPopupUI`
  - [ ] `Popup Root` → 拖回响弹窗根节点
  - [ ] `Content Text` → 拖弹窗里显示 Echo 内容的 TMP 文本
  - [ ] `Popup Message` → 统一右上角提示文案，例如“发现新的回响”；不显示 `EchoTrigger.scrapbookContent` 正文

### 2. 手账 UI

- [ ] 手账 UI 根节点上有 `ScrapBookUIManager`
  - [ ] `Photo Category Btn` → 拖照片分类按钮
  - [ ] `Deco Category Btn` → 拖装饰分类按钮
  - [ ] `Element Content` → 拖手账顶部工具栏 / Scroll Content 节点
  - [ ] `Scrap Book Canvas` → 拖放置照片/贴纸/文本框的 `ScrapBookCanvas`
  - [ ] `Deco Presets` → 拖入所有贴纸和文本框的 `ScrapBookDecoPreset`
  - [ ] `Photo Element Size` → 确认工具栏照片条目尺寸
  - [ ] `Echo Glow Color` / `Echo Glow Thickness` → 确认有 Echo 的工具栏照片描边颜色和粗细

- [ ] 手账画布区域上有 `ScrapBookCanvas`
  - [ ] `Text Box Prefab` → 如使用文本框 prefab，拖对应 prefab；为空时会走代码生成的临时文本框
  - [ ] `Default Photo Size` → 确认照片拖入画布后的默认尺寸
  - [ ] `Echo Label Gap` / `Echo Label Width` / `Echo Label Font Size` / `Echo Label Color` → 确认照片周围 Echo 文字样式

- [ ] 如果使用 `Text Box Prefab`
  - [ ] prefab 根节点有 `ScrapBookTextBox`
  - [ ] `Input Field` → 拖 prefab 内的 `TMP_InputField`
  - [ ] `Display Text` → 拖 prefab 内常态显示文字的 TMP 文本

- [ ] 每个 `ScrapBookDecoPreset` 配置完整
  - [ ] `Preset Name` 唯一，不能空
  - [ ] `Deco Type` 正确选择 `Sticker` 或 `TextBox`
  - [ ] `Icon` 已拖入工具栏显示用图
  - [ ] `Default Size` 合理

### 3. Echo 物品

- [ ] 需要触发回响的 B 类物品上有 `CameraHighlightObject`
  - 作用：让相机高亮检测系统能识别该物体

- [ ] 同一个 B 类物品上有 `EchoTrigger`
  - [ ] `Diary Content` 填入当前测试用 Echo 文字

- [ ] 相机取景/拍照时该物品能进入 `CameraHighlightObjectDetector.HighlightedObjects`
  - 作用：`CameraSystemManager` 会用检测结果把 Echo 写入 `EchoPhotoStorage`

### 4. 当前已完成步骤测试流程

- [ ] 测试 Step 0：拍摄带 `EchoTrigger` 的物品后，照片能保存，并在 `Application.persistentDataPath/Echo/EchoPhotos.json` 里写入该照片 fileName 对应的 Echo 文本

- [ ] 测试 Step 2.2：把这张有 Echo 的照片从手账工具栏拖入画布后，照片周围自动出现只读 `scrapbookContent` 文本块；它代表主角手账内容，不是爷爷日记便签内容

- [ ] 测试 Step 2.3：关闭/重新打开手账面板后，已放置照片能从存档重建，Echo 文本块随照片自动重新生成

- [ ] 测试 Step 2.4：手账工具栏照片列表中，有 Echo 的照片条目出现 `Outline` 发光描边；无 Echo 的照片没有描边

- [ ] 测试 Step 2.5：相册照片列表中，有 Echo 的照片条目出现 `Outline` 发光描边；无 Echo 的照片没有描边

- [ ] 测试手账持久化：移动、旋转、缩放照片/贴纸/文本框后关闭手账，再打开时位置、角度、缩放恢复

### 5. 爷爷日记翻页 UI 配置

- [ ] `PlayerPanelUIManager` 的 Grandpa's Diary 区域
  - [ ] `Grandpa Diary Json` → 拖 `Assets/For AI Agent/grandpa-diary-content.json` 测试内容；拖了它以后会优先使用文件内容生成页数据
  - [ ] `Grandpa Diary Pages` → 可留空；只有不使用 `Grandpa Diary Json` 时才需要手动配置每页 `pageId`、`pageIndex`、`pageTitle`、`textBlocks`
  - [ ] `Grandpa Diary Page Views` → 拖入每个日记页根节点上的 `GrandpaDiaryPageView`
  - [ ] `Grandpa Diary Prev Page Button` → 拖上一页按钮
  - [ ] `Grandpa Diary Next Page Button` → 拖下一页按钮
  - [ ] `Grandpa Diary Page Text` → 拖页码 TMP 文本

- [ ] 每个日记页根节点
  - [ ] 挂 `GrandpaDiaryPageView`
  - [ ] `Page Id` 与 JSON 中的 `pageId` 一致，例如 `p01`
  - [ ] `Page Root` 可拖本页根节点；不拖则默认用当前 GameObject
  - [ ] `Text Areas` 可手动拖，也可留空让代码自动抓子物体里的 `GrandpaDiaryTextAreaBinding`

- [ ] 每个日记正文 TextArea
  - [ ] 挂 `GrandpaDiaryTextAreaBinding`
  - [ ] `Marker Id` 与该页 `textBlocks.markerId` 一致，例如 `p01_body_01`
  - [ ] `Target Text` 拖该 TextArea 的 TMP 文本；不拖则默认用当前 GameObject 上的 TMP_Text

- [ ] 测试爷爷日记翻页
  - [ ] 打开爷爷日记 Tab 默认显示第一页
  - [ ] 点下一页后显示下一页，并按 marker 填入正确文本
  - [ ] 第一页上一页按钮不可点，最后一页下一页按钮不可点
  - [ ] 页码显示正确，例如 `1 / 3`
