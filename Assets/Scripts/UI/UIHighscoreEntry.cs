using System;
using UnityEngine;
using UnityEngine.UI;

    public class UIHighscoreEntry : MonoBehaviour
    {
        private static readonly Color colorDefault = Color.white;
        [SerializeField] private Text textRank;
        [SerializeField] private Text textScore;
        [SerializeField] private Text textName;
        [SerializeField] private Image iconTrophy;
        private RectTransform _rectTransform;
        private Animator _animator;

        public RectTransform rectTransform => _rectTransform;
        public Animator animator => _animator;
        public void SetRank(string value, Color color = default)
        {
            textRank.text = value;
            if (color == default)
            {
                textRank.color = colorDefault;
            }
            else
            {
                textRank.color = color;
            }
        }
        public void SetScore(string value, Color color = default)
        {
            textScore.text = value;
            if (color == default)
            {
                textScore.color = colorDefault;
            }
            else
            {
                textScore.color = color;
            }
        }
        public void SetName(string value, Color color = default)
        {
            textName.text = value;
            if (color == default)
            {
                textName.color = colorDefault;
            }
            else
            {
                textName.color = color;
            }
        }
        public void SetColorTrophy(Color color)
        {
            iconTrophy.color = color;
        }
        

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _animator = GetComponent<Animator>();
        }
        // .Find("posText").GetComponent<Text>().text = "";
        // entryTransform.Find("scoreText").GetComponent<Text>().text = "";
        // entryTransform.Find("nameText").GetComponent<Text>()
    }
