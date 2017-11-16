using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollisionChecker;


namespace CC_UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void should_create_robots_and_collisions()
        {
            var TestedDataReader = new DataReader();
            TestedDataReader.ReadData();


        }//
    }
}
