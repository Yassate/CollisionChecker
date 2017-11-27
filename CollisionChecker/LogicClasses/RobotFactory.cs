using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    public class RobotFactory : IRobotFactory
    {
        public Robot Instance(string name)
        {
            return new Robot(name);
        }
    }
}
