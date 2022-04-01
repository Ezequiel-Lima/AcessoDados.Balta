﻿using System;
using System.Collections.Generic;
using System.Data;
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
                //UpdateCategory(connection);
                //DeleteCategories(connection);
                //CreateManyCategories(connection);
                //GetCategories(connection);
                ExecuteReadProcedure(connection);
                //GetStudent(connection);
                //ExecuteProcedure(connection);
                //CreateCategories(connection);
            }

            Console.ReadKey();
        }

        //Consulta com Dapper
        static void GetCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");

            //Ordernando por Titulos em ordem alfabética
            foreach (var item in categories.OrderBy(x => x.Title))
            {
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        static void GetStudent(SqlConnection connection)
        {
            var estudantes = connection.Query("SELECT [Id], [Name] FROM [Student]");

            foreach (var item in estudantes)
            {
                Console.WriteLine($"{item.Id} - {item.Name}");
            }
        }

        //Inserção com Dapper
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

        //Deleção de Registro com Dapper
        static void DeleteCategories(SqlConnection connection)
        {
            var deleteQuery = "DELETE [Category] WHERE [Id]=@Id";

            var rows = connection.Execute(deleteQuery, new
            {
                Id = new Guid("f1c0ea41-6d2d-4d69-a794-fa6a836da884")
            });

            Console.WriteLine($"{rows} registros deletados");
        }

        //Inserção com Multipla Dapper
        static void CreateManyCategories(SqlConnection connection)
        {

            //Inserção com Multipla Dapper Parte 1 
            var category = new Category();
            category.Id = Guid.NewGuid();
            category.Title = "Amazon AWS";
            category.Url = "amazon";
            category.Summary = "AWS Cloud";
            category.Order = 8;
            category.Description = "Categoria destinada a serviços do AWS";
            category.Featured = false;

            var category2 = new Category();
            category2.Id = Guid.NewGuid();
            category2.Title = "Categoria Nova";
            category2.Url = "categoria-nova";
            category2.Summary = "CTG Cloud";
            category2.Order = 9;
            category2.Description = "Categoria destinada a serviços do CTG";
            category2.Featured = true;

            /*NÃO DEVEMOS CONCATENAR STRING ""$ + {0}"" EM INSERT UPDATE, SELECT...
              PARA EVITAR SQLINJECTION*/
            var insertSql = @"INSERT INTO [Category] 
                VALUES(@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

            //Inserção com Dapper Parte 2
            var rows = connection.Execute(insertSql, new[]
            {
                new
                {
                    category.Id,
                    category.Title,
                    category.Url,
                    category.Summary,
                    category.Order,
                    category.Description,
                    category.Featured
                },
                new
                {
                    category2.Id,
                    category2.Title,
                    category2.Url,
                    category2.Summary,
                    category2.Order,
                    category2.Description,
                    category2.Featured
                }
            });
            Console.WriteLine($"{rows} linhas inseridas");
        }

        //Procedure de Deleção Dapper
        static void ExecuteProcedure(SqlConnection connection)
        {
            
            //[sqDeleteStudent] é nome da procedure no caso 
            var procedure = "[spDeleteStudent]";

            var parametros = new { StudentId = "a77bb753-3d93-4919-839e-0fd97b8919e0" };
            //Comando é igual a executar uma Query a unica diferença é que passamos um commandType
            //precisa importar System.Data
            var rows = connection.Execute(procedure, parametros, commandType: CommandType.StoredProcedure);
            
            Console.WriteLine($"{rows}Linhas Afetadas");
        }

        //Procedure de Leitura Dapper
        static void ExecuteReadProcedure(SqlConnection connection)
        {
            var procedure = "[spGetCoursesByCategory]";
            var param = new { CategoryId = "09ce0b7b-cfca-497b-92c0-3290ad9d5142" };
            var result = connection.Query(
                procedure,
                param,
                commandType: CommandType.StoredProcedure);

            foreach (var item in result)
            {
                //Detalhe o nome da propriedade no caso o id tem que ser exatamente como esta no Banco
                Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }
    }
}
