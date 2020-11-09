using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Models;

namespace WebsiteCrawler
{
    public class DbStuff
    {
        private readonly string _connString = "Data Source=DESKTOP-UQ2GKS7;Initial Catalog=WebsiteCrawler; Integrated Security=true;";

        public void InputLink(Links link, int CrawelOrNot = 0)
        {
            string query = "INSERT INTO Links(Link,Crawled) VALUES(@Link, @Crawled)";

            using SqlConnection sqlConn = new SqlConnection(_connString);
            using SqlCommand command = new SqlCommand(query, sqlConn);

            command.Connection.Open();

            command.Parameters.AddWithValue("@Link", link.Link).Value = link.Link;

            if (CrawelOrNot == 0)
            {
                command.Parameters.AddWithValue("@Crawled", link.Crawled).Value = link.Crawled;
            }
            else
            {
                command.Parameters.AddWithValue("@Crawled", link.Crawled).Value = CrawelOrNot;
            }

            SqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Connection.Close();
            sqlConn.Close();
        }

        public bool CheckIfExist(string url)
        {
            string query = "SELECT * FROM Links WHERE Link=@Link";

            using SqlConnection sqlConn = new SqlConnection(_connString);

            using SqlCommand command = new SqlCommand(query, sqlConn);

            command.Connection.Open();

            command.Parameters.AddWithValue("@Link", url);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                if (url == reader["Link"].ToString())
                {
                    return true;
                }
                Console.WriteLine(reader["Link"].ToString());
            }

            reader.Close();
            command.Connection.Close();
            sqlConn.Close();

            return false;
        }

        public bool CheckIfCrawled(string url)
        {
            string query = "SELECT * FROM Links WHERE Link=@Link";

            using SqlConnection sqlConn = new SqlConnection(_connString);
            using SqlCommand command = new SqlCommand(query, sqlConn);

            command.Connection.Open();

            command.Parameters.AddWithValue("@Link", url);

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int getCrawled = Convert.ToInt32(reader["Crawled"]);

                if (url == reader["Link"].ToString() && getCrawled == 1)
                {
                    return true;
                }
            }

            reader.Close();
            command.Connection.Close();
            sqlConn.Close();

            return false;
        }

        public Links GetNextNotCrawled()
        {
            string getAllQuery = "SELECT * FROM Links WHERE Crawled = 0 ORDER BY Id DESC";
            var linkModel = new Links();

            using SqlConnection sqlConn = new SqlConnection(_connString);

            using SqlCommand command = new SqlCommand(getAllQuery, sqlConn);

            command.Connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
                    
            while (reader.Read())
            {
                int GetId = Convert.ToInt32(reader["Id"]);
                int GetCrawled = Convert.ToInt32(reader["Crawled"]);
                
                linkModel.Id = GetId;
                linkModel.Link = reader["Link"].ToString();
                linkModel.Crawled = GetCrawled;
            }

            reader.Close();
            command.Connection.Close();
            sqlConn.Close();

            return linkModel;
        }

        public void UpdateLink(int id, int setCrawledTo)
        {
            string query = "UPDATE Links SET Crawled = @Crawled WHERE Id = @Id";

            using SqlConnection sqlConn = new SqlConnection(_connString);
            using SqlCommand command = new SqlCommand(query, sqlConn);
            command.Connection.Open();

            command.Parameters.AddWithValue("@Id", id).Value = id;

            var commandAddCrawled = command.Parameters.AddWithValue("@Crawled", setCrawledTo);

            if (setCrawledTo == 1)
            {
                commandAddCrawled.Value = 1;
            }
            else
            {
                commandAddCrawled.Value = 2;
            }

            SqlDataReader reader = command.ExecuteReader();

            reader.Close();
            command.Connection.Close();
            sqlConn.Close();
        }
    }
}
