using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using online_store_api.Data;

namespace online_store_api.Controllers
{
    // Route Products
    [Route("[controller]")]
    // change the behaviour of the controller
    [ApiController]

    public class ProductsController : ControllerBase {
        private readonly ApplicationDbContext _dbContext;

        public ProductsController(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }
        // create a dubm list of product
        // private readonly static List<Product> _products = new List<Product> {
        //     new Product {
        //         Id = 1,
        //         Name = "Product 1",
        
        //     },
        //     new Product {
        //         Id = 2,
        //         Name = "Product 2",
           
        //     },
        //     new Product {
        //         Id = 3,
        //         Name = "Product 3",
      
        //     },
        //      new Product {
        //         Id = 4,
        //         Name = "Product 11",
        
        //     },
        //     new Product {
        //         Id = 5,
        //         Name = "Product 22",
           
        //     },
        //     new Product {
        //         Id = 6,
        //         Name = "Product 33",
      
        //     },
            
        // };

        [HttpGet]
        public async Task<Page<Product>>  GetAllProducts(string? name = "", int pageIndex = 0, int pageSize = 3) {

            // check if the query is empty or null 
            // if (string.IsNullOrEmpty(name)) {
            //     return _products;
            // }
            pageSize = Math.Min(pageSize, 25);
            // return products where the name contains the query
            var query =  _dbContext.Products.Where(p => (p.Name.Contains(name)));

            var totalCount = await query.CountAsync();

            var products  = await query
            .OrderBy(p => p.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
            ;

            return new Page<Product> {
                Items = products.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,

            };
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesErrorResponseType(typeof(NotFoundResult))]
        public async Task<ActionResult<Product>> GetProductById(int? id) {
            // return the product with the id
            var product = await  _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<Product>> CreateProduct(ProductRequest request) {
            var product = request.ToModel();
            // product.Id = _dbContext.Products.Max(p => p.Id) + 1;
            // add the product to the list
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // update the product
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Product>> UpdateProduct(int? id, ProductRequest request) {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
            product.Name = request.Name;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        // delete the product
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> DeleteProduct(int? id) {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) {
                return NotFound();
            }
            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }



    public class ProductRequest {
        public string Name { get; set; }

        public Product ToModel() {
            return new Product {
                Name = Name,
            };
        }
    }

    public class Page<T> {
        public List<T> Items {get; set;}

        public int PageIndex{get; set;}

        public int TotalCount{get; set;}
    }
    
}