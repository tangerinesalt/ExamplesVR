# 图片转PDF转换器

## 概述

这是一组用于Unity项目中将图片转换为PDF文件的工具脚本。提供了三种不同的实现方式，您可以根据项目需求选择合适的实现。

## 脚本说明

### 1. ImageToPdfConverter.cs

基础版本的图片转PDF转换器，提供了基本的转换框架，但不包含实际的PDF生成功能。这个脚本主要用于演示转换流程。

### 2. ImageToPdfConverterWithITextCore.cs

使用iText Core库实现的图片转PDF转换器。iText Core是iText 7的核心库，是一个功能强大的PDF处理库，支持丰富的PDF功能。

**注意：** 使用此脚本需要导入iText Core相关DLL到项目中。项目已包含iText Core 9.2.0版本。

### 3. ImageToPdfConverterWithPdfSharp.cs

使用PDFsharp库实现的图片转PDF转换器。PDFsharp是一个轻量级的PDF生成库，适合简单的PDF生成需求。

**注意：** 使用此脚本需要导入PDFsharp库到项目中。

## 使用方法

1. 将脚本添加到GameObject上
2. 在Inspector中设置参数：
   - 图片文件夹路径
   - PDF输出路径
   - 页面大小和方向
   - 其他选项（根据不同实现有所不同）
3. 点击「转换图片为PDF」按钮开始转换

## 导入第三方库

### 导入iText Core库

项目已包含iText Core 9.2.0版本，位于Packages目录下。如果需要手动导入，请按以下步骤操作：

1. 从NuGet或[iText官网](https://github.com/itext/itext7-dotnet)下载iText Core相关DLL
2. 至少需要导入以下DLL：
   - iText.Kernel.dll
   - iText.IO.dll
   - iText.Layout.dll
   - iText.Commons.dll
3. 将DLL文件放入Unity项目的Assets/Plugins文件夹中

### 导入PDFsharp库

1. 从NuGet或[PDFsharp官网](http://www.pdfsharp.net/)下载PdfSharp.dll和PdfSharp.Charting.dll
2. 将DLL文件放入Unity项目的Assets/Plugins文件夹中

## 功能特点

- 支持多种图片格式（JPG、PNG、BMP等）
- 可按文件名排序
- 支持不同页面大小和方向
- 可保持图片原始比例或拉伸填充
- 支持设置页面边距
- 部分实现支持添加页码和文件名标题

## 注意事项

1. 这些脚本需要在Unity编辑器中运行，不适用于构建后的应用程序
2. 处理大量或高分辨率图片时可能会消耗较多内存
3. 实际使用时需要导入相应的第三方PDF库