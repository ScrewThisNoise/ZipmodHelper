using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using Dapper;

namespace ScrewLib
{
    class DatabaseHandler
    {
        public static void Initialize(string dbName)
        {
            if (File.Exists($"{dbName}.db"))
            {
                var cs = $"URI=file:{dbName}.db";
                var con = new SQLiteConnection(cs);
                con.Open();
                Logger.Writer($"Successfully opened {dbName}.db.");
            }
            else
            {
                Logger.Writer($"{dbName}.db doesn't exist, creating...");
                var cs = $"URI=file:{dbName}.db";
                var con = new SQLiteConnection(cs);
                con.Open();
                Logger.Writer($"Successfully opened {dbName}.db.");
                con.Execute(
                    @"create table Mods
                        (
                            ID              INTEGER NOT NULL UNIQUE primary key AUTOINCREMENT,
                            GUID            TEXT,
                            Name            TEXT,
                            Version         TEXT,
                            Author          TEXT,
                            Game            TEXT,
                            OriginalFile    TEXT,
                            NewFile         TEXT,
                            MD5             TEXT,
                            ModName         TEXT
                        )"
                );
                Logger.Writer($"Written base schema to {dbName}.db.");
            }
        }
    }
}
