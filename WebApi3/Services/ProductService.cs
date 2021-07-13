using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using WebApi3.Models;

namespace WebApi3.Services
{
    public class ProductService : IProductService
    {
        private string _connectionString = "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True";
        public async Task<string> AddNewProduct(Product product)
        {
            if (product.IDProduct == null || product.IDProduct < 0)
            {
                return "404 Bledne ID produktu";
            }
            else if (product.IDWarehouse == null || product.IDWarehouse < 0)
            {
                return "404 Bledne ID magazynu";
            }
            else if (product.Amount == null || product.Amount < 1)
            {
                return "404 Amount musi byc wieksze od 1 i nie moze byc puste"; 
            }
            else if (product.CreatedAt == null)
            {
                return "404 Brak danych w polu CreatedAt";
            }

            

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    var sql = $"SELECT * FROM Product WHERE IdProduct={product.IDProduct}";
                    using (var command = new SqlCommand())
                    {
                        int idOrder = 0;
                        int IdPW = 0;
                        double Price = 0;

                        //1
                        command.Transaction = transaction;
                        command.Connection = connection;
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync())
                        {
                            if (!sqlDataReader.HasRows)
                            {
                                return "404 Brak produktu o danym ID";
                            }
                            while (sqlDataReader.Read()) {
                                Price = double.Parse(sqlDataReader["Price"].ToString());
                            }
                        }
                        //await connection.CloseAsync();


                        //await connection.OpenAsync();
                        sql = $"SELECT * FROM Warehouse WHERE IdWarehouse = {product.IDWarehouse}";
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync()) 
                        { 
                            if (!sqlDataReader.HasRows)
                            {
                                return "404 Brak hurtowni o danym ID";
                            }
                        }
                        //await connection.CloseAsync();


                        //2
                        //await connection.OpenAsync();
                        sql = $"SELECT * FROM [Order] WHERE IdProduct = {product.IDProduct} AND Amount = {product.Amount} AND CreatedAt<'{product.CreatedAt}'";
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync())
                        {
                            if (sqlDataReader.Read()) {
                                idOrder = sqlDataReader.GetInt32(0);
                                if (!sqlDataReader.HasRows)
                                {
                                    return "404 Brak zlecenia na taki produkt";
                                }
                                else if (!sqlDataReader.IsDBNull(4))
                                {
                                    return "400 Zlecenie zostało juz zrealizowane";
                                }
                                
                            }
 
                        }
                        //await connection.CloseAsync();

                        //await connection.OpenAsync();
                        sql = $"SELECT * FROM Product_Warehouse WHERE IdOrder = {idOrder}";
                        command.CommandText = sql;
                        using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync())
                        {
                             if (sqlDataReader.HasRows)
                             {
                                 return "404 Zlecenie zostało juz zrealizowane";
                             }   
                        }
                        //await connection.CloseAsync();

                        //4
                        sql = $"UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder={idOrder}";
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        //5
                        //await connection.OpenAsync();
                        double licz = (double)(Price*product.Amount);
                        NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
                        numberFormatInfo.NumberDecimalSeparator = ".";
                        sql = $"INSERT INTO Product_Warehouse VALUES ({product.IDWarehouse},{product.IDProduct},{idOrder},{product.Amount},{(licz).ToString(numberFormatInfo)},GETDATE())";
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        

                        //6
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
