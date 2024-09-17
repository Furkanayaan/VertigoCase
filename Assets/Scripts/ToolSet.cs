using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using WheelObjectsType;

namespace ToolSet {
    static public class Tools {
        static public ObjectsType StringToType(string typeName) {
            ObjectsType typeStatus;
            string splitedName = typeName.Split("_")[0];
            System.Enum.TryParse(splitedName, out typeStatus);
            return typeStatus;
        } 
        
        static public int StringToLevel(string name) {
            string lvl = Regex.Match(name,@"\d+").Value;
            return int.Parse(lvl);
        }
    }
    
}
