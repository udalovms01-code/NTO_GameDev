using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Endings
{
    [Serializable]
    public class EndingSlide
    {
        [field: TextArea]
        [field: SerializeField] public string Text { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }

        public EndingSlide()
        {
        }

        public EndingSlide(string text, Sprite image = null)
        {
            Text = text;
            Image = image;
        }
    }

    [Serializable]
    public class EndingDefinition
    {
        [SerializeField] private string _id = "default";
        [SerializeField] private List<EndingSlide> _slides = new();

        public string Id => _id;
        public IReadOnlyList<EndingSlide> Slides => _slides;

        public void AddSlide(string text, Sprite image = null)
        {
            _slides ??= new List<EndingSlide>();
            _slides.Add(new EndingSlide(text, image));
        }
    }
}
