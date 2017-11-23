using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace CollisionChecker
{
    public class CsvDataReader : IDataReader
    {
        public List<Collision> CollisionSets { get; } = new List<Collision>();
        public List<Robot> Robots { get; } = new List<Robot>();
        private TextFieldParser csvParser;
        private string inputFilePath;
        private readonly ICollectedDataChecker collectedDataChecker;

        public CsvDataReader(string inputFilePath, ICollectedDataChecker collectedDataChecker)
        {
            this.inputFilePath = inputFilePath;
            this.collectedDataChecker = collectedDataChecker;
            CollisionSets = new List<Collision>();
            Robots = new List<Robot>();
        }

        private void ClearData()
        {
            CollisionSets.Clear();
            Robots.Clear();
            this.csvParser = null;
        }
       
        public void ReadData()
        {
            ClearData();
            this.csvParser = new TextFieldParser(inputFilePath);
            SetupCsvParser();
            ReadRobotsCollisions();
            ReadInterlockProcess();
            this.csvParser.Close();
        }

        private void SetupCsvParser()
        {
            this.csvParser.CommentTokens = new string[] { "#" };
            this.csvParser.SetDelimiters(new string[] { ";" });
            this.csvParser.HasFieldsEnclosedInQuotes = false;
        }

        private void ReadRobotsCollisions()
        {
            string[] prevLine = null, splitCollNrs = null;
            string[] line = new string[3];
            line[0] = "";
            Robot robot1, robot2;

            if (csvParser.ReadFields()[0] != "Collision Sets")
            {
                return;
            }

            while (line[0] != "Interlock Process")
            {
                line = csvParser.ReadFields();
                if (prevLine == null)
                {
                    prevLine = line;
                    continue;
                }
                splitCollNrs = prevLine[2].Split(',');

                robot1 = Robots.Find(x => x.name == prevLine[0]);
                robot2 = Robots.Find(x => x.name == prevLine[1]);
                if (robot1 == null)
                {
                    robot1 = new Robot(prevLine[0]);
                    Robots.Add(robot1);
                }
                if (robot2 == null)
                {
                    robot2 = new Robot(prevLine[1]);
                    Robots.Add(robot2);
                }

                foreach (var nr in splitCollNrs)
                {
                    if (nr.Length == 0) continue;
                    var colNr = short.Parse(nr);
                    Collision newCollision = new Collision(colNr, robot1, robot2);
                    robot1.AddCollision(newCollision);
                    robot2.AddCollision(newCollision);
                    if (!CollisionSets.Contains(newCollision)) CollisionSets.Add(newCollision);
                }
                prevLine = line;
            }
        }

        private void ReadInterlockProcess()
        {
            string[] line = null, prevLine = null;
            string robotName = null;
            string[] interlockProcess;
            List<int> interlockProcessList = new List<int>();
            while (!csvParser.EndOfData)
            {
                line = csvParser.ReadFields();
                robotName = line[0];
                var robot = Robots.Find(x => x.name == robotName);
                if (robot == null) return;
                interlockProcess = line[1].Split(',');
                foreach (var proc in interlockProcess)
                {
                    interlockProcessList.Add(short.Parse(proc));
                }
                robot.interlockProcess2.Add(interlockProcessList);
                robot.interlockProcessId.Add("DUMMY");
                prevLine = line;
                interlockProcessList = new List<int>();
            }
            foreach (var robot in Robots)
            {
                robot.CreateRobotStates();
            }
        }

        public bool DataIsValid()
        {
            return collectedDataChecker.Check(Robots, CollisionSets);
        }
    }
}
