using UnityEngine;

namespace iGame.Editor
{
    [System.Serializable]
    public class iGamePackage
    {
        // Private variables
        [SerializeField] private string m_Name;
        [SerializeField] private string m_Description;
        [SerializeField] private Texture2D m_Icon;

        [SerializeField] private string m_GitUrl;
        [SerializeField] private string m_PackageHandle;

        // Getters
        public string Name => m_Name;
        public string Description => m_Description;
        public Texture2D Icon => m_Icon;

        public string GitUrl => m_GitUrl;
        public string PackageHandle => m_PackageHandle;
    }
}