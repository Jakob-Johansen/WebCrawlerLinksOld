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

        public void InputLink(Links link)
        {
            string query = "INSERT INTO Links(Link,Crawled) VALUES(@Link, @Crawled)";

            using (SqlConnection sqlConn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Connection.Open();

                command.Parameters.AddWithValue("@Link", link.Link).Value = link.Link;
                command.Parameters.AddWithValue("@Crawled", link.Crawled).Value = link.Crawled;

                SqlDataReader reader = command.ExecuteReader();
                reader.Close();
            }
        }

        public bool CheckIfExist(string url)
        {
            string query = "SELECT * FROM Links WHERE Link=@Link";

            using (SqlConnection sqlConn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Connection.Open();

                command.Parameters.AddWithValue("@Link", url);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (url == reader["Link"].ToString())
                        {
                            return true;
                        }
                        Console.WriteLine(reader["Link"].ToString());
                    }
                    command.Connection.Close();
                }
            }
            return false;
        }

        public List<Links> GetAllNotCrawled()
        {
            string getAllQuery = "SELECT * FROM Links WHERE Crawled = 0";
            List<Links> linkList = new List<Links>();

            using (SqlConnection sqlConn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(getAllQuery,sqlConn);
                command.Connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int GetId = Convert.ToInt32(reader["Id"]);
                        int GetCrawled = Convert.ToInt32(reader["Crawled"]);

                        var linkModel = new Links()
                        {
                            Id = GetId,
                            Link = reader["Link"].ToString(),
                            Crawled = GetCrawled
                        };

                        linkList.Add(linkModel);
                    }
                    command.Connection.Close();
                }
            }
            return linkList;
        }

        public void UpdateLink(int id)
        {
            string query = "UPDATE Links SET Crawled = '1' WHERE Id = @Id";

            using (SqlConnection sqlConn = new SqlConnection(_connString))
            {
                SqlCommand command = new SqlCommand(query, sqlConn);
                command.Connection.Open();

                command.Parameters.AddWithValue("@Id", id).Value = id;

                SqlDataReader reader = command.ExecuteReader();
                command.Connection.Close();
            }
        }
    }
}
