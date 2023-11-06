using UnityEngine;
using System;
using System.Collections.Generic;

namespace SpeechText
{
    [Serializable]
    public class TextLine
    {
        public string text;
    }

    [Serializable]
    public class TextSection
    {
        public string StateName;
        public List<TextLine> lines = new List<TextLine>();
    }

    [CreateAssetMenu(fileName = "New Text Data", menuName = "Custom/Text Data")]
    public class TextData : ScriptableObject
    {
        public List<TextSection> states = new List<TextSection>();
        public float messageShowCooldown = 3;

        public List<TextLine> GetTextLinesByStateName(string stateName)
        {
            return states.Find(item => item.StateName == stateName)?.lines ?? new List<TextLine>();
        }
    }
}
