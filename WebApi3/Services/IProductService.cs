using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi3.Models;

namespace WebApi3.Services
{
    public interface IProductService
    {
        public Task<string> AddNewProduct(Product product);
    }
}
