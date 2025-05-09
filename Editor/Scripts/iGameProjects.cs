using UnityEngine;
using System.Collections.Generic;

namespace iGame.Editor
{

    //[CreateAssetMenu(fileName = "iGameProjects", menuName = "iGame/iGameProjects")]
    public class iGameProjects : ScriptableObject
    {
        public List<iGameProject> Projects = new List<iGameProject>();
    }
}