using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HRINTERNSHIP.Models
{
    public class InternModels
    {
        private DBModel db = new DBModel();
        //Admin
        public List<Intern_ListAttendance_Result> ListAllAttendance(string fromDate, string toDate)
        {
            List<Intern_ListAttendance_Result> lst = new List<Intern_ListAttendance_Result>();
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Intern_ListAttendance", con);
                com.Parameters.Add("@StartDate", SqlDbType.NVarChar, 50).Value = fromDate;
                com.Parameters.Add("@EndDate", SqlDbType.NVarChar, 50).Value = toDate;
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    lst.Add(new Intern_ListAttendance_Result
                    {
                        test = rdr["test"].ToString(),
                        KPK = rdr["KPK"].ToString(),
                        Cardholder_Name = rdr["Cardholder_Name"].ToString(),
                        ClockIn = rdr["ClockIn"].ToString(),
                        DateClockIn = rdr["DateClockIn"].ToString(),
                        ClockOut = rdr["ClockOut"].ToString(),
                        DateClockOut = rdr["DateClockOut"].ToString(),
                        Entry_Date_Time = rdr["Entry_Date_Time"].ToString(),
                        Entry_Out = rdr["Entry_Out"].ToString(),
                        Attendance_id = Convert.ToInt32(rdr["Attendance_id"]),
                        total_hour = Convert.ToInt32(rdr["total_hour"]),
                        Date = rdr["Date"].ToString(),
                        id = Convert.ToInt32(rdr["id"]),
                        Date_public_holidays = rdr["Date_public_holidays"].ToString(),
                        Date_description = rdr["Date_description"].ToString(),
                        Date_mandatory = rdr["Date_mandatory"].ToString(),
                        Type_mandatory = rdr["Type_mandatory"].ToString(),
                        Activity_ID = Convert.ToInt32(rdr["Activity_ID"]),
                        Activity = rdr["Activity"].ToString(),
                        MENTOR = rdr["MENTOR"].ToString(),
                        MANAGER = rdr["MANAGER"].ToString(),
                    });
                }
                return lst;
            }
        }

        public int Updateattendance(string kpk, string date, string dateclockin, string dateclockout, string atnid)
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Intern_UpdateAttendance", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Add("@kpk", SqlDbType.NVarChar, 50).Value = kpk;
                com.Parameters.Add("@date", SqlDbType.NVarChar, 50).Value = date;
                com.Parameters.Add("@dateclockin", SqlDbType.NVarChar, 50).Value = dateclockin;
                com.Parameters.Add("@dateclockout", SqlDbType.NVarChar, 50).Value = dateclockout;
                com.Parameters.Add("@atnid", SqlDbType.NVarChar, 50).Value = atnid;
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Intern_DeleteAttendanceNoInMaster()
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Intern_DeleteAttendanceNoInMaster", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        //Attendance Intern
        public int InsertUpdateActivity(string KPK, string Date, string Activity, string Activity_ID, string Action)
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Intern_InsertUpdateActivity", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Add("@KPK", SqlDbType.NVarChar, 50).Value = KPK;
                com.Parameters.Add("@Date", SqlDbType.NVarChar, 50).Value = Date;
                com.Parameters.Add("@Activity", SqlDbType.Text).Value = Activity;
                com.Parameters.Add("@Activity_ID", SqlDbType.NVarChar, 50).Value = Activity_ID;
                com.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = Action;
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int InsertUpdatePayment(string KPK, int TAtn, int TAbs, int HalfWorkHour, int TDay, int Report_ID, string DatePeriod, string Action)
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("Intern_InsertUpdatePayment", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.Add("@KPK", SqlDbType.NVarChar, 10).Value = KPK;
                com.Parameters.Add("@TAtn", SqlDbType.Int).Value = TAtn;
                com.Parameters.Add("@TAbs", SqlDbType.Int).Value = TAbs;
                com.Parameters.Add("@HalfWorkHour", SqlDbType.Int).Value = HalfWorkHour;
                com.Parameters.Add("@TDay", SqlDbType.Int).Value = TDay;
                com.Parameters.Add("@Report_ID", SqlDbType.Int).Value = Report_ID;
                com.Parameters.Add("@DatePeriod", SqlDbType.NVarChar, 50).Value = DatePeriod;
                com.Parameters.Add("@Action", SqlDbType.NVarChar, 10).Value = Action;
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int MergePayment()
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("AttendancePayment_bulk", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int MergeActivity()
        {
            int i;
            using (SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("AttendanceActivty_bulk", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                i = com.ExecuteNonQuery();
            }
            return i;
        }
    }
}