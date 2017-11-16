using System.Collections.Generic;

namespace CollisionChecker
{
    public class RobotState
    {
        public List<Collision> takenColls = new List<Collision>();
        public Collision wantedColl;
        public short positionInInterlockProcess = 0;
        public short interlockProcessNr = 0;

        public RobotState() {}
        public RobotState(RobotState toCopy)
        {
            this.takenColls = new List<Collision>(toCopy.takenColls);
            this.wantedColl = toCopy.wantedColl;
        }

        public bool TakeCollisions()
        {
            foreach (var coll in takenColls)
            {
                if(takenColls.Count == 0) return true;
                else if (!coll.TakeCollision()) return false;
            }
            return true;
        }

        public void AddTakenCollision(Collision newTakenCol)
        {
            if (!this.takenColls.Contains(newTakenCol)) this.takenColls.Add(newTakenCol);
        }

        public void RemoveTakenCollision(Collision collision)
        {
            if (this.takenColls.Contains(collision)) this.takenColls.Remove(collision);
        }

        public void AddWantedCollision(Collision wantedColl)
        {
            this.wantedColl = wantedColl;
        }
    }
}
