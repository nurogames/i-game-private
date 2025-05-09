using UnityEngine;

namespace iGame.Editor
{
    [System.Serializable]
    public class iGameProject
    {
        // Private variables
        [SerializeField] private string m_Name;
        [SerializeField] private string m_Description;
        [SerializeField] private Texture2D m_Icon;
        [SerializeField] private string m_CreationDatum;

        // Getters
        public string Name => m_Name;
        public string Description => m_Description;
        public Texture2D Icon => m_Icon;
        public string CreationDatum => m_CreationDatum;
    }
}
