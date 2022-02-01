using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course_Planner.SQLite_Interface
{
    //Interface for SQLite
    public interface Isqlite
    {
        SQLiteConnection GetConnection(); //Abstract method
    }
}
