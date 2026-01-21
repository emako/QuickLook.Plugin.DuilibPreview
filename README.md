![QuickLook icon](https://user-images.githubusercontent.com/1687847/29485863-8cd61b7c-84e2-11e7-97d5-eacc2ba10d28.png)

# QuickLook.Plugin.DuilibPreview

这是一个 [QuickLook](https://github.com/QL-Win/QuickLook) 插件，用于直接预览 [Duilib](https://github.com/duilib/duilib) 界面布局文件 (`.xml`)。

通过此插件，你无需启动 IDE 或专用预览工具，只需在 Windows 资源管理器中选中 XML 文件并按下空格键，即可查看 Duilib 界面的实际渲染效果。

## ✨ 功能特性

*   **快速预览**: 选中包含 Duilib 布局的 XML 文件，按空格键即刻预览。
*   **智能识别**: 插件会自动检查 XML 内容，仅对包含 `<Window>` 或 `<Global>` 等 Duilib 特征标签的文件生效，不影响普通 XML 文件的查看。
*   **原生精确**: 基于内置的改进版 Duilib 核心库渲染，确保预览效果与实际程序运行效果高度一致。
*   **交互支持**: 预览窗口支持基本的交互，方便查看控件状态。

## 📂 项目结构

本项目主要由以下三个部分组成：

1.  **DuiLib**
    *   位于 `DuiLib/` 目录。
    *   这是一个基于官方 Duilib 修改的分支，包含了一系列 Bug 修复和功能增强。
    *   详细修改日志请参考 [DUILIB.md](DUILIB.md)。

2.  **DuilibPreview** (Native)
    *   位于 `DuilibPreview/` 目录。
    *   C++ 编写的原生模块，负责实际加载 XML 并创建 Duilib 窗口。
    *   生成 `DuilibPreview.dll` 供插件调用。

3.  **QuickLook.Plugin.DuilibPreview** (Plugin)
    *   位于 `QuickLook.Plugin.DuilibPreview/` 目录。
    *   C# 编写的 QuickLook 插件封装层。
    *   实现了 `IViewer` 接口，通过 `HwndHost` 托管原生的 Duilib 预览窗口。

## 🛠️ 构建指南

如果你希望自己编译本项目：

1.  **环境要求**: 
    *   Visual Studio (推荐 2022 或更新版本)
    *   C++ 桌面开发工作负载
    *   .NET 桌面开发工作负载

2.  **编译步骤**:
    *   打开解决方案文件 (如 `DuiLib.slnx` 或对应的 `.sln`)。
    *   确保编译配置为 `Release`。
    *   首先编译 `DuiLib` 和 `DuilibPreview` 项目，生成原生 DLL。
    *   编译 `QuickLook.Plugin.DuilibPreview` 项目。
    
3.  **打包**:
    *   可以参考 `Scripts/` 目录下的打包脚本将编译产物打包为 `.qlplugin` 文件。

## 📦 安装方法

1.  下载编译好的 `.qlplugin` 文件。
2.  确保已安装并启动 [QuickLook](https://github.com/QL-Win/QuickLook)。
3.  选中 `.qlplugin` 文件并按空格键，根据提示安装插件。
4.  安装完成后，可能需要重启 QuickLook 生效。

## 📝 许可证

*   **Duilib**: 遵循原项目许可证 (通常为 MIT 或 BSD，请参考源文件)。
*   **QuickLook.Plugin.DuilibPreview**: MIT License.
