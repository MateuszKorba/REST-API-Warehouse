using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApi3.Models;

namespace WebApi3.Services
{
    public class ProductService2 : IProductService2
    {
        private string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";

        public async Task<string> AddNewProductFromProc(Product product)
        {
            int idOrder = 0;
            int IdProduct = 0;
            int IdPW = 0;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    var sql = "AddProductToWarehouse";
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = sql;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IdProduct", product.IDProduct);
                        command.Parameters.AddWithValue("@IdWarehouse", product.IDWarehouse);
                        command.Parameters.AddWithValue("@Amount", product.Amount);
                        command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);

                        await command.ExecuteNonQueryAsync();
                        

                        sql = $"SELECT * FROM [Order] WHERE IdProduct = {product.IDProduct} AND Amount = {product.Amount} AND CreatedAt<'{product.CreatedAt}'";
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync())
                        {
                            if (sqlDataReader.Read())
                            {
                                idOrder = sqlDataReader.GetInt32(0);
                            }
                        }
                        sql = $"SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdProduct = {product.IDProduct} AND IdOrder = {idOrder}";
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync())
                        {
                            if (sqlDataReader.Read())
                            {
                                IdPW = int.Parse(sqlDataReader["IdProductWarehouse"].ToString());
                            }
                        }
                        await transaction.CommitAsync();
                        await connection.CloseAsync();
                        return $"200 Nadano ID = {IdPW}";
                    }
                }
            }
        }
    }
}
