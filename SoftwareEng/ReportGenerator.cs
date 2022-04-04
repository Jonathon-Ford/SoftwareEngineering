﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEng
{
    public class ReportGenerator
    {
        public static int Average(List<int> array)
        {
            int sum = 0;
            for(int i = 0; i < array.Count; i++)
            {
                sum += array[i];
            }

            return sum/ array.Count;
        }
        /*
         * 
         */
        public static void GenerateInsentiveReport()
        {

        }
        /* This function writes the 30 day expected occupancy report and saves it to a file
         * 
         */
        public static void GenerateThirtyDayOccupancyReport()
        {
            List<List<int>> occupancies = PreparedStatements.GetThirtyDayOccupancyInfo();

            DateTime curDate = DateTime.Now;

            Console.WriteLine("Occupancy Report ----- Generated: " + curDate.ToString("MM/dd/yyyy h:mm tt"));

            List<int> totals = new List<int>();

            String data = String.Format("{0,-10} {1,-10} {2,-10} {3, -10} {4, -10} {5, -10} \n",
                    "Date", "Prepaid", "60 Day", "Conventional", "incentive", "Total");
            for (int i = 0; i < 30; i++)
            {
                string date = curDate.ToString("MM/dd/yyyy");
                /* occupancies is in the form:
                 * [
                 *    Day1 : {#ofprepaid, #ofsixtyday, #ofconventional, #ofincentive},
                 *    Day2 : ...
                 * ]
                 */
                int prepaidNum = occupancies[i][0];
                int sixtyDayNum = occupancies[i][1];
                int conventionalNum = occupancies[i][2];
                int incentiveNum = occupancies[i][3];
                int total = prepaidNum + sixtyDayNum + conventionalNum + incentiveNum;
                totals.Add(total);

                
                data += String.Format("{0,-10} {1,-10} {2,-10} {3, -10} {4, -10} {5, -10} \n",
                    date, prepaidNum, sixtyDayNum, conventionalNum, incentiveNum, total);

                curDate.AddDays(1);
            }

            int average = Average(totals);

            data += "Average occupancy: " + average;

            Console.WriteLine($"\n{data}");

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "ThirtyDayOccupancy.txt")))
            {
                    outputFile.WriteLine($"\n{data}");
            }
        }
        /* This function prints the daily occupancy report to the screen, and saves it to a file in documents
         * 
         * The report takes the form...
         * Room      Name                                 Departure Date
         * #         *name                                today
         * #         name                                 somedate after today
         * #         name 
         */
        public static void GenerateDailyOccupancyReport()
        {
            List<Reservations> dailyOccupancy = PreparedStatements.GetTodaysOccupancies();

            String data = String.Format("{0,-10} {1,-50} {2,-10} \n",
                    "Room", "Name", "Departure Date");

            for (int i = 0; i < dailyOccupancy.Count; i++)
            {
                int roomNum = dailyOccupancy[i].RoomNum;

                string name = dailyOccupancy[i].FirstName + " " + dailyOccupancy[i].LastName;

                string date;

                if(dailyOccupancy[i].StartDate.Date < DateTime.Now.Date)
                {
                    date = dailyOccupancy[i].EndDate.ToString("dd/MM/yyyy");
                }
                else
                {
                    date = "";
                }

                if(dailyOccupancy[i].EndDate.Date == DateTime.Now.Date)
                {
                    name = "*" + name;
                }


                data += String.Format("{0,-10} {1,-50} {2,-10} \n",
                    roomNum, name, date);
            }

            Console.WriteLine($"\n{data}");

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "DailyOccupancy.txt")))
            {
                outputFile.WriteLine($"\n{data}");
            }
        }
    }
}
