using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ImageToPdfConverter : MonoBehaviour
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
    public Vector2 pageSize = new Vector2(595, 842); // A4大小，单位为点(72dpi)
    
    [Tooltip("图片边距")]
    public float margin = 20f;
    
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
            
            // 注意：Unity默认不包含PDF生成库，这里使用伪代码表示PDF生成过程
            // 在实际项目中，你需要导入第三方PDF库，如iTextSharp或PDFsharp
            
            Debug.Log("开始创建PDF文档...");
            
            // 伪代码：创建PDF文档
            // using (var document = new PdfDocument())
            // {
            //     foreach (string imagePath in imageFilePaths)
            //     {
            //         // 加载图片
            //         Texture2D texture = LoadImageFromFile(imagePath);
            //         if (texture != null)
            //         {
            //             // 创建新页面
            //             var page = document.AddPage();
            //             page.Size = new Size(pageSize.x, pageSize.y);
            //             
            //             // 计算图片在页面中的位置和大小
            //             float imageWidth = pageSize.x - (margin * 2);
            //             float imageHeight = pageSize.y - (margin * 2);
            //             
            //             // 绘制图片到页面
            //             var gfx = XGraphics.FromPdfPage(page);
            //             var image = XImage.FromStream(new MemoryStream(texture.EncodeToPNG()));
            //             
            //             // 保持图片比例
            //             double imageRatio = (double)image.PixelWidth / image.PixelHeight;
            //             double pageRatio = imageWidth / imageHeight;
            //             
            //             double drawWidth, drawHeight;
            //             if (imageRatio > pageRatio)
            //             {
            //                 // 图片更宽，适应宽度
            //                 drawWidth = imageWidth;
            //                 drawHeight = drawWidth / imageRatio;
            //             }
            //             else
            //             {
            //                 // 图片更高，适应高度
            //                 drawHeight = imageHeight;
            //                 drawWidth = drawHeight * imageRatio;
            //             }
            //             
            //             // 计算居中位置
            //             double x = margin + (imageWidth - drawWidth) / 2;
            //             double y = margin + (imageHeight - drawHeight) / 2;
            //             
            //             gfx.DrawImage(image, x, y, drawWidth, drawHeight);
            //         }
            //     }
            //     
            //     // 保存PDF文档
            //     document.Save(outputPdfPath);
            // }
            
            // 由于Unity默认不包含PDF生成库，这里只是模拟完成过程
            Debug.Log($"PDF文档创建完成，保存至: {outputPdfPath}");
            Debug.Log("注意：此脚本需要导入第三方PDF库才能实际生成PDF文件。");
            Debug.Log("推荐使用iTextSharp或PDFsharp等库，需要在项目中导入相应的DLL文件。");
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

#if UNITY_EDITOR
[CustomEditor(typeof(ImageToPdfConverter))]
public class ImageToPdfConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ImageToPdfConverter converter = (ImageToPdfConverter)target;
        
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
    }
}
#endif