using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using LitJson;

namespace SthGame
{
    public class ConfigManager
    {
        public static T LoadConfig<T>(string path)
        {
            string jsonStr = string.Empty;

            T config = default(T);

            jsonStr = LoadTextConfig(path);
            if (!string.IsNullOrEmpty(jsonStr))
            {
                try
                {
                    config = JsonMapper.ToObject<T>(jsonStr);
                }
                catch (System.Exception e)
                {
                    Logger.Error(string.Format("LoadConfig Json File {0} Failed! Exception:\r\n{1}", path, e.Message));
                }
            }
            return config;
        }

        public static string LoadTextConfig(string configFilePath)
        {
            string text = string.Empty;
            TextAsset textAsset = Resources.Load<TextAsset>(configFilePath);
            if (textAsset != null)
            {
                text = textAsset.text;
            }
            else
            {
                FileInfo fileInfo = new FileInfo(configFilePath);
                if (fileInfo.Exists)
                {
                    StreamReader streamReader = null;
                    try
                    {
                        streamReader = fileInfo.OpenText();
                        text = streamReader.ReadToEnd();
                        streamReader.Close();
                        streamReader.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (streamReader != null)
                        {
                            streamReader.Close();
                            streamReader.Dispose();
                        }
                        text = string.Empty;
                        Logger.Error(e.ToString());
                    }
                }
            }
            return text;
        }
    }
}
