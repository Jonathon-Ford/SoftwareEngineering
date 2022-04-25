/*
 * Author: Jonathon Ford
 * 
 * This class contains all of the logic for generating reports for the hotel
 * 
 * Methods contained:
 * 
 * Average - Calculates the average of an array of ints
 * AveragePrice - Calculates the average of an array of floats
 * PrintToConsoleAndSaveToDocs - This prints whatever string is sent to console and saves it to a file in documents with the name passed
 * GenerateIncentiveReport - Prints the incentive report and creates a text file with the information
 * GenerateThirtyDayOccupancyReport - Prints the 30 day occupancy report and creates a text file with the information
 * GenerateOccupancyReport - Prints the names and room numbers of everyone staying at the hotel
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareEng
{
    public class ReportGenerator
    {
        /* Takes an array of ints
         * Returns the average of the array 
         */
        public static int Average(List<int> array)
        {
            int sum = 0;
            for(int i = 0; i < array.Count; i++)
            {
                sum += array[i];
            }

            return sum/ array.Count;
        }
        /* Takes an array of floats
         * Returns the average of the array
         */
        public static float AveragePrice(List<float> array)
        {
            float sum = 0;
            for (int i = 0; i < array.Count; i++)
            {
                sum += array[i];
            }

            return sum / array.Count;
        }

        /* Takes data to print and save, and name with no .txt on end. this adds .txt for you
         * 
         */
        public static void PrintToConsoleAndSaveToDocs(String data, string name)
        {
            Console.WriteLine($"\n{data}");

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, name +".txt")))
            {
                outputFile.WriteLine($"\n{data}");
            }
        }

        /* This function writes the 30 day incentive report to console and saves it to a file in documents
         * 
         */
        public static void GenerateInsentiveReport()
        {
            String data = "Incentive Report ----- Generated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\n";
            List<float> losses = PreparedStatements.GetIncentiveReportInfo();
            data += String.Format("{0,-10} {1,-10} \n",
                    "Date", "Total loss");

            float complete_total = 0;
            DateTime curDate = DateTime.Now.Date;
            for (int i = 0;i < 30; i++)
            {
                float total = losses[i];

                string date = curDate.ToString("dd/MM/yyyy");

                data += String.Format("{0,-10} {1,-10} \n",
                    date, total);
                curDate.AddDays(1);
                complete_total += total;
            }
            float avg = AveragePrice(losses);

            data += "Total loss over all 30 days: " + complete_total;
            data += "Average loss over 30 days: " + avg;

            PrintToConsoleAndSaveToDocs(data, "IncentiveReport");
        }

        /* This function writes the 30 day expected occupancy report and saves it to a file
         * 
         * This report takes the form:
         * Date     Prepaid     60 day      Conventional        Incentive       Total
         * curdate  10          4           15                  12              41
         */
        public static void GenerateThirtyDayOccupancyReport()
        {
            List<List<int>> occupancies = PreparedStatements.GetThirtyDayOccupancyInfo();

            DateTime curDate = DateTime.Now;

            String data = "30 Day Occupancy Report ----- Generated: " + curDate.ToString("MM/dd/yyyy h:mm tt") + "\n";

            List<int> totals = new List<int>();

            data += String.Format("{0,-15} {1,-15} {2,-15} {3, -15} {4, -15} {5, -15} \n",
                    "Date", "Prepaid", "60 Day", "Conventional", "Incentive", "Total");
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

                
                data += String.Format("{0,-15} {1,-15} {2,-15} {3, -15} {4, -15} {5, -15} \n",
                    date, prepaidNum, sixtyDayNum, conventionalNum, incentiveNum, total);

                curDate.AddDays(1);
            }

            int average = Average(totals);

            data += "Average occupancy: " + average;

            PrintToConsoleAndSaveToDocs(data, "ThirtyDayOccupancyReport");
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

            String data = "Daily Occupancy Report ----- Generated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\n";
            data += String.Format("{0,-10} {1,-50} {2,-15} \n",
                    "Room", "Name", "Departure Date");

            for (int i = 0; i < dailyOccupancy.Count; i++)
            {
                int roomNum = dailyOccupancy[i].RoomNum;

                string name = dailyOccupancy[i].FirstName + " " + dailyOccupancy[i].LastName;

                string date;

                if(dailyOccupancy[i].StartDate < DateTime.Now) // If they stayed the night before print when they leave
                {
                    date = dailyOccupancy[i].EndDate.ToString("dd/MM/yyyy");
                }
                else // Otherwise do not do that
                {
                    date = "";
                }

                if(dailyOccupancy[i].EndDate == DateTime.Now) // If they leave today add a * before their name
                {
                    name = "*" + name;
                }


                data += String.Format("{0,-10} {1,-50} {2,-10} \n",
                    roomNum, name, date);
            }

            PrintToConsoleAndSaveToDocs(data, "DailyOccupancyReport");
        }

        /*This function prints the daily arrivals report to the screen and saves it to a file in documents
         * 
         * Report takes the form:
         * Name     Reservation Type        Room Number     Departure Date
         * 
         */
        public static void GenerateDailyArrivalsReport()
        {
            String data = "Daily Arrivals Report ----- Generated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\n";
            data += String.Format("{0,-50} {1,-20} {2,-15} {3, -15} \n",
                    "Name", "Reservation Type", "Room Number", "Departure Date");

            List<Reservations> reservations = PreparedStatements.GetDailyArrivals();

            for(int i = 0; i < reservations.Count; i++)
            {
                string name = reservations[i].FirstName + reservations[i].LastName;
                int resoType = reservations[i].ReservationType.ReservationID;
                string reso;
                switch (resoType)
                {
                    case 1: reso = "Prepaid"; break;
                    case 2: reso = "60 Day"; break;
                    case 3: reso = "Conventional"; break;
                    case 4: reso = "Incentive"; break;
                    default: reso = "Error"; break;
                }
                int roomNum = reservations[i].RoomNum;
                string endTime = reservations[i].EndDate.ToString("dd/MM/yyyy");

                data += String.Format("{0,-50} {1,-20} {2,-15} {3, -15} \n",
                    name, reso, roomNum, endTime);
            }

            PrintToConsoleAndSaveToDocs(data, "DailyArrivalsReport");
        }

        /* This function prints the 30 day incentive report to the console and saves it to a file in documents
         * 
         * Report takes the form:
         * Date             Income
         */
        public static void GenerateThirtyDayIncomeReport()
        {
            String data = "30 Day Income Report ----- Generated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\n";

            data += String.Format("{0,-20} {1,-20} \n",
                    "Date", "Total Income");

            List<float> income = PreparedStatements.GetThirtyDayIncomeInfo();

            DateTime curDate = DateTime.Now.Date;
            float total = 0;
            foreach (float x in income)
            {
                total += x;

                string date = curDate.ToString("dd/MM/yyyy");

                data += String.Format("{0,-20} {1,-20} \n",
                    date, x);
                curDate.AddDays(1);
            }

            float avg = AveragePrice(income);

            data += "Total expected income: " + total;
            data += "Average daily income: " + avg;

            PrintToConsoleAndSaveToDocs(data, "ExpectedIncomeReport");
        }

        /* This function prints a customers bill to the console 
         * Takes in: A payment
         * Outputs: A bill for that payment, includes all changes made to the reservation and each days price
         * 
         * ***Warning this function has not been tested as well as the others***
         * 
         */
        public static void GenerateBill(Payments payment)
        {
            List<Reservations> billableResos = PreparedStatements.GetAllResosToBeBilled(payment);

            String data = "Bill Generated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\n";
            bool changeFee = false;

            if(billableResos.Count > 1)//If the reservation was changed to another date at least once, print the old dates and old price
            {
                changeFee = true;

                data += "Origonal reservation:\n";
                for(int i = billableResos.Count - 1; i >= 1; i--)
                {
                    DateTime start = billableResos[i].StartDate;
                    DateTime end = billableResos[i].EndDate;
                    float price = billableResos[i].Price;

                    data += "-----------------------------------------------\n"
                        + "Old start date: " + start.ToString("dd/MM/yyyy") + "\n"
                        + "Old end date: " + end.ToString("dd/MM/yyyy") + "\n"
                        + "Old total price (not billed): " + price + "\n"
                        + "Changed To:\n" 
                        + "----------------------------------------------\n";
                }
            }

            data += "Final Reservation:\n";
            data += String.Format("{0,-20} {1,-20} {2,-20} {3,-20}\n",
                    "Date", "Base Rate", "Discount/Fee", "Price");
            for (int i = 0; i < billableResos[0].BaseRates.Count; i++)
            {
                float dis_fee;
                if (changeFee)
                {
                    dis_fee = (float)1.1;
                }
                else
                {
                    dis_fee = billableResos[0].ReservationType.PercentOfBase;
                }

                float price = dis_fee * billableResos[0].BaseRates.ElementAt(i).Rate;

                data += String.Format("{0,-20} {1,-20} {2,-20} {3,-20}\n",
                    billableResos[0].BaseRates.ElementAt(i).EffectiveDate.ToString("dd/MM/yyyy"),
                    billableResos[0].BaseRates.ElementAt(i).Rate,
                    dis_fee, price);
            }

            data += "Total: " + billableResos[0].Price;

            PrintToConsoleAndSaveToDocs(data, "MostRecentBill");
        }
    }
}
