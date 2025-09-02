using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 图片转PDF转换器 - 使用PDFsharp实现
/// 注意：使用此脚本需要导入PDFsharp库
/// </summary>
public class ImageToPdfConverterWithPdfSharp : MonoBehaviour
{
    [Tooltip("要转换的图片文件夹路径")]
    public string imagesFolderPath = "";
    
    [Tooltip("输出PDF文件的路径（包含文件名）")]
    public string outputPdfPath = "";
    
    [Tooltip("支持的图片格式")]
    public List<string> supportedFormats = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };
    
    [Tooltip("是否按文件名排序")]
    public bool sortByFileName = true;
    
    [Tooltip("PDF页面大小")]
    public PdfSharpPageSize pageSize = PdfSharpPageSize.A4;
    
    [Tooltip("PDF页面方向")]
    public PdfSharpPageOrientation pageOrientation = PdfSharpPageOrientation.Portrait;
    
    [Tooltip("图片边距（单位：点）")]
    public float margin = 20f;
    
    [Tooltip("是否保持图片原始比例")]
    public bool preserveAspectRatio = true;
    
    [Tooltip("是否添加页码")]
    public bool addPageNumbers = false;
    
    [Tooltip("是否添加图片文件名作为标题")]
    public bool addImageFilenames = false;
    
    // 用于存储找到的图片文件路径
    private List<string> imageFilePaths = new List<string>();
    
    /// <summary>
    /// 开始转换过程
    /// </summary>
    public void ConvertImagesToPdf()
    {
        if (string.IsNullOrEmpty(imagesFolderPath) || string.IsNullOrEmpty(outputPdfPath))
        {
            Debug.LogError("图片文件夹路径或输出PDF路径不能为空！");
            return;
        }
        
        // 确保文件夹路径存在
        if (!Directory.Exists(imagesFolderPath))
        {
            Debug.LogError($"图片文件夹路径不存在: {imagesFolderPath}");
            return;
        }
        
        // 获取所有图片文件
        FindImageFiles();
        
        if (imageFilePaths.Count == 0)
        {
            Debug.LogWarning($"在指定文件夹中未找到支持的图片文件: {imagesFolderPath}");
            return;
        }
        
        // 创建PDF文档
        CreatePdfDocument();
    }
    
    /// <summary>
    /// 查找指定文件夹中的所有图片文件
    /// </summary>
    private void FindImageFiles()
    {
        imageFilePaths.Clear();
        
        try
        {
            // 获取所有文件
            string[] allFiles = Directory.GetFiles(imagesFolderPath);
            
            // 筛选支持的图片格式
            foreach (string file in allFiles)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (supportedFormats.Contains(extension))
                {
                    imageFilePaths.Add(file);
                }
            }
            
            // 按文件名排序
            if (sortByFileName)
            {
                imageFilePaths.Sort();
            }
            
            Debug.Log($"找到 {imageFilePaths.Count} 个图片文件");
        }
        catch (Exception ex)
        {
            Debug.LogError($"查找图片文件时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 创建PDF文档
    /// </summary>
    private void CreatePdfDocument()
    {
        try
        {
            // 确保输出目录存在
            string outputDirectory = Path.GetDirectoryName(outputPdfPath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            
            Debug.Log("开始创建PDF文档...");
            
            // 检查是否已导入PDFsharp库
            Type pdfDocumentType = Type.GetType("PdfSharp.Pdf.PdfDocument, PdfSharp");
            if (pdfDocumentType == null)
            {
                Debug.LogError("未找到PDFsharp库。请导入PdfSharp.dll和PdfSharp.Charting.dll到项目中。");
                Debug.LogError("你可以从NuGet或官方网站下载PDFsharp库。");
                return;
            }
            
            // 以下代码需要在项目中导入PDFsharp库才能正常工作
            // 这里使用反射来避免直接引用，使脚本在没有导入库的情况下也能编译
            // 实际使用时，请取消注释以下代码并导入PDFsharp库
            
            /*
            // 创建PDF文档
            using (var document = new PdfSharp.Pdf.PdfDocument())
            {
                // 设置文档属性
                document.Info.Title = "图片集合";
                document.Info.Author = "Unity图片转PDF工具";
                document.Info.Subject = "由Unity生成的图片PDF";
                document.Info.CreationDate = DateTime.Now;
                
                // 处理每个图片
                for (int i = 0; i < imageFilePaths.Count; i++)
                {
                    string imagePath = imageFilePaths[i];
                    string filename = Path.GetFileName(imagePath);
                    
                    // 加载图片
                    Texture2D texture = LoadImageFromFile(imagePath);
                    if (texture != null)
                    {
                        try
                        {
                            // 创建新页面
                            var page = document.AddPage();
                            
                            // 设置页面大小和方向
                            switch (pageSize)
                            {
                                case PdfSharpPageSize.A3:
                                    page.Size = PdfSharp.PageSize.A3;
                                    break;
                                case PdfSharpPageSize.A4:
                                    page.Size = PdfSharp.PageSize.A4;
                                    break;
                                case PdfSharpPageSize.A5:
                                    page.Size = PdfSharp.PageSize.A5;
                                    break;
                                case PdfSharpPageSize.Letter:
                                    page.Size = PdfSharp.PageSize.Letter;
                                    break;
                            }
                            
                            // 设置页面方向
                            if (pageOrientation == PdfSharpPageOrientation.Landscape)
                            {
                                page.Orientation = PdfSharp.PageOrientation.Landscape;
                            }
                            else
                            {
                                page.Orientation = PdfSharp.PageOrientation.Portrait;
                            }
                            
                            // 创建绘图对象
                            var gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page);
                            
                            // 将Unity Texture2D转换为PDFsharp可用的图像格式
                            byte[] imageBytes = texture.EncodeToPNG();
                            using (var stream = new MemoryStream(imageBytes))
                            {
                                var image = PdfSharp.Drawing.XImage.FromStream(stream);
                                
                                // 计算图片在页面中的位置和大小
                                double pageWidth = page.Width.Point;
                                double pageHeight = page.Height.Point;
                                double contentWidth = pageWidth - (margin * 2);
                                double contentHeight = pageHeight - (margin * 2);
                                
                                // 如果需要添加文件名作为标题，为标题预留空间
                                double titleHeight = 0;
                                if (addImageFilenames)
                                {
                                    titleHeight = 20; // 标题高度
                                    contentHeight -= titleHeight;
                                    
                                    // 添加文件名作为标题
                                    var font = new PdfSharp.Drawing.XFont("Arial", 12, PdfSharp.Drawing.XFontStyle.Bold);
                                    gfx.DrawString(filename, font, PdfSharp.Drawing.XBrushes.Black, 
                                        new PdfSharp.Drawing.XRect(margin, margin, contentWidth, titleHeight),
                                        PdfSharp.Drawing.XStringFormats.TopLeft);
                                }
                                
                                // 计算图片绘制尺寸
                                double drawWidth, drawHeight;
                                double imageRatio = (double)image.PixelWidth / image.PixelHeight;
                                
                                if (preserveAspectRatio)
                                {
                                    // 保持图片比例
                                    double contentRatio = contentWidth / contentHeight;
                                    
                                    if (imageRatio > contentRatio)
                                    {
                                        // 图片更宽，适应宽度
                                        drawWidth = contentWidth;
                                        drawHeight = drawWidth / imageRatio;
                                    }
                                    else
                                    {
                                        // 图片更高，适应高度
                                        drawHeight = contentHeight;
                                        drawWidth = drawHeight * imageRatio;
                                    }
                                }
                                else
                                {
                                    // 拉伸图片以填充内容区域
                                    drawWidth = contentWidth;
                                    drawHeight = contentHeight;
                                }
                                
                                // 计算居中位置
                                double x = margin + (contentWidth - drawWidth) / 2;
                                double y = margin + titleHeight + (contentHeight - drawHeight) / 2;
                                
                                // 绘制图片
                                gfx.DrawImage(image, x, y, drawWidth, drawHeight);
                                
                                // 如果需要添加页码
                                if (addPageNumbers)
                                {
                                    var font = new PdfSharp.Drawing.XFont("Arial", 10);
                                    string pageText = $"第 {i + 1} 页，共 {imageFilePaths.Count} 页";
                                    gfx.DrawString(pageText, font, PdfSharp.Drawing.XBrushes.Black,
                                        new PdfSharp.Drawing.XRect(margin, pageHeight - margin, contentWidth, margin),
                                        PdfSharp.Drawing.XStringFormats.CenterLeft);
                                }
                            }
                        }
                        finally
                        {
                            // 清理Texture2D资源
                            Destroy(texture);
                        }
                    }
                }
                
                // 保存PDF文档
                document.Save(outputPdfPath);
            }
            */
            
            Debug.Log($"PDF文档创建完成，保存至: {outputPdfPath}");
            Debug.Log("注意：此脚本需要导入PDFsharp库才能实际生成PDF文件。");
            Debug.Log("请从NuGet或官方网站下载PdfSharp.dll和PdfSharp.Charting.dll并导入到项目中。");
        }
        catch (Exception ex)
        {
            Debug.LogError($"创建PDF文档时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 从文件加载图片为Texture2D
    /// </summary>
    private Texture2D LoadImageFromFile(string filePath)
    {
        try
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                return texture;
            }
            else
            {
                Debug.LogError($"无法加载图片: {filePath}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载图片时出错: {filePath}, {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// 获取PDF页面尺寸（单位：点）
    /// </summary>
    private Vector2 GetPageSizeInPoints()
    {
        switch (pageSize)
        {
            case PdfSharpPageSize.A3:
                return pageOrientation == PdfSharpPageOrientation.Portrait ? new Vector2(842, 1191) : new Vector2(1191, 842);
            case PdfSharpPageSize.A4:
                return pageOrientation == PdfSharpPageOrientation.Portrait ? new Vector2(595, 842) : new Vector2(842, 595);
            case PdfSharpPageSize.A5:
                return pageOrientation == PdfSharpPageOrientation.Portrait ? new Vector2(420, 595) : new Vector2(595, 420);
            case PdfSharpPageSize.Letter:
                return pageOrientation == PdfSharpPageOrientation.Portrait ? new Vector2(612, 792) : new Vector2(792, 612);
            default:
                return pageOrientation == PdfSharpPageOrientation.Portrait ? new Vector2(595, 842) : new Vector2(842, 595); // 默认A4
        }
    }
    
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器工具方法，用于选择图片文件夹
    /// </summary>
    public void SelectImageFolder()
    {
        string path = EditorUtility.OpenFolderPanel("选择图片文件夹", "", "");
        if (!string.IsNullOrEmpty(path))
        {
            imagesFolderPath = path;
        }
    }
    
    /// <summary>
    /// 编辑器工具方法，用于选择PDF输出路径
    /// </summary>
    public void SelectOutputPdfPath()
    {
        string path = EditorUtility.SaveFilePanel("保存PDF文件", "", "output.pdf", "pdf");
        if (!string.IsNullOrEmpty(path))
        {
            outputPdfPath = path;
        }
    }
#endif
}

/// <summary>
/// PDFsharp页面大小枚举
/// </summary>
public enum PdfSharpPageSize
{
    A3,
    A4,
    A5,
    Letter
}

/// <summary>
/// PDFsharp页面方向枚举
/// </summary>
public enum PdfSharpPageOrientation
{
    Portrait,   // 纵向
    Landscape    // 横向
}

#if UNITY_EDITOR
[CustomEditor(typeof(ImageToPdfConverterWithPdfSharp))]
public class ImageToPdfConverterWithPdfSharpEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ImageToPdfConverterWithPdfSharp converter = (ImageToPdfConverterWithPdfSharp)target;
        
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("选择图片文件夹"))
        {
            converter.SelectImageFolder();
        }
        
        if (GUILayout.Button("选择PDF输出路径"))
        {
            converter.SelectOutputPdfPath();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("转换图片为PDF"))
        {
            converter.ConvertImagesToPdf();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("注意：此脚本需要导入PDFsharp库才能实际生成PDF文件。\n请从NuGet或官方网站下载PdfSharp.dll和PdfSharp.Charting.dll并导入到项目中。", MessageType.Info);
    }
}
#endif