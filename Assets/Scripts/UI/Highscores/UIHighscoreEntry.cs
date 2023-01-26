using System;
using UnityEngine;
using UnityEngine.UI;

    public partial class UIHighscoreEntry : MonoBehaviour
    {
        private static readonly Color colorDefault = Color.white;
        [SerializeField] private Text textRank;
        [SerializeField] private Text textScore;
        [SerializeField] private Text textName;
        [SerializeField] private Image iconTrophy;
        private RectTransform _rectTransform;
        private Animator _animator;
        private static readonly int IsHighlited = Animator.StringToHash("IsHighlited");
        public RectTransform rectTransform => _rectTransform;
        public Animator animator => _animator;
        public void SetRank(string value, Color color = default)
        {
            SetText(textRank,value,color);
        }
        public void SetScore(string value, Color color = default)
        {
            SetText(textScore,value,color);
        }
        public void SetName(string value, Color color = default)
        {
            SetText(textName,value,color);
        }
        private void SetText(Text component, string value, Color color)
        {
            component.text = value;
            if (color == default)
            {
                component.color = colorDefault;
            }
            else
            {
                component.color = color;
            }
        }
        public void SetColorTrophy(Color color)
        {
            iconTrophy.color = color;
        }

        public void SetHightlight(bool value)
        {
            _animator.SetBool(IsHighlited, value);
            _animator.enabled = value;
        }
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _animator = GetComponent<Animator>();
        }
    }
