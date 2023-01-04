using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SthGame
{
    public class MangWenPicController : UIBaseController
    {
        MangWenPicView m_View;

        //"⠀⠄"
        string m_Code = "⠀⠁⠈⠉⠂⠃⠊⠋⠐⠑⠘⠙⠒⠓⠚⠛⠄⠅⠌⠍⠆⠇⠎⠏⠔⠕⠜⠝⠖⠗⠞⠟⠠⠡⠨⠩⠢⠣⠪⠫⠰⠱⠸⠹⠲⠳⠺⠻⠤⠥⠬⠭⠦⠧⠮⠯⠴⠵⠼⠽⠶⠷⠾⠿⡀⡁⡈⡉⡂⡃⡊⡋⡐⡑⡘⡙⡒⡓⡚⡛⡄⡅⡌⡍⡆⡇⡎⡏⡔⡕⡜⡝⡖⡗⡞⡟⡠⡡⡨⡩⡢⡣⡪⡫⡰⡱⡸⡹⡲⡳⡺⡻⡤⡥⡬⡭⡦⡧⡮⡯⡴⡵⡼⡽⡶⡷⡾⡿⢀⢁⢈⢉⢂⢃⢊⢋⢐⢑⢘⢙⢒⢓⢚⢛⢄⢅⢌⢍⢆⢇⢎⢏⢔⢕⢜⢝⢖⢗⢞⢟⢠⢡⢨⢩⢢⢣⢪⢫⢰⢱⢸⢹⢲⢳⢺⢻⢤⢥⢬⢭⢦⢧⢮⢯⢴⢵⢼⢽⢶⢷⢾⢿⣀⣁⣈⣉⣂⣃⣊⣋⣐⣑⣘⣙⣒⣓⣚⣛⣄⣅⣌⣍⣆⣇⣎⣏⣔⣕⣜⣝⣖⣗⣞⣟⣠⣡⣨⣩⣢⣣⣪⣫⣰⣱⣸⣹⣲⣳⣺⣻⣤⣥⣬⣭⣦⣧⣮⣯⣴⣵⣼⣽⣶⣷⣾⣿";

        string[,] m_MangWenArray;

        protected override string GetResourcePath()
        {
            return "Prefabs/MangWenPic";
        }

        public override void Init()
        {
            base.Init();
            m_View = UINode as MangWenPicView;

            m_View.m_RefreshBtn.onClick.AddListener(OnClickRefreshBtn);
            m_View.m_CloseBtn.onClick.AddListener(OnClickCloseBtn);
        }

        protected override void OpenCallBack()
        {
            base.OpenCallBack();

            GenerateMangWenPicTxt();
        }

        void OnClickRefreshBtn()
        {
            GenerateMangWenPicTxt();
        }

        private void OnClickCloseBtn()
        {
            Close();
        }

        void GenerateMangWenPicTxt()
        {
            Texture2D m_Texture = m_View.m_SourceRawImg.mainTexture as Texture2D;
            int width = m_Texture.width / m_View.m_PixelSize;
            int height = m_Texture.height / m_View.m_PixelSize;

            int strWidth = width / 2;
            int strHeight = height / 4;
            m_MangWenArray = new string[strWidth, strHeight];

            bool[,] boolArray = new bool[width, height];

            Texture2D tex2D = new Texture2D(width, height, TextureFormat.ARGB32, false);

            Logger.Log($"w = {width}, h = {height} ");

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color32 c = m_Texture.GetPixel(i * m_View.m_PixelSize, j * m_View.m_PixelSize);
                    float value = c.r * 0.22f + c.g * 0.707f + c.b * 0.071f;

                    float fValue = value / 255f;
                    //Logger.Log($"x = {i}, y = {i}, value = {value.ToString()}, fValue = {fValue.ToString()}");

                    boolArray[i, j] = fValue > m_View.m_Threshold;
                    fValue = fValue > m_View.m_Threshold ? 1f : 0f;

                    tex2D.SetPixel(i, j, new Color(fValue, fValue, fValue, c.a));
                }
            }

            StringBuilder sb = new StringBuilder();

            Logger.Log($"strWidth = {strWidth.ToString()}, strHeight = {strHeight.ToString()}");

            for (int j = 0; j < strHeight; j++)
            {
                for (int i = 0; i < strWidth; i++)
                {
                    int v1 = (!boolArray[i * 2 + 0, (strHeight - j) * 4 - 1] ? 1 : 0) << 0;
                    int v2 = (!boolArray[i * 2 + 1, (strHeight - j) * 4 - 1] ? 1 : 0) << 1;
                    int v3 = (!boolArray[i * 2 + 0, (strHeight - j) * 4 - 2] ? 1 : 0) << 2;
                    int v4 = (!boolArray[i * 2 + 1, (strHeight - j) * 4 - 2] ? 1 : 0) << 3;
                    int v5 = (!boolArray[i * 2 + 0, (strHeight - j) * 4 - 3] ? 1 : 0) << 4;
                    int v6 = (!boolArray[i * 2 + 1, (strHeight - j) * 4 - 3] ? 1 : 0) << 5;
                    int v7 = (!boolArray[i * 2 + 0, (strHeight - j) * 4 - 4] ? 1 : 0) << 6;
                    int v8 = (!boolArray[i * 2 + 1, (strHeight - j) * 4 - 4] ? 1 : 0) << 7;

                    byte index = (byte)(v1 | v2 | v3 | v4 | v5 | v6 | v7 | v8);

                    sb.Append(m_Code[index].ToString());

                    //Logger.Log($"i = {i}, j = {j}, index = {index.ToString()}, code = {m_Code[index].ToString()}");
                }
                sb.Append("\n");
            }


            tex2D.Apply(false, false);

            m_View.m_TranferRawImg.texture = tex2D;

            m_View.m_TranferRawImg.SetNativeSize();

            m_View.m_PicTxt.text = sb.ToString();
        }
    }
}