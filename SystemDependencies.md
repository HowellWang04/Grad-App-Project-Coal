# System Dependencies

各系统内 C# 文件的依赖与调用关系。按系统分节，新系统直接追加。

---

## ScrapBook System

### 文件职责速览

| 文件 | 类型 | 职责 |
|------|------|------|
| `ScrapBookDecoPreset.cs` | ScriptableObject | 装饰物数据：名称、类型（Sticker/TextBox）、图标、默认尺寸 |
| `ScrapBookDraggable.cs` | MonoBehaviour | 工具栏条目，从工具栏拖入画布，生成 ghost，落点合法时调用 Canvas |
| `ScrapBookCanvas.cs` | MonoBehaviour | 画布管理器，接收放置请求，实例化元素 GameObject，挂载 Element 组件 |
| `ScrapBookElement.cs` | MonoBehaviour | 已放置元素的交互：左键移动、右键旋转、滚轮缩放、点击置顶 |
| `ScrapBookTextBox.cs` | MonoBehaviour | 文本框内容管理：显示/编辑切换，双击编辑，失焦保存 |
| `ScrapBookUIManager.cs` | MonoBehaviour | 手账 UI 总控，管理工具栏按钮与面板开关 |
| `ScrapBookDataManager.cs` | MonoBehaviour | 手账数据持久化（存档/读档） |

### 调用时序

```
[工具栏]
ScrapBookDecoPreset（ScriptableObject — 纯数据）
        ↓ Inspector 配置到
ScrapBookDraggable（挂在工具栏每个条目上）
        │
        │ OnBeginDrag → 生成半透明 ghost 跟随鼠标
        │ OnEndDrag   → 判断落点是否在 ScrapBookCanvas 内
        │
        ├─ 照片条目（SetupPhoto 初始化）─────→ ScrapBookCanvas.AddPhotoElement(tex, pos)
        │                                              ↓ CreateElementBase()
        │                                              ↓ 挂 RawImage + ScrapBookElement.Init()
        │
        └─ 装饰条目（Setup(DecoPreset) 初始化）→ ScrapBookCanvas.AddDecoElement(preset, pos)
                                                       │
                                          ┌────────────┴────────────┐
                                   Sticker（贴纸）            TextBox（文本框）
                                   AddStickerElement()        AddTextBoxElement()
                                   挂 Image                   挂 ScrapBookTextBox
                                   + ScrapBookElement         + ScrapBookElement

[画布上]
ScrapBookElement（挂在每个已放置元素上）
        左键拖拽 → 移动（ClampToCanvas 限边界）
        右键拖拽 → 旋转
        滚轮     → 缩放（0.3x ~ 3x）
        点击     → SetAsLastSibling（置顶）

ScrapBookTextBox（仅文本框元素）
        双击 → 切换到 InputField 编辑模式
        失焦 → 写回 DisplayText，隐藏 InputField
```

### 关键依赖边

```
ScrapBookDraggable  →  ScrapBookDecoPreset   （读取 icon、type 构建 ghost）
ScrapBookDraggable  →  ScrapBookCanvas        （调用 AddPhotoElement / AddDecoElement）
ScrapBookCanvas     →  ScrapBookDecoPreset    （读取 decoType、icon、defaultSize 创建元素）
ScrapBookCanvas     →  ScrapBookElement       （AddComponent + Init(rect)）
ScrapBookCanvas     →  ScrapBookTextBox       （AddComponent + Init / InitRefs）
ScrapBookElement    →  ScrapBookCanvas        （仅通过 Init 接收 RectTransform，无反向调用）
```

### 注意事项

- `ScrapBookDraggable` 拖入完成后使命结束，不持有对 `ScrapBookElement` 的引用
- `ScrapBookCanvas.CreateElementBase()` 是照片和贴纸的公共底层，TextBox 单独走 `AddTextBoxElement()`（因为需要额外的 TextBox 组件）
- TextBox 有两种创建路径：prefab（`textBoxPrefab` 不为 null）和纯代码构建（`CreateTextBoxFromCode`），两者最终都调用 `element.Init(rect)` 和 `textBox.InitRefs()`

---

<!-- 新系统从这里继续追加 -->
