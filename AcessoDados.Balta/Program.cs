using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcessoDados.Balta.Models;
using Dapper;
using Microsoft.Data.SqlClient;

//Microsoft.Data.SqlClient == pacote para se conectar ao Banco de dados
//Dapper é uma extensão a Microsoft.Data.SqlClient

namespace AcessoDados.Balta
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Server=localhost,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;Encrypt=false";

            using (var connection = new SqlConnection(connectionString))
            {
                UpdateCategory(connection);
                ListCategories(connection);
                //CreateCategories(connection);
            }

            Console.ReadKey();
        }

        static void ListCategories(SqlConnection connection)
        {
            //Consulta com Dapper
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");

            foreach (var item in categories)
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void CreateCategories(SqlConnection connection)
        {

            //Inserção com Dapper Parte 1 
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destinada a serviços do AWS";
            category.Featured = false;

            /*NÃO DEVEMOS CONCATENAR STRING ""$ + {0}"" EM INSERT UPDATE, SELECT...
              PARA EVITAR SQLINJECTION*/
            var insertSql = @"INSERT INTO [Category] 
                VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";
           
            //Inserção com Dapper Parte 2
            var rows = connection.Execute(insertSql, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });
            Console.WriteLine($"{rows} linhas inseridas");
        }

        //Atualização de Registro com Dapper
        static void UpdateCategory(SqlConnection connection)
        {
            var updateQuery = "UPDATE [Category] SET [Title]=@Title Where [Id]=@Id";
            var rows = connection.Execute(updateQuery, new
            {
                Id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
                Title = "Frontend 2021"
            });

            Console.WriteLine($"{rows} registros atualizados");
        }
    }
}
