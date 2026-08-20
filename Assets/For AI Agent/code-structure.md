# 代码结构

## 技术栈

- 引擎：Unity（URP）
- 地形系统：MapMagic
- 摄像机系统：Cinemachine
- 渲染：Universal Render Pipeline

## 文件结构

所有自定义系统均位于 `Assets/Scripts/In-Game Systems/`，按系统模块划分：

### CameraSystem（拍摄系统）
- `CameraStateMachine.cs` / `CameraModeState.cs` — 相机状态机，管理拍摄模式切换
- `CameraSystemManager.cs` — 相机系统总入口
- `CameraCapture.cs` — 拍照逻辑，截图捕获（全屏 ReadPixels，存盘为 1920×1080）
- `CameraHighlightObject.cs` / `CameraHighlightObjectDetector.cs` — 可拍摄物体的高亮显示（单纯视觉高亮，不涉及灵感触发逻辑）
- `CameraRaiseAnimationHandler.cs` — 举起相机的动画控制
- `CameraPhotoStorage.cs` — 照片磁盘存储，JSON manifest 记录文件名列表，存于 Application.persistentDataPath
- `CameraPhotoAlbumUIManager.cs` / `CameraPhotoPreviewUIManager.cs` / `CameraPhotoThumbItem.cs` — 相册与预览 UI
- `CameraTweakUIManager.cs` — 相机参数调节 UI
- `AlbumPhotoContextMenu.cs` — 相册照片右键菜单

### CharacterMovementSystem（角色移动系统）
- `PlayerLocomotionInput.cs` — 输入读取
- `PlayerMovementManager.cs` — 移动逻辑管理
- `PlayerFootstepAudio.cs` — 脚步音效
- `PlayerControls.cs` — Input System 生成的输入映射

### InputSystem（输入路由）
- `PhotoModeInputRouter.cs` — 拍照模式下的输入分发
- `PlayerPanelInputRouter.cs` — 玩家面板模式下的输入分发

### InspectObjectSystem（物品检视系统）
- `InspectSystemManager.cs` — 检视系统总控
- `InspectableItem.cs` — 可检视物品的数据组件
- `InspectRotator.cs` — 3D 物品旋转交互
- `InspectUIManager.cs` — 检视界面 UI

### PlayerPanel / ItemBackpack（玩家面板和背包）
- `PlayerPanelUIManager.cs` — 玩家面板总 UI
- `ItemBackpackManager.cs` — 背包数据管理
- `ItemBackpackSlot.cs` — 背包格子逻辑
- `ItemBackpackUIManager.cs` — 背包 UI

### ScrapBookSystem（手账系统）
- `ScrapBookDataManager.cs` — 手账数据管理（记录已加入手账的照片和装饰ID；仅内存存储，当前无磁盘持久化）
- `ScrapBookCanvas.cs` — 手账画布管理，接收放置请求，实例化元素 GameObject
- `ScrapBookElement.cs` — 挂在画布上已放置元素上，提供四种交互：左键拖拽移动位置、右键拖拽旋转、滚轮缩放（范围 0.3x ~ 3x）、点击置顶（SetAsLastSibling）；内置 ClampToCanvas 限制元素不能拖出 ScrapBookCanvas 边界
- `ScrapBookDraggable.cs` — 挂在工具栏条目上（照片、贴纸、文本框），从工具栏拖入画布；拖动时生成半透明 ghost，松手后调用 ScrapBookCanvas 放置元素，使命结束
- `ScrapBookTextBox.cs` — 文字输入框
- `ScrapBookDecoPreset.cs` — 装饰元素预设（ScriptableObject，存名称、类型、图标、默认尺寸）
- `ScrapBookUIManager.cs` — 手账 UI 总控

### EchoSystem（回响系统）
- `EchoTrigger.cs` — 挂在B类物品上的数据组件，定义拍摄后触发的爷爷日记内容
- `EchoSystemManager.cs` — 监听拍照事件，检测画面内带EchoTrigger的物体，触发回响UI
- `EchoPopupUI.cs` — 回响弹出UI，显示"发现新回响"标题和日记内容，独立于PlayerPanel
- `ItemUseTrigger.cs` — 通用道具使用触发点，玩家背包带指定物品时按F触发UnityEvent，支持任意交互（翻墙、开门等），通过Inspector配置 `requiredItemName` 和 `onUse` 事件

### GameSystem
- `GameFlowManager.cs` — 全局游戏流程管理（暂未实现）
