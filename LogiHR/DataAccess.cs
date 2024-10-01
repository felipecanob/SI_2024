using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace LogiHR
{
    internal class DataAccess
    {
        private SqlConnection sqlcon;
        public SqlConnection Sqlcon
        {
            get { return this.sqlcon; }
            set { this.sqlcon = value; }
        }

        private SqlCommand sqlcom;
        public SqlCommand Sqlcom
        {
            get { return this.sqlcom; }
            set { this.sqlcom = value; }
        }

        private SqlDataAdapter sda;
        public SqlDataAdapter Sda
        {
            get { return this.sda; }
            set { this.sda = value; }
        }

        private DataSet ds;
        public DataSet Ds
        {
            get { return this.ds; }
            set { this.ds = value; }
        }

        //internal DataTable dt;

        public DataAccess()
        {
            string connectionString = GetConnectionStringFromXml();
            this.Sqlcon = new SqlConnection(connectionString);
            //this.Sqlcon = new SqlConnection(@"Data Source=FELIPE-PC\T;Initial Catalog=Test;Persist Security Info=True;User ID=FELIPE-PC\felip;Integrated Security=True");
            Sqlcon.Open();
        }

        // Load connection string from XML file
        private string GetConnectionStringFromXml()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string xmlFilePath = Path.Combine(exeDirectory, "DbConfig.xml");

            if (!File.Exists(xmlFilePath))
            {
                throw new FileNotFoundException("Database configuration file not found: " + xmlFilePath);
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            // Get the XML node values
            XmlNode databaseNode = xmlDoc.SelectSingleNode("configuration/database");
            string dataSource = databaseNode["dataSource"].InnerText;
            string initialCatalog = databaseNode["initialCatalog"].InnerText;
            string integratedSecurity = databaseNode["integratedSecurity"].InnerText;
            string userID = databaseNode["userID"]?.InnerText;
            string password = databaseNode["password"]?.InnerText;

            // Build the connection string
            string connectionString;
            if (integratedSecurity.ToLower() == "true")
            {
                connectionString = $"Data Source={dataSource};Initial Catalog={initialCatalog};Integrated Security=True;";
            }
            else
            {
                connectionString = $"Data Source={dataSource};Initial Catalog={initialCatalog};User ID={userID};Password={password};";
            }

            return connectionString;
        }

        private void QueryText(string query)
        {
            this.Sqlcom = new SqlCommand(query, this.Sqlcon);
        }

        public DataSet ExecuteQuery(string sql)
        {
            this.Sqlcom = new SqlCommand(sql, this.Sqlcon);
            this.Sda = new SqlDataAdapter(this.Sqlcom);
            this.Ds = new DataSet();
            this.Sda.Fill(this.Ds);
            return Ds;
        }

        public int ExecuteDMLQuery(string sql)
        {
            this.Sqlcom = new SqlCommand(sql, this.Sqlcon);
            int u = this.Sqlcom.ExecuteNonQuery();
            return u;
        }
    }

}
