﻿using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
// To resolve the CS0246 error, you need to add a reference to the "Microsoft Jet and Replication Objects 2.6 Library" (JRO) in your project.
// This library is a COM component and is not included by default in .NET projects.
// Follow these steps to fix the issue:

// 1. Open your project in Visual Studio.
// 2. Right-click on your project in the Solution Explorer and select "Add" -> "Reference".
// 3. In the "Reference Manager" window, go to the "COM" tab.
// 4. Search for "Microsoft Jet and Replication Objects 2.6 Library" in the list.
// 5. Select it and click "OK" to add the reference to your project.
// 6. Save and rebuild your project.

// After adding the reference, the `using JRO;` directive will work without errors.
using JRO; // Add this namespace to resolve the 'JRO' reference


namespace MarioApp
{
    public class MarHelpers
    {        
        public static void ResetCompanyGlobals()
        {
            SharedGlobals.ActiveCompany = "";
            SharedGlobals.SetMarntMdvLocation("");
            SharedGlobals.CompanyName = ""; // default values
            SharedGlobals.CompanyAddress = ""; // default values
            SharedGlobals.CompanyPostalCodeAndCity = ""; // default values
            SharedGlobals.CompanyPhoneNumber = ""; // default values
            SharedGlobals.CompanyKBONumber = ""; // default values
            SharedGlobals.CompanyVATNumber = ""; // default values
            SharedGlobals.CompanyIBANNumber = ""; // default values
            SharedGlobals.CompanyBICNumber = ""; // default values
            SharedGlobals.CompanyEmailAddress = ""; // default values
            SharedGlobals.CompanyContactPerson = ""; // default values
            SharedGlobals.CompanyContactEmailAddress = ""; // default values
        }
        public static void SetCompanyGlobals(string selectedCompany)
        {
            SharedGlobals.ActiveCompany = selectedCompany;
            // Use the provided method to set the value instead of direct assignment
            SharedGlobals.SetMarntMdvLocation("\\" + selectedCompany + "\\marnt.mdv");
            string jrFile = GetHighestCounterTable(SharedGlobals.MarntMdvLocation, "jr");

            string connStr = SharedGlobals.DbJetProvider + SharedGlobals.MimDataLocation + SharedGlobals.MarntMdvLocation;
            OleDbConnection conn = new OleDbConnection
            {
                ConnectionString = connStr
            };
            conn.Open();

            string sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's046'";
            OleDbCommand cmd = new OleDbCommand
            {
                CommandText = sqlQuery,
                Connection = conn
            };
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyName = reader["v217"].ToString();
                SharedGlobals.CompanyName = companyName;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's047'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyAddress = reader["v217"].ToString();
                SharedGlobals.CompanyAddress = companyAddress;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's048'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyPostalCodeAndCity = reader["v217"].ToString();
                SharedGlobals.CompanyPostalCodeAndCity = companyPostalCodeAndCity;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's049'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyPhoneNumber = reader["v217"].ToString();
                SharedGlobals.CompanyPhoneNumber = companyPhoneNumber;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's292'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyKBONumber = reader["v217"].ToString();
                SharedGlobals.CompanyKBONumber = companyKBONumber;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's051'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyVATNumber = reader["v217"].ToString();
                SharedGlobals.CompanyVATNumber = companyVATNumber;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's293'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyIBANNumber = reader["v217"].ToString();
                SharedGlobals.CompanyIBANNumber = companyIBANNumber;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's294'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyBICNumber = reader["v217"].ToString();
                SharedGlobals.CompanyBICNumber = companyBICNumber;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's295'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyEmailAddress = reader["v217"].ToString();
                SharedGlobals.CompanyEmailAddress = companyEmailAddress;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's052'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyContactPerson = reader["v217"].ToString();
                SharedGlobals.CompanyContactPerson = companyContactPerson;
            }
            reader.Close();

            sqlQuery = "SELECT v217 FROM " + jrFile + " WHERE  v071 = 's050'";
            cmd.CommandText = sqlQuery;
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string companyContactEmailAddress = reader["v217"].ToString();
                SharedGlobals.CompanyContactEmailAddress = companyContactEmailAddress;
            }
            reader.Close();
            conn.Close();
        }                
    
        private static string GetHighestCounterTable(string location, string filter)
        {
            // Change provider depending on Access version:
            // For .mdb (Jet)
            string foundTable = string.Empty;
            string connStr = SharedGlobals.DbJetProvider + SharedGlobals.MimDataLocation + location;

            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();

            // Retrieve schema for tables only
            DataTable tables = conn.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                // Use null-coalescing operator to handle potential null values
                string tableName = row["TABLE_NAME"]?.ToString() ?? string.Empty;
                string tableType = row["TABLE_TYPE"]?.ToString() ?? string.Empty;

                // Filter out system/internal tables if needed
                if (tableType == "TABLE")
                {
                    if (tableName.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        foundTable = tableName;
                    }
                }
            }
            conn.Close();
            return foundTable;  // Return empty string if no matching table is found
        }
    }

    public class BackupHelper
    {
        public static string ZipFolderToCloudDrive(string sourceFolderPath, string cloudFolderPath, string companyFolderNumber)
        {
            if (!Directory.Exists(sourceFolderPath))
                throw new DirectoryNotFoundException($"Source folder not found: {sourceFolderPath}");

            if (!Directory.Exists(cloudFolderPath))
                throw new DirectoryNotFoundException($"Cloud folder not found: {cloudFolderPath}");

            try
            {
                string zipFileName = $"{Path.GetFileName(sourceFolderPath)}_backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                string tempZipPath = Path.Combine(Path.GetTempPath(), zipFileName);
                string destZipPath = Path.Combine(cloudFolderPath, zipFileName);

                // Create zip in temp directory
                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);

                ZipFile.CreateFromDirectory(sourceFolderPath, tempZipPath, CompressionLevel.Optimal, false);

                // Copy to OneDrive sync folder
                File.Copy(tempZipPath, destZipPath, true);

                return destZipPath;
            }
            catch (Exception)
            {
                return "Bedrijfsdata is nog in gebruik voor dit bedrijf. Eerst marIntegraal afsluiten a.u.b.";
            }            
        }
    }

    public class DatabaseHelper
        {
            public static void CompactAccessDatabase(string databasePath)
            {
                if (!File.Exists(databasePath))
                    throw new FileNotFoundException($"Database file not found: {databasePath}");
                string tempDatabasePath = Path.Combine(Path.GetDirectoryName(databasePath), "temp_compact.mdb");
                // Use JRO to compact the database
                var jro = new JetEngine(); // Ensure the JRO namespace is correctly referenced
                jro.CompactDatabase($"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={databasePath};", $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={tempDatabasePath};Jet OLEDB:Engine Type=5");
                // Replace original database with compacted one
                File.Delete(databasePath);
                File.Move(tempDatabasePath, databasePath);
                Console.WriteLine("Database compacted successfully.");
            }
        }
    }
    




