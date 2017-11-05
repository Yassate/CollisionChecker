using System.Collections.Generic;

namespace CollisionChecker
{
    public static class ExtensionMethods
    {
        public static bool CompareRobotListByName<Robot>(this List<Robot> firstToCompare, List<Robot> secondToCompare)
        {
            int found = 0;
            if(firstToCompare.Count != secondToCompare.Count) return false;
            foreach(var robot in firstToCompare)
            {
                if (secondToCompare.Contains(robot)) found++;
            }
            if(found == firstToCompare.Count) return true;
            else return false;
        }
    }
}
