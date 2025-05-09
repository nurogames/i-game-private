using System.Collections.Generic;
using UnityEngine;

namespace iGame.Editor
{
    //[CreateAssetMenu(fileName = "iGamePackages", menuName = "iGame/iGamePackages")]
    public class iGamePackages : ScriptableObject
    {
        public List<iGamePackage> Packages = new List<iGamePackage>();
    }
}