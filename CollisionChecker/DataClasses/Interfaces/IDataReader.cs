using System.Collections.Generic;

namespace CollisionChecker
{
    public interface IDataReader
    {
        List<Robot> Robots { get; }
        List<Collision> CollisionSets { get; }
        void ReadData();
        bool DataIsValid();
    }
}