using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace JizdniRad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //loading values

        List<Stations> stations = new List<Stations>();
        List<TerminalTimeTable> terminalTimeTables = new List<TerminalTimeTable>();

        string CurrentStation;
        int CurrentStationY;

        //@"C:\Users\matej\Desktop\Stations.csv";
        // @"C:\Users\matej\Desktop\TerminalTimetable.csv";

       
      
        
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadValues(@"C:\Users\matej\Desktop\Stations.csv");
            LoadValues(@"C:\Users\matej\Desktop\TerminalTimetable.csv");
            CurrentStation = comboBox1.Text;
        }

        public void LoadValues(string filepath)
        {
            StreamReader sr = new StreamReader(filepath);

            string[] separators = { ";" };

            string firstline = sr.ReadLine();

            string[] firstLineWords = firstline.Split(separators, StringSplitOptions.RemoveEmptyEntries);



            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                string[] tempLine = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (firstLineWords[0] == "DayType" && firstLineWords[1] == "DepartureTime")
                {
                    TerminalTimeTable temp = new TerminalTimeTable();

                    temp.DayType = tempLine[0];
                    temp.DepartureTime = Convert.ToDateTime(tempLine[1]);
                    terminalTimeTables.Add(temp);

                }
                else if(firstLineWords[0] == "Name" && firstLineWords[1] == "TimeToReach")
                {
                    Stations temp = new Stations();

                    temp.Name = tempLine[0];
                    temp.TimeToReach = Convert.ToInt32(tempLine[1]);
                    stations.Add(temp);
                }

            }
            sr.Close();
            sr.Dispose();
        }


        private void pnl_Paint(object sender, PaintEventArgs e)
        {
            Pen blackline = new Pen(Color.Black);
            Pen border = new Pen(Color.Black, 5F);
            Font normal = new Font("Arial", 10F);
            Font bold = new Font("Arial", 10F, FontStyle.Bold);
            Font main = new Font("Arial", 14F, FontStyle.Bold);
            Brush black = new SolidBrush(Color.Black);
            Brush orange = new SolidBrush(Color.LightSalmon);

            //table
            for (int i = 0; i < 24; i++)
            {
                if (i % 2 == 0)
                {
                    e.Graphics.FillRectangle(orange, 200, 52 + i * 20, 910, 20);
                }
            }

            e.Graphics.DrawRectangle(border, 2.5F, 2.5F, 1105, 545);
            e.Graphics.DrawLine(border, 200, 0, 200, 550);
            e.Graphics.DrawLine(border, 700, 0, 700, 550);
            e.Graphics.DrawLine(border, 900, 0, 900, 550);
            e.Graphics.DrawLine(border, 0, 50, 1110, 50);

            e.Graphics.DrawLine(blackline, 220, 50, 220, 550);
            e.Graphics.DrawLine(blackline, 1085, 50, 1085, 550);
            //non dynamic texts
            e.Graphics.DrawString("Tarifní pásmo P", main, black, 45, 15);
            e.Graphics.DrawString("PRACOVNÍ DEN", main, black, 380, 15);
            e.Graphics.DrawString("SOBOTA", main, black, 760, 15);
            e.Graphics.DrawString("NEDĚLE", main, black, 970, 15);
            e.Graphics.DrawString("směr: ČERNÝ MOST", bold, black, 20, 60);


            for (int i = 0; i < 24; i++)
            {
                string text = Convert.ToString(i + 4);

                if(Convert.ToInt32(text) >= 24)
                {
                    text = Convert.ToString(Convert.ToInt32(text) - 24);
                }
                e.Graphics.DrawString(text, bold, black, 202, 52 + i * 20);
                e.Graphics.DrawString(text, bold, black, 1087, 52 + i * 20);
            }
            //stationlist
            PaintStationList(CurrentStation, e);

            PaintMinutesFromStation(CurrentStation, e);

            //times
            PaintTimesForStation(CurrentStation,"Wrk",e);
            PaintTimesForStation(CurrentStation, "Sat", e);
            PaintTimesForStation(CurrentStation, "Sun", e);

        }

        public void PaintStationList(string currStation, PaintEventArgs e)
        {
            Font normal = new Font("Arial", 10F);
            Font under = new Font("Arial", 10F, FontStyle.Underline | FontStyle.Bold);
            Brush black = new SolidBrush(Color.Black);

            for(int i = 0; i < stations.Count; i++)
            {
                if(stations[i].Name == currStation)
                {                    
                    e.Graphics.DrawString(stations[i].Name, under ,black, 25, 100 + i * 15);
                }
                else
                {
                    e.Graphics.DrawString(stations[i].Name, normal, black, 25, 100 + i * 15);
                }
            }
        }

        public void PaintTimesForStation(string currStation,string daytype, PaintEventArgs e)
        {
            List<DateTime> temp = new List<DateTime>();
            int timeAddMinutes = 0;
            int offset = 0;

            foreach(var item in stations)
            {
                if(currStation == item.Name)
                {
                    timeAddMinutes = item.TimeToReach;
                    break;
                }
            }
            
            foreach(var item in terminalTimeTables)
            {
                if(item.DayType == daytype)
                {
                    temp.Add(item.DepartureTime.AddMinutes(timeAddMinutes));
                }
            }


            switch (daytype)
            {
                case "Wrk": offset = 0;
                    break;
                case "Sat": offset = 482;
                    break;
                case "Sun": offset = 682;
                    break;
            }


            for (int i = 0; i < 24; i++)
            {

                Font normal = new Font("Arial", 10F);
                Brush black = new SolidBrush(Color.Black);
                int hour = i + 4;
                float offsetBetweenNumbers = 0;
                
                if(hour >= 24)
                {
                    hour = hour - 24;
                }

                while (true)
                {
                    if(temp.Count == 0)
                    {
                        break;
                    }

                    int currHour = temp[0].Hour;                

                    if (hour != currHour)
                    {
                        break;
                    }                   
                    e.Graphics.DrawString(Convert.ToString(temp[0].Minute), normal, black, 220 + offsetBetweenNumbers + offset, 52 + i * 20);

                    offsetBetweenNumbers += 17.5F;


                    temp.Remove(temp[0]);
                }

            }

        }

        public void PaintMinutesFromStation(string currStation, PaintEventArgs e)
        {
            Font bold = new Font("Arial", 10F, FontStyle.Bold);
            Brush black = new SolidBrush(Color.Black);
            int minutes = 0;
            foreach(var item in stations)
            {
                if(item.Name == currStation)
                {
                    minutes = item.TimeToReach;
                }
            }

            for(int i = 0; i < stations.Count; i++)
            {
                int timeToReach = stations[i].TimeToReach - minutes;
                if(timeToReach > 0)
                {
                    e.Graphics.DrawString(Convert.ToString(timeToReach), bold, black, 8, 100 + i * 15);
                }
                else if(timeToReach == 0)
                {
                    e.Graphics.DrawString("-", bold, black, 8, 100 + i * 15);
                }
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            CurrentStation = comboBox1.Text;
            this.pnl.Refresh();
        }
    }
}
