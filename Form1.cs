using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OSProjectFinal
{
    public partial class Form1 : Form
    {
        enum Algorithm { FCFS, SJF, PRIORITY, ROUNDROBIN };
        static Process[] inputProcessesArr;
        static Process[] sortedProcessesArr;
        static Algorithm algorithm;
        static List<Process> excutedProcess;
        public Form1()
        {

            InitializeComponent();
            algorithm = Algorithm.SJF;
            excutedProcess = new List<Process>();
            
        }

        //GUI CONTROLS HANDELLERS
        private void createBtn_Click(object sender, EventArgs e)
        {
            rescheduleBtn.Enabled=true;
            excutedProcess.Clear();
            inputProcessesArr = new Process[(Int32)numericUpDown1.Value];
            for (int i = 0; i < inputProcessesArr.Length; i++)
            {
                inputProcessesArr[i] = new Process();
            }
            schedule();
            updateGrid();
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton obj = (RadioButton)sender;
            if (obj == radioButton1)
            {
                algorithm = Algorithm.FCFS;
            }
            else if (obj == radioButton2)
            {
                algorithm = Algorithm.SJF;
            }
            else if (obj == radioButton3)
            {
                algorithm = Algorithm.PRIORITY;
            }
            else if (obj == radioButton4)
            {
                algorithm = Algorithm.ROUNDROBIN;
            }

        }
        private void rescheduleBtn_Click(object sender, EventArgs e)
        {
            excutedProcess.Clear();
            schedule();
            updateGrid();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            rescheduleBtn.Enabled = false;

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //ALGORITHMS AND FUNCTIONS
        private  void schedule()
        {
           
            sortedProcessesArr= new Process[inputProcessesArr.Length];
            for (int i = 0; i < inputProcessesArr.Length; i++) {
                sortedProcessesArr[i] = Process.Copy(inputProcessesArr[i]);
            
            }
           // Array.Copy(inputProcessesArr,sortedProcessesArr,inputProcessesArr.Length);
            Array.Sort(sortedProcessesArr);
            switch (algorithm)
            {
                case Algorithm.FCFS:
                    FCFS();
                    // Array.Sort(sortedProcessesArr, new CompareByArrivalTime());
                    break;
                case Algorithm.SJF:
                    SJF();
                    //  Array.Sort(sortedProcessesArr, new CompareByBurstTime());
                    break;
                case Algorithm.PRIORITY:
                    PF();
                    //  Array.Sort(sortedProcessesArr, new CompareByPriority());
                    break;

                case Algorithm.ROUNDROBIN:
                    RRoubin();
                    break;
            }



        }
        private  void updateGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            DataTable dt = new DataTable();
            float avgTurn = 0,avgWait=0;
            int count = 0;
          //  StreamWriter file = new StreamWriter(@"D:\file.txt");
            foreach (Process p in excutedProcess)
            {
                DataRow dr = dt.NewRow();
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[count++].Clone();
                row.Cells[0].Value = p.ArrivalTime.ToString();
                row.Cells[1].Value = p.BurstTime.ToString();
                row.Cells[2].Value = p.Priority.ToString();
                row.Cells[3].Value = p.WaitingTime.ToString();
                row.Cells[4].Value = p.TurnAroundTime.ToString(); 
          
                avgTurn += p.TurnAroundTime;
                avgWait += p.WaitingTime;
                dataGridView1.Rows.Add(row);
            }
           // dataGridView1.DataSource = dt;
            
            avgWait /= excutedProcess.Count;
            avgTurn /= excutedProcess.Count;
            waitingTextBox.Text = avgWait.ToString();
            turnTextBox.Text = avgTurn.ToString();
          //  file.Close();

        }
        private  int arrivedProcesse(ref List<Process> currentProcesse, ref int elapsedTime, int lastIndex)
        {



            int j = lastIndex;
            for (; j < sortedProcessesArr.Length; j++)
            {
                if (sortedProcessesArr[j].ArrivalTime > elapsedTime)
                    break;
                else
                    currentProcesse.Add(sortedProcessesArr[j]);

            }

            if (currentProcesse.Count == 0)
            {
                j = lastIndex;
                for (; j < sortedProcessesArr.Length; j++)
                {
                    if (sortedProcessesArr[j].ArrivalTime > sortedProcessesArr[lastIndex].ArrivalTime)
                        break;

                    else
                        currentProcesse.Add(sortedProcessesArr[j]);

                }
                if (elapsedTime < currentProcesse.First().ArrivalTime)
                {
                    elapsedTime = currentProcesse.First().ArrivalTime;
                }
            }


            return j;

        }
        private  void SJF()
        {
            excutedProcess.Clear();
            //  currentProcesse1 = new SortedSet<Process>();
            // SortedDictionary<Process,int>currentProcesse1=new Dictionary<Process,int>(new CompareByBurstTime());
            List<Process> currentProcesse = new List<Process>();
            currentProcesse.Add(sortedProcessesArr[0]);

            int lastIndex = 1;
            int elapsedTime = sortedProcessesArr[0].ArrivalTime;
            while (currentProcesse.Count > 0 || lastIndex < sortedProcessesArr.Length)
            {
                lastIndex = arrivedProcesse(ref currentProcesse, ref elapsedTime, lastIndex);
                currentProcesse.Sort(new CompareByBurstTime());
                int index=0;
                for (int i = 1; i < currentProcesse.Count && currentProcesse[i].BurstTime == currentProcesse[0].BurstTime; i++)
                {
                 
                        index = (currentProcesse[i].ArrivalTime < currentProcesse[index].ArrivalTime) ? i : index;
                   
                    
                }
                if (index != 0 )
                {
                    Process temp = currentProcesse[index];
                    currentProcesse[index] = currentProcesse[0];
                    currentProcesse[0] = temp;
                   
                }
                currentProcesse.First().WaitingTime = elapsedTime - currentProcesse.First().ArrivalTime;
                elapsedTime += currentProcesse.First().BurstTime;
                currentProcesse.First().TurnAroundTime = currentProcesse.First().WaitingTime + currentProcesse.First().BurstTime;
                excutedProcess.Add(currentProcesse.First());
                currentProcesse.Remove(currentProcesse.First());

                Console.WriteLine(elapsedTime);
            }

        }
        private  void PF()
        {
            excutedProcess.Clear();
            //  currentProcesse1 = new SortedSet<Process>();
            // SortedDictionary<Process,int>currentProcesse1=new Dictionary<Process,int>(new CompareByBurstTime());
            List<Process> currentProcesse = new List<Process>();
            currentProcesse.Add(sortedProcessesArr[0]);

            int lastIndex = 1;
            int elapsedTime = sortedProcessesArr[0].ArrivalTime;
            while (currentProcesse.Count > 0 || lastIndex < sortedProcessesArr.Length)
            {
                lastIndex = arrivedProcesse(ref currentProcesse, ref elapsedTime, lastIndex);
                currentProcesse.Sort(new CompareByPriority());
                int index = 0;
                for (int i = 1; i < currentProcesse.Count && currentProcesse[i].Priority == currentProcesse[0].Priority; i++)
                {
                    index = (currentProcesse[i].ArrivalTime < currentProcesse[index].ArrivalTime) ? i : index;
                        
                }
                if (index != 0 )
                {
                    Process temp = currentProcesse[index];
                    currentProcesse[index] = currentProcesse[0];
                    currentProcesse[0] = temp;
                   
                }

                currentProcesse.First().WaitingTime = elapsedTime - currentProcesse.First().ArrivalTime;
                elapsedTime += currentProcesse.First().BurstTime;
                currentProcesse.First().TurnAroundTime = currentProcesse.First().WaitingTime + currentProcesse.First().BurstTime;
                excutedProcess.Add(currentProcesse.First());
                currentProcesse.Remove(currentProcesse.First());

                Console.WriteLine(elapsedTime);
            }


        }
        private  void FCFS()
        {
            int elapsedTime = sortedProcessesArr[0].ArrivalTime+sortedProcessesArr[0].BurstTime;
            excutedProcess.Add(sortedProcessesArr[0]);
            excutedProcess[0].TurnAroundTime = excutedProcess[0].BurstTime;
            for (int i = 1; i < sortedProcessesArr.Length; i++)
            {
                Process p = sortedProcessesArr[i];
                if (elapsedTime < p.ArrivalTime)
                {
                    elapsedTime = p.ArrivalTime;
                    
                }
                p.WaitingTime = elapsedTime - p.ArrivalTime;
                p.TurnAroundTime = p.WaitingTime + p.BurstTime;
                elapsedTime += p.BurstTime;
                excutedProcess.Add(p);


            }

        }
        private void RRoubin()
        {
            excutedProcess.Clear();
            List<Process> currentProcesse = new List<Process>();
            currentProcesse.Add(sortedProcessesArr[0]);
            const int QUANTUM = 2;

            int lastIndex = 1;
            int elapsedTime = sortedProcessesArr[0].ArrivalTime;
            lastIndex = arrivedProcesse(ref currentProcesse, ref elapsedTime, lastIndex);

            while (currentProcesse.Count > 0 || lastIndex < sortedProcessesArr.Length)
            {
                if (currentProcesse.Count == 0)
                    lastIndex = lastIndex = arrivedProcesse(ref currentProcesse, ref elapsedTime, lastIndex);
                //if (currentProcesse.Count > 0)
                elapsedTime += (currentProcesse[0].RemainingTime < QUANTUM) ? currentProcesse[0].RemainingTime : QUANTUM;

                lastIndex = arrivedProcesse(ref currentProcesse, ref elapsedTime, lastIndex);

                currentProcesse[0].RemainingTime -= QUANTUM;

                if (currentProcesse[0].RemainingTime <= 0)
                {
                    currentProcesse[0].TurnAroundTime = elapsedTime - currentProcesse[0].ArrivalTime;
                    currentProcesse[0].WaitingTime = currentProcesse[0].TurnAroundTime - currentProcesse[0].BurstTime;
                    excutedProcess.Add(currentProcesse[0]);
                    currentProcesse.Remove(currentProcesse[0]);

                }
                else
                {
                    currentProcesse.Add(currentProcesse[0]);
                    currentProcesse.Remove(currentProcesse[0]);


                }



            }


        }
        private void numericUpDown1_ValueChanged(object sender, KeyPressEventArgs e)
        {
            rescheduleBtn.Enabled = false;

        }

        


    }
    class CompareByBurstTime : IComparer<Process>
    {
        public int Compare(Process x, Process y)
        {
            return x.BurstTime.CompareTo(y.BurstTime);

        }

    }
    class CompareByPriority : IComparer<Process>
    {
        public int Compare(Process x, Process y)
        {
            return x.Priority.CompareTo(y.Priority);
        }
    }
    class Process : IComparable<Process>
    {

        public int WaitingTime { get; set; }
        public int TurnAroundTime { get; set; }
        public int BurstTime { get; set; }
        public int ArrivalTime { get; set; }
        public int Priority { get; set; }
        public int RemainingTime { get; set; }
        public static Random rand = new Random();
        public static Process Copy(Process that) {
            Process Instance = new Process();
            Instance.BurstTime = that.BurstTime;
            Instance.ArrivalTime = that.ArrivalTime;
            Instance.Priority = that.Priority;
            return Instance;
        }
        public Process()
        {
            // Random rand = new Random();
            BurstTime = rand.Next(1, 10);
            ArrivalTime = rand.Next(0, 10);
            Priority = rand.Next(0, 4);
            RemainingTime = BurstTime;
        }
        public int CompareTo(Process other)
        {
            return this.ArrivalTime.CompareTo(other.ArrivalTime);
        }
        public String ToString()
        {
            return "Burst Time:: " + BurstTime + " Arrival Time:: " + ArrivalTime + " Priority:: " + Priority + " Waiting Time:: " + WaitingTime + " TurnAround:: " + TurnAroundTime;
        }
    }
}
