using UnityEditor;
using System.IO;

public class AssetBundlesEditor
{

    /// <summary>
    /// 打包生成所有的AssetBundles(包)
    /// </summary>
    //[MenuItem("Tools/BuildAllAB")]
    //public static void BuildAllAB()
    //{
    //    //打包AB输出路径
    //    string strABOutPathDIR = string.Empty;

    //    //获取"StreamingAssets"数值
    //    strABOutPathDIR = PathTools.GetABOutPath();//Application.streamingAssetPath/Windows

    //    //判断生成输出目录文件夹
    //    if (!Directory.Exists(strABOutPathDIR))
    //    {
    //        Directory.CreateDirectory(strABOutPathDIR);
    //    }

    //    // BuildAssetBundleOptions.None 使用LZMA算法打包，压缩的包更小，但是加载时间更长。
    //    // BuildAssetBundleOptions.UncompressedAssetBundle：不压缩，包大，加载快
    //    // BuildAssetBundleOptions.ChunkBasedCompression：使用LZ4压缩，压缩率没有LZMA高，但是我们可以加载指定资源而不用解压全部。
    //    // BuildTarget 选择build出来的AB包要使用的平台
    //    BuildPipeline.BuildAssetBundles(strABOutPathDIR, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    //}

    //public class DeleteAssetBundle
    //{
    //    /// <summary>
    //    /// 批量删除AB包文件
    //    /// </summary>
    //    [MenuItem("Tools/DeleteAllAB")]
    //    public static void DelAssetBundle()
    //    {
    //        //删除AB包输出目录
    //        string strNeedDeleteDIR = string.Empty;

    //        strNeedDeleteDIR = PathTools.GetABOutPath();
    //        if (!string.IsNullOrEmpty(strNeedDeleteDIR))
    //        {
    //            //注意： 这里参数"true"表示可以删除非空目录
    //            Directory.Delete(strNeedDeleteDIR, true);
    //            //去除删除警告
    //            File.Delete(strNeedDeleteDIR + ".meta");
    //            //刷新
    //            AssetDatabase.Refresh();
    //        }
    //    }
    //}
}