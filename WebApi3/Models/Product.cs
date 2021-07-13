using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi3.Models
{
    public class Product
    {
        public int? IDProduct{ get; set; }
       
        public int? IDWarehouse { get; set; }
        
        public int? Amount { get; set; }
        
        public DateTime? CreatedAt { get; set; }
    }
}
