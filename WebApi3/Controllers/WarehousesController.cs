using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi3.Models;
using WebApi3.Services;

namespace WebApi3.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    public class WarehousesController : ControllerBase
    {
        private IProductService _iProductService;

        public WarehousesController(IProductService productService)
        {
            _iProductService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProduct([FromBody] Product newProduct)
        {
            var resultOperation = await _iProductService.AddNewProduct(newProduct);
            var error = int.Parse(resultOperation.Substring(0,3));
            if (error == 400)
            {
                return BadRequest(resultOperation.Substring(4));
            }
            else if (error == 404)
            {
                return NotFound(resultOperation.Substring(4));
            }
            else if(error == 200)
            {
                return Ok(resultOperation.Substring(4));
            }
            else{
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
