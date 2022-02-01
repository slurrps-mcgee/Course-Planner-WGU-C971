using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Course_Planner.Droid;
using Xamarin.Forms;
using System.IO;
using Android.Provider;
using Course_Planner.SQLite_Interface;

//[assembly: Dependency(typeof(SQLiteDroid))]
[assembly: Dependency(typeof(SQLiteDroid))]
namespace Course_Planner.Droid
{
    //Inherit interface Isqlite
    public class SQLiteDroid : Isqlite
    {
        //Implement Isqlite method GetConnection
        public SQLiteConnection GetConnection()
        {
            //Name the database
            var dbase = "CoursePlannerDB";
            //get the dbpath special folder application data
            var dbpath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            //Create the path using the dbpath and dbase variables
            var path = Path.Combine(dbpath, dbase);
            //Set a new SQLiteConnection sending it the path
            var connection = new SQLiteConnection(path);
            return connection; //Return the connection
        }
    }
}