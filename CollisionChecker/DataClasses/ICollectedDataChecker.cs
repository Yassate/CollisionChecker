using System.Collections.Generic;

namespace CollisionChecker
{
    public interface ICollectedDataChecker
    {
        bool Check(List<Robot> robots, List<Collision> collisionSets);
    }
}