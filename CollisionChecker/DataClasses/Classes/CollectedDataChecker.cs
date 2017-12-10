using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollisionChecker
{
    public class CollectedDataChecker : ICollectedDataChecker
    {
        private readonly INotifier notifier;
        public CollectedDataChecker(INotifier notifier)
        {
            this.notifier = notifier;
        } 
        public bool Check(List<Robot> robots, List<Collision> collisionSets)
        {
            if (robots.Count != 0 && collisionSets.Count != 0)
            {
                return true;
            }
            else
            {
                notifier.ShowMessage("Data not loaded! Probably formatting is incorrect.");
                return false;
            }
        }
    }
}
