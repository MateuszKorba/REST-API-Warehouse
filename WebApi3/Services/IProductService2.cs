using System.Threading.Tasks;
using WebApi3.Models;

namespace WebApi3.Services
{
    public interface IProductService2
    {
        public Task<string> AddNewProductFromProc(Product product);
    }
}
