using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker.LogicClasses
{
    public class CollisionFactory : ICollisionFactory
    {
        public Collision Instance(int nr, Robot robot1, Robot robot2)
        {
            return new Collision(nr, robot1, robot2);
        }
    }
}
