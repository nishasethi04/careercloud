﻿using CareerCloud.DataAccessLayer;
using CareerCloud.Pocos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CareerCloud.ADODataAccessLayer
{
    public class CompanyJobDescriptionRepository : IDataRepository<CompanyJobDescriptionPoco>
    {
        private readonly string _connStr;

        public CompanyJobDescriptionRepository()
        {
            var config = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            config.AddJsonFile(path, false);
            var root = config.Build();
            _connStr = root.GetSection("ConnectionStrings").GetSection("DataConnection").Value;
        }
        public void Add(params CompanyJobDescriptionPoco[] items)
        {
            using (SqlConnection connection = new SqlConnection(_connStr))
            {
                foreach (CompanyJobDescriptionPoco poco in items)
                {
                    SqlCommand comm = new SqlCommand();
                    comm.Connection = connection;
                    comm.CommandText = @"INSERT INTO [dbo].[Company_Jobs_Descriptions]
                                               ([Id]
                                               ,[Job]
                                               ,[Job_Name]
                                               ,[Job_Descriptions])
                                         VALUES
                                               (@Id
                                               ,@Job
                                               ,@Job_Name
                                               ,@Job_Descriptions)";

                    comm.Parameters.AddWithValue("@Id", poco.Id);
                    comm.Parameters.AddWithValue("@Job", poco.Job);
                    comm.Parameters.AddWithValue("@Job_Name", poco.JobName);
                    comm.Parameters.AddWithValue("@Job_Descriptions", poco.JobDescriptions);


                    connection.Open();
                    int rowAffected = comm.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public void CallStoredProc(string name, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<CompanyJobDescriptionPoco> GetAll(params Expression<Func<CompanyJobDescriptionPoco, object>>[] navigationProperties)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT [Id]
                                          ,[Job]
                                          ,[Job_Name]
                                          ,[Job_Descriptions]
                                          ,[Time_Stamp]
                                      FROM [dbo].[Company_Jobs_Descriptions]";

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                CompanyJobDescriptionPoco[] pocos = new CompanyJobDescriptionPoco[2000];
                int index = 0;

                while (reader.Read())
                {
                    CompanyJobDescriptionPoco poco = new CompanyJobDescriptionPoco();

                    poco.Id = reader.GetGuid(0);
                    poco.Job = reader.GetGuid(1);
                    poco.JobName = reader.GetString(2);
                    poco.JobDescriptions= reader.GetString(3);
                    poco.TimeStamp = (byte[])reader[4];

                    pocos[index] = poco;
                    index++;
                }
                conn.Close();
                return pocos.Where(a => a != null).ToList();
            }
        }

        public IList<CompanyJobDescriptionPoco> GetList(Expression<Func<CompanyJobDescriptionPoco, bool>> where, params Expression<Func<CompanyJobDescriptionPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public CompanyJobDescriptionPoco GetSingle(Expression<Func<CompanyJobDescriptionPoco, bool>> where, params Expression<Func<CompanyJobDescriptionPoco, object>>[] navigationProperties)
        {
            IQueryable<CompanyJobDescriptionPoco> pocos = GetAll().AsQueryable();
            return pocos.Where(where).FirstOrDefault();
        }

        public void Remove(params CompanyJobDescriptionPoco[] items)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                foreach (CompanyJobDescriptionPoco poco in items)
                {
                    cmd.CommandText = @"DELETE FROM [dbo].[Company_Jobs_Descriptions]
                                       WHERE ID=@Id";
                    cmd.Parameters.AddWithValue("Id", poco.Id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void Update(params CompanyJobDescriptionPoco[] items)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                foreach (var poco in items)
                {
                    cmd.CommandText = @"UPDATE [dbo].[Company_Jobs_Descriptions]
                                       SET [Job] = @Job
                                          ,[Job_Name] = @Job_Name
                                          ,[Job_Descriptions] = @Job_Descriptions
                                       WHERE [Id]=@Id";

                    cmd.Parameters.AddWithValue("@Id", poco.Id);
                    cmd.Parameters.AddWithValue("@Job", poco.Job);
                    cmd.Parameters.AddWithValue("@Job_Name", poco.JobName);
                    cmd.Parameters.AddWithValue("@Job_Descriptions", poco.JobDescriptions);

                    conn.Open();
                    int count = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
