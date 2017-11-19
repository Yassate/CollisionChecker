using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;


namespace CollisionChecker
{
    public class CsvDataReader : IDataReader
    {
        public List<Collision> CollisionList { get; } = new List<Collision>();
        public List<Robot> RobotList { get; } = new List<Robot>();
        private TextFieldParser csvParser;
        private string inputFilePath;

        public CsvDataReader(string inputFilePath)
        {
            this.inputFilePath = inputFilePath;
            CollisionList = new List<Collision>();
            RobotList = new List<Robot>();
        }

        private void ClearData()
        {
            CollisionList.Clear();
            RobotList.Clear();
            this.csvParser = null;
        }
       
        public void ReadData()
        {
            ClearData();
            this.csvParser = new TextFieldParser(inputFilePath);
            SetupCsvParser();
            ReadRobotsCollisionsFromCsv();
            ReadInterlockProcessFromCsv();
            this.csvParser.Close();
        }

        private void SetupCsvParser()
        {
            this.csvParser.CommentTokens = new string[] { "#" };
            this.csvParser.SetDelimiters(new string[] { ";" });
            this.csvParser.HasFieldsEnclosedInQuotes = false;
        }

        public void ReadRobotsCollisionsFromCsv()
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

                robot1 = RobotList.Find(x => x.name == prevLine[0]);
                robot2 = RobotList.Find(x => x.name == prevLine[1]);
                if (robot1 == null)
                {
                    robot1 = new Robot(prevLine[0]);
                    RobotList.Add(robot1);
                }
                if (robot2 == null)
                {
                    robot2 = new Robot(prevLine[1]);
                    RobotList.Add(robot2);
                }

                foreach (var nr in splitCollNrs)
                {
                    if (nr.Length == 0) continue;
                    var colNr = short.Parse(nr);
                    Collision newCollision = new Collision(colNr, robot1, robot2);
                    robot1.AddCollision(newCollision);
                    robot2.AddCollision(newCollision);
                    if (!CollisionList.Contains(newCollision)) CollisionList.Add(newCollision);
                }
                prevLine = line;
            }
        }

        public void ReadInterlockProcessFromCsv()
        {
            string[] line = null, prevLine = null;
            string robotName = null;
            string[] interlockProcess;
            List<int> interlockProcessList = new List<int>();
            while (!csvParser.EndOfData)
            {
                line = csvParser.ReadFields();
                robotName = line[0];
                var robot = RobotList.Find(x => x.name == robotName);
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
            foreach (var robot in RobotList)
            {
                robot.CreateRobotStates();
            }
        }

    }
}
