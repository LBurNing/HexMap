using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemTipsInfo
{
    class TipsInfo
    {
        public string content;

        public TipsInfo(string _content)
        {
            this.content = _content;
        }
    }

    public class SystemTipsUI : MonoBehaviour
    {
        private CanvasGroup m_MainGroup;
        private Transform m_CloneTarget;

        private Tween m_CurTween;
        private float m_AnimTime = 0.5f;

        [Header("tips 停留的时间")]
        [SerializeField]
        private float m_TipsVisibleTime = 3f;

        [Header("最多同时显示的个数")]
        [SerializeField]
        private int m_ShowMaxCount = 5;

        [Header("每行最多展示的文本字数")]
        [SerializeField]
        private int m_RowMaxTextConut = 50;

        // 准备要显示的 tips 数据集
        private Stack<TipsInfo> m_ReadyVisibleTipsInfo = new Stack<TipsInfo>();
        // 正在显示的 tips 个数
        private int m_VisibleTips = 0;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            m_MainGroup = GetComponent<CanvasGroup>();
            m_MainGroup.blocksRaycasts = false;
            m_MainGroup.alpha = 1;

            m_CloneTarget = transform.Find("Clone");
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="type">tips 类型</param>
        /// <returns></returns>
        public Transform AddSystemInfo(string content)
        {
            if (m_VisibleTips < m_ShowMaxCount) {
                // 直接显示
                ++m_VisibleTips;
                return RealAddSystemInfo(content);
            }
            else
            {
                // 添加到 stack 中
                m_ReadyVisibleTipsInfo.Push(new TipsInfo(content));
            }
            return null;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="type">tips 类型</param>
        /// <returns></returns>
        private Transform RealAddSystemInfo(string content)
        {

            m_CloneTarget.Find("info").GetComponent<Text>().text = HandContentBeyong(content);
            CanvasGroup tmpGroup = m_CloneTarget.GetComponent<CanvasGroup>();
            tmpGroup.alpha = 1;

            m_CloneTarget.gameObject.SetActive(true);

            // 重新刷新布局，避免文本还没有自适应布局
            //LayoutRebuilder.ForceRebuildLayoutImmediate(m_CloneTarget as RectTransform);

            // 动画
            Sequence seq = DOTween.Sequence();
            seq.Append(CanvasGroupFade(tmpGroup, 1, m_AnimTime));
            seq.AppendInterval(m_TipsVisibleTime);
            seq.Append(CanvasGroupFade(tmpGroup, 0, m_AnimTime));
            seq.AppendCallback(() => {
                //GameObject.Destroy(m_CloneTarget.gameObject);
                --m_VisibleTips;
                TipsInfo tmpInfo = m_ReadyVisibleTipsInfo.Pop();
                // 从 stack 中取数据生成新的 item
                if (tmpInfo != null) {
                    ++m_VisibleTips;
                    RealAddSystemInfo(tmpInfo.content);
                }
            });

            return m_CloneTarget;
        }

        private string HandContentBeyong(string text)
        {
            int contentLen = text.Length;
            int curCount = contentLen / m_RowMaxTextConut;
            if (curCount < 1) return text;

            for (int i = 0; i < curCount; ++i) {
                text = text.Insert((i + 1) * m_RowMaxTextConut - 1, "\n");
            }
            return text;
        }

        /// <summary>
        /// 控制 CanvaGroup 的显隐
        /// </summary>
        /// <param name="isShow"></param>
        public void ToggleMainGroup(bool isShow)
        {
            if (m_CurTween != null)
            {
                m_CurTween.Kill();
                m_CurTween = null;
            }

            m_CurTween = CanvasGroupFade(m_MainGroup, isShow ? 1 : 0, m_AnimTime).OnComplete(() => {
                m_CurTween = null;
            });
        }

        private Tween CanvasGroupFade(CanvasGroup target ,float endValue ,float animTime)
        {
            return DOTween.To(() => target.alpha, (v) => {
                target.alpha = v;
            }, endValue, animTime);
        }
    }
}