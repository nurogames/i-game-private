using UnityEngine;

namespace iGame.Editor
{
    [System.Serializable]
    public class iGameProject
    {
        // Private variables
        [SerializeField] private string m_Name;
        [SerializeField] [Multiline] private string m_Description;
        [SerializeField] private Texture2D m_Icon;
        [SerializeField] private string m_CreationDatum;
        [SerializeField] private iGameProgress m_OverallProgress = new iGameProgress();
        [SerializeField] private iGameProgress[] m_Progresses;

        // Getters
        public string Name => m_Name;
        public string Description => m_Description;
        public Texture2D Icon => m_Icon;
        public string CreationDatum => m_CreationDatum;
        public iGameProgress OverallProgress => m_OverallProgress;
        public iGameProgress[] Progresses => m_Progresses;
    }
}
