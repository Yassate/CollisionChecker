using System.Collections.Generic;

namespace CollisionChecker
{
    public interface IDataReader
    {
        List<Robot> RobotList { get; }
        List<Collision> CollisionList { get; }

        void ReadData();
    }
}