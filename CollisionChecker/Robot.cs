using System;
using System.Collections.Generic;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;

namespace CollisionChecker
{
    class Robot : IEquatable<Robot>
    {
        public string name;
        public List<Collision> collisionList = new List<Collision>();
        public List<List<int>> interlockProcess2 = new List<List<int>>();
        public List<String> interlockProcessId = new List<String>();
        public List<RobotState> robotStates = new List<RobotState>();
        public bool stopped = false;
        public int activeRobotState = 0;
        public bool InHome { get; set; }

        public Robot(string name)
        {
            this.name = name;
            this.InHome = true;
            this.CheckStarted = false;
        }

        public bool CheckStarted { get; set; }

        /// <summary>
        /// Adds a new collision if doesn't exist and sorts collision list.
        /// </summary>
        /// <param name="collision"></param>
        public void AddCollision(Collision collision)
        {
            if (!collisionList.Contains(collision)) collisionList.Add(collision);
            IComparer<Collision> compMethod = new Collision();
            collisionList.Sort(compMethod);
        }

        /// <summary>
        /// Changes active robot state to next one.
        /// </summary>
        /// <returns>True if succeed, false if actual state is the last one in the list.</returns>
        public bool SwitchToNextState()
        {
            if (this.activeRobotState < robotStates.Count-1)
            {
                this.activeRobotState++;
                if (GetCurrentState().positionInInterlockProcess == 0) InHome = true;
                else InHome = false;
                return true;
            }
            else
            {
                this.activeRobotState = 0;
                if (GetCurrentState().positionInInterlockProcess == 0) InHome = true;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TakeCurrentStateCollisions()
        {
            if (GetCurrentState().TakeCollisions()) return true;
            else return false;
        }

        public void SaveCollisionListToExcel(Excel.Application excelApp)        //TODO
        {
            Excel.Workbook excelWorkbook = excelApp.ActiveWorkbook;
            Excel._Worksheet excelSheet = excelWorkbook.ActiveSheet;
            Excel.Range allCells = excelSheet.Cells;
            foreach (var collision in collisionList)
            {

            }
        }

        public void SaveDescriptionToExcel(Excel._Worksheet excelSheet, Excel.Range allCells, ref int rowNr, ref int colNr)
        {
            object firstRngCell, lastRngCell;
            Excel.Range operatingRange;

            firstRngCell = (Excel.Range)allCells[rowNr, colNr];
            allCells[rowNr, colNr] = this.name;
            operatingRange = excelSheet.Range[allCells[rowNr, colNr], allCells[rowNr + 1, colNr]];
            operatingRange.Merge();
            colNr++;
            allCells[rowNr, colNr] = GetCurrentInterlockProcessId();
            operatingRange = excelSheet.Range[allCells[rowNr, colNr], allCells[rowNr + 1, colNr]];
            operatingRange.Merge();
            colNr++;

            operatingRange = (Excel.Range)allCells[rowNr, colNr];
            operatingRange.Interior.Color = Const.LGreen;
            operatingRange.Value = "Take";
            operatingRange = (Excel.Range)allCells[rowNr + 1, colNr];
            operatingRange.Interior.Color = Const.DGreen;
            operatingRange.Value = "Release";
            lastRngCell = (Excel.Range)allCells[rowNr + 1, colNr];

            operatingRange = excelSheet.get_Range(firstRngCell, lastRngCell);
            operatingRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexNone, Type.Missing, Type.Missing);
            operatingRange.Borders[Excel.XlBordersIndex.xlInsideVertical].Weight = Excel.XlBorderWeight.xlThin;
            colNr++;

            excelSheet.get_Range("B:B").ColumnWidth = 10;
            excelSheet.get_Range("C:C").Columns.AutoFit();
        }

        public void SaveCollisionClearing(Excel.Range operatingRange, int collNumber, bool beforePos)
        {
            operatingRange.Value = System.Math.Abs(collNumber);
            if (beforePos)
            {
                operatingRange.Interior.Color = Const.Brown;
                operatingRange.Font.Bold = true;
                operatingRange.Font.Color = Const.White;
            }
            else operatingRange.Interior.Color = Const.DGreen;
            
            operatingRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexNone, Type.Missing, Type.Missing);
            operatingRange.ColumnWidth = 3;
        }

        public void SaveCollisionEntering(Excel.Range operatingRange, int collNumber, bool beforePos)
        {
            operatingRange.Value = System.Math.Abs(collNumber);

            if (beforePos)
            {
                operatingRange.Interior.Color = Const.Brown;
                operatingRange.Font.Bold = true;
                operatingRange.Font.Color = Const.White;
            }
            else operatingRange.Interior.Color = Const.LGreen;

            operatingRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexNone, Type.Missing, Type.Missing);
            operatingRange.ColumnWidth = 3;
        }

        public void SaveCollisionComment(Excel.Range operatingRange, int collNumber)
        {
            string targetRobotName;
            Collision currentCollision;

            operatingRange.Validation.Add(Excel.XlDVType.xlValidateInputOnly, Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, Type.Missing, Type.Missing);
            currentCollision = collisionList.Find(x => x.nr == System.Math.Abs(collNumber));

            targetRobotName = currentCollision.GetSecondRobot(this).name;
            operatingRange.Validation.InputMessage = "Collision zone with " + targetRobotName;
        }

        public void SaveStateToExcel(Excel.Workbook excelWorkbook, int rowNr, int colNr)
        {
            Excel._Worksheet excelSheet = excelWorkbook.Sheets["Possible Deadlocks"];
            Excel.Range allCells = excelSheet.Cells;
            Excel.Range operatingRange;
            int i = 0;
            object firstRngCell, lastRngCell;

            excelSheet.Activate();
            SaveDescriptionToExcel(excelSheet, allCells, ref rowNr, ref colNr);

            firstRngCell = allCells[rowNr, colNr];
            lastRngCell = allCells[rowNr+1, colNr + this.GetCurrentInterlockProcess().Count - 1];
            operatingRange = excelSheet.get_Range(firstRngCell, lastRngCell);
            operatingRange.Interior.Color = Const.White;

            foreach (var collNumber in GetCurrentInterlockProcess())
            {
                if (collNumber < 0)
                {
                    operatingRange = (Excel.Range)allCells[rowNr + 1, colNr];
                    if (i < this.GetCurrentState().positionInInterlockProcess) SaveCollisionClearing(operatingRange, collNumber, true);
                    else SaveCollisionClearing(operatingRange, collNumber, false);    
                }
                else
                {
                    operatingRange = (Excel.Range)allCells[rowNr, colNr];
                    if (i < this.GetCurrentState().positionInInterlockProcess) SaveCollisionEntering(operatingRange, collNumber, true);
                    else SaveCollisionEntering(operatingRange, collNumber, false);

                    if (i == this.GetCurrentState().positionInInterlockProcess && i != 0)
                    {
                        operatingRange = excelSheet.get_Range((Excel.Range)allCells[rowNr, colNr], (Excel.Range)allCells[rowNr + 1, colNr]);
                        operatingRange.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        operatingRange.Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlMedium;
                        operatingRange.Borders[Excel.XlBordersIndex.xlEdgeLeft].Color = Const.Red;
                        operatingRange = (Excel.Range)allCells[rowNr, colNr];
                    }
                }
                SaveCollisionComment(operatingRange, collNumber);
                i++;
                colNr++;
            }
            lastRngCell = allCells[rowNr + 1, --colNr];
            operatingRange = excelSheet.get_Range(firstRngCell, lastRngCell);
            operatingRange.BorderAround2(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium, Excel.XlColorIndex.xlColorIndexNone, Type.Missing, Type.Missing);
        }

        public RobotState GetCurrentState()
        {
            return robotStates[activeRobotState];
        }

        public List<int> GetCurrentInterlockProcess()
        {
            int currentInterlockProcessNr= GetCurrentState().interlockProcessNr;
            return interlockProcess2[currentInterlockProcessNr];
        }

        public string GetCurrentInterlockProcessId()
        {
            int currentInterlockProcessNr = GetCurrentState().interlockProcessNr;
            return interlockProcessId[currentInterlockProcessNr];
        }

        public bool IsStuckFirst(int maxSteps)
        {
            if (!stopped) return false;
            return GetCurrentState().wantedColl.GetSecondRobot(this).IsStuck(1, maxSteps);            
        }

        public bool IsStuck(int stepNr, int maxSteps)
        {
            Robot tempRobot;
            if (!stopped || stepNr == maxSteps) return false;
            if (CheckStarted) return true;
            tempRobot = GetCurrentState().wantedColl.GetSecondRobot(this);       
            return tempRobot.IsStuck(++stepNr, maxSteps);
        }

        public void SetStoppedStatus()
        {
            Collision wantedCollision = GetCurrentState().wantedColl;
            if (wantedCollision != null) this.stopped = GetCurrentState().wantedColl.IsTaken();
        }

        public static bool operator ==(Robot robot1, Robot robot2)
        {
            if (ReferenceEquals(robot1, robot2)) return true;
            bool robotNull = object.ReferenceEquals(robot1, null) || object.ReferenceEquals(robot2, null);
            if (robotNull) return false;

            bool namesOk = robot1.name == robot2.name;
            bool stoppedOk = true;// robot1.stopped == robot2.stopped;
            bool activeRobotStateOk = robot1.activeRobotState == robot2.activeRobotState;

            return namesOk && stoppedOk && activeRobotStateOk;
        } //TODO?: porownianie robotStates jeszcze

        public static bool operator !=(Robot robot1, Robot robot2)
        {
            return !(robot1 == robot2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            return Equals((Robot) obj);
        }

        public bool Equals(Robot robot)
        {
            bool robotNull = object.ReferenceEquals(this, null) || object.ReferenceEquals(robot, null);
            if (robotNull) return false;
            if (ReferenceEquals(this, robot)) return true;

            bool namesOk = this.name == robot.name;
            bool stoppedOk = true; //this.stopped == robot.stopped;
            bool activeRobotStateOk = this.activeRobotState == robot.activeRobotState;

            return namesOk && stoppedOk && activeRobotStateOk;
        }

        public override int GetHashCode()
        {
            return 3 + (3 ^ activeRobotState * name.Length);               //TODO?: rozbudowac i doczytac
        }

        public bool addInterlockProcess(List<int> interlockProcessToAdd)
        {
            foreach (var list in interlockProcess2)
            {
                if (list.SequenceEqual(interlockProcessToAdd)) return false;
            }
            interlockProcess2.Add(interlockProcessToAdd);
            return true;
        }

        public void CreateRobotStates()
        {
            RobotState tempState = new RobotState();
            short i,j, takenCount = 0;
            int coll;
            j = 0;

            foreach (var process in interlockProcess2)
            {
                tempState = new RobotState();
                tempState.AddWantedCollision(collisionList.Find(x => x.nr == process[0]));
                tempState.positionInInterlockProcess = 0;
                tempState.interlockProcessNr = j;
                robotStates.Add(tempState); 
                tempState = new RobotState();
                for (i = 0; i < process.Count - 1; i++)            // this assumes that in last step in interlockProcess collision is released;
                {
                    coll = process[i];
                    if (coll > 0)
                    {
                        takenCount++;
                        tempState.AddTakenCollision(collisionList.Find(x => x.nr == coll));
                    }
                    else if (coll < 0)
                    {
                        takenCount--;
                        tempState.RemoveTakenCollision(collisionList.Find(x => x.nr == -coll));
                    }
                    if (tempState.takenColls.Count >= 1 && process[i + 1] > 0)
                    {
                        tempState.AddWantedCollision(collisionList.Find(x => x.nr == process[i + 1]));
                        tempState.positionInInterlockProcess = (short)(i + 1);
                        tempState.interlockProcessNr = j; 
                        robotStates.Add(tempState);
                        tempState = new RobotState(tempState);
                    }
                }
                j++;
            }
        }

    }
}

