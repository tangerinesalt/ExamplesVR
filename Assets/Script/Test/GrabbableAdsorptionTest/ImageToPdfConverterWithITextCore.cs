using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
// iText Core引用
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 图片转PDF转换器 - 使用iText Core实现
/// 注意：使用此脚本需要导入iText Core库
/// </summary>
public class ImageToPdfConverterWithITextCore : MonoBehaviour
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
    public PdfPageSize pageSize = PdfPageSize.A4;
    
    [Tooltip("PDF页面方向")]
    public PdfPageOrientation pageOrientation = PdfPageOrientation.Portrait;
    
    [Tooltip("图片边距（单位：点）")]
    public float margin = 20f;
    
    [Tooltip("图片质量（0-100）")]
    [Range(0, 100)]
    public int imageQuality = 90;
    
    [Tooltip("是否保持图片原始比例")]
    public bool preserveAspectRatio = true;
    
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
        this.CreatePdfDocument();
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
                string extension = System.IO.Path.GetExtension(file).ToLower();
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
            Debug.LogError($"创建PDF文档失败: {ex.Message}");
            Debug.LogError($"堆栈跟踪: {ex.StackTrace}");
            Debug.LogError($"查找图片文件时出错: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 创建PDF文档
    /// </summary>
    private void CreatePdfDocument()
    {
        Debug.Log($"创建PDF文档mathf...");
        
        try
        {
            // 确保输出目录存在
            string outputDirectory = System.IO.Path.GetDirectoryName(outputPdfPath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Debug.Log($"创建输出目录: {outputDirectory}");
                Directory.CreateDirectory(outputDirectory);
                Debug.Log($"目录创建成功: {outputDirectory}");
            }

            Debug.Log("开始创建PDF文档...");

            // 使用iText Core库生成PDF文件
            // 设置页面大小和方向
            Debug.Log($"设置PDF页面: 大小={pageSize}, 方向={pageOrientation}");
            PageSize pageRect;
            switch (pageSize)
            {
                case PdfPageSize.A3:
                    pageRect = PageSize.A3;
                    Debug.Log("使用A3页面大小");
                    break;
                case PdfPageSize.A4:
                    pageRect = PageSize.A4;
                    Debug.Log("使用A4页面大小");
                    break;
                case PdfPageSize.A5:
                    pageRect = PageSize.A5;
                    Debug.Log("使用A5页面大小");
                    break;
                case PdfPageSize.Letter:
                    pageRect = PageSize.LETTER;
                    Debug.Log("使用Letter页面大小");
                    break;
                default:
                    pageRect = PageSize.A4;
                    Debug.Log("使用默认A4页面大小");
                    break;
            }

            // 设置页面方向
            if (pageOrientation == PdfPageOrientation.Landscape)
            {
                Debug.Log("设置页面方向为横向");
                pageRect = pageRect.Rotate();
            }
            else
            {
                Debug.Log("设置页面方向为纵向");
            }

            // 创建PDF文档和写入器
            PdfWriter writer = null;
            Debug.Log("创建PDF写入器...");
            try
            {
                writer = new PdfWriter(outputPdfPath);
                Debug.Log("创建PDF写入器成功");
            }
            catch (Exception ex)
            {
                Debug.LogError($"创建PDF写入器失败: {ex.ToString()}");
                if (ex.InnerException != null)
                    Debug.LogError($"内部异常: {ex.InnerException.ToString()}");
                // 可以选择在这里返回null或抛出异常
                return;
            }

            PdfDocument pdf = new PdfDocument(writer);
            if(pdf != null) Debug.Log("创建PDF文档成功");
            else Debug.LogError("创建PDF文档失败");
            Document document = new Document(pdf, pageRect);
            if (document != null) Debug.Log("创建PDF文档对象成功");
            else Debug.LogError("创建PDF文档对象失败");

            if (document != null && pdf != null && writer != null)
            {
                Debug.Log("创建PDF文档中...");
                // 设置页边距
                document.SetMargins(margin, margin, margin, margin);

                foreach (string imagePath in imageFilePaths)
                {
                    Debug.Log($"处理图片: {imagePath}");
                    // 加载图片
                    Texture2D texture = LoadImageFromFile(imagePath);
                    if (texture != null)
                    {
                        try
                        {
                            Debug.Log("将Texture2D转换为PNG格式");
                            // 将Unity Texture2D转换为iText可用的图像格式
                            byte[] imageBytes = texture.EncodeToPNG();
                            ImageData imageData = ImageDataFactory.Create(imageBytes);
                            Image pdfImage = new Image(imageData);

                            // 计算图片在页面中的位置和大小
                            float pageWidth = pageRect.GetWidth() - (margin * 2);
                            float pageHeight = pageRect.GetHeight() - (margin * 2);

                            if (preserveAspectRatio)
                            {
                                Debug.Log("保持图片原始比例");
                                // 保持图片比例
                                if (imageData.GetWidth() > pageWidth || imageData.GetHeight() > pageHeight)
                                {
                                    Debug.Log("缩放图片以适应页面");
                                    // 需要缩放图片以适应页面
                                    float widthRatio = pageWidth / imageData.GetWidth();
                                    float heightRatio = pageHeight / imageData.GetHeight();
                                    float scaleFactor = Math.Min(widthRatio, heightRatio);

                                    pdfImage.SetWidth(imageData.GetWidth() * scaleFactor);
                                    pdfImage.SetHeight(imageData.GetHeight() * scaleFactor);
                                }
                            }
                            else
                            {
                                Debug.Log("拉伸图片以填充页面");
                                // 拉伸图片以填充内容区域
                                pdfImage.SetWidth(pageWidth);
                                pdfImage.SetHeight(pageHeight);
                            }

                            // 居中图片
                            float xPos = (pageWidth - pdfImage.GetImageWidth()) / 2 + margin;
                            float yPos = (pageHeight - pdfImage.GetImageHeight()) / 2 + margin;
                            pdfImage.SetFixedPosition(xPos, yPos);

                            Debug.Log("添加图片到PDF文档");
                            // 添加图片到文档
                            document.Add(pdfImage);

                            // 如果不是最后一张图片，添加新页面
                            if (imagePath != imageFilePaths[imageFilePaths.Count - 1])
                            {
                                Debug.Log("添加新页面");
                                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                            }
                        }
                        finally
                        {
                            Debug.Log("清理Texture2D资源");
                            // 清理Texture2D资源
                            Destroy(texture);
                        }
                    }
                    else
                    {
                        Debug.LogError($"无法加载图片: {imagePath}");
                    }
                }

                // 关闭文档
                document.Close();
            }

            Debug.Log($"PDF文档创建完成，保存至: {outputPdfPath}");
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
            case PdfPageSize.A3:
                return pageOrientation == PdfPageOrientation.Portrait ? new Vector2(842, 1191) : new Vector2(1191, 842);
            case PdfPageSize.A4:
                return pageOrientation == PdfPageOrientation.Portrait ? new Vector2(595, 842) : new Vector2(842, 595);
            case PdfPageSize.A5:
                return pageOrientation == PdfPageOrientation.Portrait ? new Vector2(420, 595) : new Vector2(595, 420);
            case PdfPageSize.Letter:
                return pageOrientation == PdfPageOrientation.Portrait ? new Vector2(612, 792) : new Vector2(792, 612);
            default:
                return pageOrientation == PdfPageOrientation.Portrait ? new Vector2(595, 842) : new Vector2(842, 595); // 默认A4
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
/// PDF页面大小枚举
/// </summary>
public enum PdfPageSize
{
    A3,
    A4,
    A5,
    Letter
}

/// <summary>
/// PDF页面方向枚举
/// </summary>
public enum PdfPageOrientation
{
    Portrait,   // 纵向
    Landscape    // 横向
}

#if UNITY_EDITOR
[CustomEditor(typeof(ImageToPdfConverterWithITextCore))]
public class ImageToPdfConverterWithITextCoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ImageToPdfConverterWithITextCore converter = (ImageToPdfConverterWithITextCore)target;
        
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
        EditorGUILayout.HelpBox("注意：此脚本使用iText Core库生成PDF文件。\n项目已包含iText Core 9.2.0版本，位于Packages目录下。\n如需更新，请导入以下DLL：iText.Kernel.dll, iText.IO.dll, iText.Layout.dll, iText.Commons.dll", MessageType.Info);
    }
}
#endif