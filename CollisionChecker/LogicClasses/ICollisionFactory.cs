namespace CollisionChecker.LogicClasses
{
    public interface ICollisionFactory
    {
        Collision Instance(int nr, Robot robot1, Robot robot2);
    }
}