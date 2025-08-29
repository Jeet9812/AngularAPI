using AngularAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace AngularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            string storedProcedure = "dbo.CheckLogin";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var parametersq = new DynamicParameters();
                parametersq.Add("@UserName", login.UserName);
                parametersq.Add("@UserPassword", login.UserPassword);
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<string>(
                    storedProcedure,
                    parametersq,
                    commandType: CommandType.StoredProcedure);

                    if (result == "Login Successful")
                    {
                        return Ok(new { Message = result });
                    }
                    else
                    {
                        return Unauthorized(new { Message = result });
                    }
                }
                catch (SqlException ex)
                {
                    return StatusCode(500, new { Message = "An error occurred while processing the request 121.", Error = ex.Message });
                }
            }
        }
    }
}