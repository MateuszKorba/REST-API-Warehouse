using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi3.Models;
using WebApi3.Services;

namespace WebApi3.Controllers
{
    [ApiController]
    [Route("api/warehouses2")]
    public class Warehouses2Controller : ControllerBase
    {
        private IProductService2 _iProductService2;

        public Warehouses2Controller(IProductService2 productService2)
        {
            _iProductService2 = productService2;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProductWithProcedure([FromBody] Product newProduct)
        {
            var resultOperation = await _iProductService2.AddNewProductFromProc(newProduct);
            var error = int.Parse(resultOperation.Substring(0, 3));
            if (error == 200) {
                return Ok(resultOperation.Substring(4));
            }
            else
            {
                return BadRequest("Procedura nie udana");
            }
        }
    }
}
