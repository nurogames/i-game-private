using System;
using UnityEngine;

namespace iGame.Editor
{
    [System.Serializable]
    public class iGameProgress
    {
        // Private variables
        [SerializeField] private string m_Name;
        [SerializeField] private float m_Progress;

        // Events
        public Action<float> onProgressChanged;

        // Getters
        public string Name => m_Name;
        public float Progress => m_Progress;

        // Public functions
        public void ChangeProgress(float newProgress)
        {
            newProgress = Mathf.Clamp01(newProgress);

            m_Progress = newProgress;
            onProgressChanged?.Invoke(m_Progress);
        }
    }
}
