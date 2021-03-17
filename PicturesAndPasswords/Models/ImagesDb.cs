using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesAndPasswords.Models
{
    public class ImagesDb
    {
        private readonly string _connectionString;
        public ImagesDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddImage(Image image)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "insert into images (name, password, views) values (@name, @password, @views) select SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", image.Name);
            cmd.Parameters.AddWithValue("@password", image.Password);
            cmd.Parameters.AddWithValue("@views", 1);
            connection.Open();
            image.Id = (int)(decimal)cmd.ExecuteScalar();
        }
        public void UpdateImageViews(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "update images set views = (views+1) where id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public Image GetImage(int Id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select * from images where id=@id";
            cmd.Parameters.AddWithValue("@id", Id);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            Image image = new Image
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Password = (string)reader["password"],
                Views = (int)reader["views"]
            };
            return image;
        }
    }
    public class Image
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Views { get; set; }
    }
}
