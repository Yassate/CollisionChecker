using System;
using System.Collections.Generic;

namespace CollisionChecker
{
    class Collision : IEquatable<Collision>, IComparer<Collision>
    {
        public int nr = 0;
        public Robot Robot1, Robot2;
        private bool taken = false;

        public Collision() {}

        public Collision(int nr, Robot Robot1, Robot Robot2)
        {
            this.nr = nr;
            this.Robot1 = Robot1;
            this.Robot2 = Robot2;
        }

        public bool IsTaken()
        {
            return taken;
        }

        public bool TakeCollision()
        {
            if (!this.taken) return this.taken = true;
            else return false;
        }

        public void ReleaseCollision() 
        { 
            this.taken = false;
        }

        public Robot GetSecondRobot(Robot firstRobot)
        {
            if (firstRobot == this.Robot1) return this.Robot2;
            else if (firstRobot == this.Robot2) return this.Robot1;
            else return null;
        }

        public int Compare(Collision comp1, Collision comp2)
        {
            if (comp1.nr < comp2.nr) return -1;
            else if (comp1.nr == comp2.nr) return 0;
            else  return 1;
        }

        public static bool operator ==(Collision comp1, Collision comp2)
        {
            bool collisionNull, robotNull, firstCase, secondCase, nrCheck;
            if (ReferenceEquals(comp1, comp2)) return true;
            collisionNull = object.ReferenceEquals(comp1, null) || object.ReferenceEquals(comp2, null);
            if (collisionNull) return false;

            robotNull = !(comp1.Robot1 != null && comp1.Robot2 != null && comp2.Robot1 != null && comp2.Robot2 != null);
            firstCase = comp1.Robot1 == comp2.Robot1 && comp1.Robot2 == comp2.Robot2;
            secondCase = comp1.Robot1 == comp2.Robot2 && comp1.Robot2 == comp2.Robot1;
            nrCheck = comp1.nr == comp2.nr;

            if (robotNull) return false;
            if (comp1.GetHashCode() != comp2.GetHashCode()) return false;
            return nrCheck && (firstCase || secondCase);
        }

        public static bool operator !=(Collision comp1, Collision comp2)
        {
            return !(comp1 == comp2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((Collision)obj);
        }

        public bool Equals(Collision comp2)
        {
            bool collisionNull, robotNull, firstCase, secondCase, nrCheck;
            collisionNull = object.ReferenceEquals(comp2, null);
            if (collisionNull) return false;
            if (ReferenceEquals(this, comp2)) return true;

            robotNull = this.Robot1 == null || this.Robot2 == null || comp2.Robot1 == null || comp2.Robot2 == null;
            firstCase = this.Robot1 == comp2.Robot1 && this.Robot2 == comp2.Robot2;
            secondCase = this.Robot1 == comp2.Robot2 && this.Robot2 == comp2.Robot1;
            nrCheck = this.nr == comp2.nr;

            if (robotNull) return false;
            if (this.GetHashCode() != comp2.GetHashCode()) return false;
            return nrCheck && (firstCase || secondCase);
        }

        public override int GetHashCode()
        {
            return 3 + (3 ^ nr);
        }

    }
}


