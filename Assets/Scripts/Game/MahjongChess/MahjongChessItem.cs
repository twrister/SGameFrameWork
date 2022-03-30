using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SthGame
{
    public class MahjongChessItem : MonoBehaviour
    {
        public Image m_Image;
        public Image m_HeightLight;

        public int MahjongType => MahjongId / 4;
        public int MahjongId { get; private set; }
        int m_GridIndex;

        public void InitPos(int gridIdx)
        {
            m_GridIndex = gridIdx;
            ResetPos();
        }

        void ResetPos()
        {
            m_HeightLight.gameObject.SetActive(false);
            transform.localPosition = MahjongChessController.GetMahjongGridPos(m_GridIndex);
        }

        void SetImage()
        {
            m_Image.LoadSprite("MahjongAtlas", string.Format("mahjong_{0}", MahjongType));
        }

        public void SetData(int mahjongIdx, bool resetPos = false)
        {
            if (MahjongId != mahjongIdx)
            {
                MahjongId = mahjongIdx;
                SetImage();
            }

            UpdateShow(resetPos);
        }

        public void UpdateShow(bool resetPos = false)
        {
            gameObject.SetActive(MahjongId >= 0);
            if (MahjongId < 0) return;

            bool select = MahjongChessController.SelectIndexOne == MahjongId;
            m_Image.color = select ? Color.yellow : Color.white;

            if (resetPos) ResetPos();
        }

        public void OnPointerDown()
        {
            GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerDown, MahjongId);
        }

        public void OnPointerUp()
        {
            GlobalEventSystem.Instance.Fire(EventId.OnMahjongPointerUp, MahjongId);
        }

        public void DoFlashHighLight()
        {
            m_HeightLight.gameObject.SetActive(true);
            m_HeightLight.DOFade(0f, 0f);
            m_HeightLight.DOFade(0.5f, 0.2f).onComplete = () => {
                m_HeightLight.DOFade(0f, 0.2f);
            };
        }
    }
}