using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using SthGame;

namespace SthGameLog
{
    public class Logger
    {
        public static readonly Logger Instance = new Logger();
        public const string PathLogDir = "Cache/Log";
        public string PathLogFile = string.Empty;
        private FileStream streamLog;
        private StreamWriter swLog;

        private bool isInit;
        public void Init()
        {
            if (isInit) return;
            isInit = true;

            try
            {
                string logDirectory = LogDirPath;
                DeleteOldLogs(logDirectory, 4);
#if UNITY_EDITOR
                PathLogFile = string.Format("{0}/Sth_{1}.log", logDirectory, System.DateTime.Now.ToString("yyyy_MM_dd"));
#else
                PathLogFile = string.Format("{0}/Sth_{1}.log", logDirectory, System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"));
#endif
                try
                {
                    System.IO.Directory.CreateDirectory(logDirectory);
                    streamLog = new FileStream(PathLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }

                if (streamLog == null)
                {
                    Logger.Error(string.Format("streamLog == null"));
                    return;
                }

                swLog = new StreamWriter(streamLog);

                if (swLog == null)
                {
                    Logger.Error(string.Format("swLog open failed"));
                    return;
                }

                swLog.AutoFlush = true;
                swLog.Write(string.Format("\n------------------{1}--------------------\n[Log][{0}] Logger Beginning\r\n", GetTimeStamp(), PathLogFile));

                Application.logMessageReceived += LogCallback;
            }
            catch (Exception e)
            {
                streamLog = null;
                swLog = null;
                Logger.Error(e.ToString());
            }
        }

        private void LogCallback(string condition, string stackTrace, LogType type)
        {
            try
            {
                if (swLog != null)
                {
                    swLog.WriteLine(string.Format("[{0}][{1}] {2}", type.ToString(), GetTimeStamp(), condition));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string LogDirPath
        {
            get { return Path.Combine(GetDataFolderPath(), PathLogDir); }
        }

        static string GetTimeStamp()
        {
            return DateTimeTools.FormatNowTimeStamp();
        }

        void DeleteOldLogs(string dir, int reservedCount)
        {
            if (string.IsNullOrEmpty(dir) || (!Directory.Exists(dir)))
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(dir);
            FileSystemInfo[] fsiList = di.GetFileSystemInfos("*.log");

            if (fsiList.Length > reservedCount)
            {
                Array.Sort(fsiList, (f1, f2) => f1.LastWriteTimeUtc > f2.LastWriteTimeUtc ? 1 : -1);

                FileSystemInfo fsi;
                for (int i = 0; i < fsiList.Length - reservedCount; ++i)
                {
                    fsi = fsiList[i];

                    if (fsi.FullName.EndsWith(".log"))
                    {
                        try
                        {
                            fsi.Delete();
                        }
                        catch (Exception ex)
                        {
                            Warming(string.Format("Failed to clear {0} at DeleteOldLogs: {1}", fsi.FullName, ex.ToString()));
                        }
                    }
                }
            }
        }

        static string EDITOR_DATA_PATH = null;
        const string EDITOR_DATA_PATH_REMOVE = "/Assets";
        public static string GetDataFolderPath()
        {
            string dataFolderDirectory = string.Empty;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (string.IsNullOrEmpty(EDITOR_DATA_PATH))
                {
                    EDITOR_DATA_PATH = Application.dataPath;
                    EDITOR_DATA_PATH = EDITOR_DATA_PATH.Substring(0, EDITOR_DATA_PATH.Length - EDITOR_DATA_PATH_REMOVE.Length);
                }
                dataFolderDirectory = EDITOR_DATA_PATH;
                if (dataFolderDirectory == null)
                {
                    dataFolderDirectory = string.Empty;
                }
            }
            else
            {
//#if UNITY_ANDROID || SHIPPING_EXTERNAL
//                dataFolderDirectory = Application.temporaryCachePath;
//#else
//                dataFolderDirectory = Application.persistentDataPath;
//#endif
            }
            return dataFolderDirectory;
        }


        public static void Log(string format, params object[] paras)
        {
            Debug.Log(string.Format("[{0}] {1}", GetTimeStamp(), string.Format(format, paras)));
        }

        public static void Warming(string format, params object[] paras)
        {
            Debug.LogWarning(string.Format("[{0}] {1}", GetTimeStamp(), string.Format(format, paras)));
        }

        public static void Error(string format, params object[] paras)
        {
            Debug.LogError(string.Format("[{0}] {1}", GetTimeStamp(), string.Format(format, paras)));
        }
    }
}
