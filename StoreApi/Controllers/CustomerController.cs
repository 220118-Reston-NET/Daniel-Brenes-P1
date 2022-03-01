using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StoreModel;

namespace StoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IStoreBL _storeBL;
        private IMemoryCache _memoryCache; //Only used for GetAllCustomerAsync, memory cache not necessary for P1
        public CustomerController(IStoreBL p_storeBL, IMemoryCache p_memoryCache)
        {
            _storeBL = p_storeBL;
            _memoryCache = p_memoryCache;
        }
        // GET: api/Customer
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomerAsync()
        {
            try
            {
            List<Customer> listOfCustomer = new List<Customer>();
            //TryGetValue(checks if the cache still exists and if it does "out listOfCustomer" puts that data inside our variable)
            if (!_memoryCache.TryGetValue("customerList", out listOfCustomer))
            {
                listOfCustomer = await _storeBL.GetAllCustomerAsync();
                _memoryCache.Set("customerList", listOfCustomer, new TimeSpan(0, 0, 30));
            }
                Log.Information("Get All Customers" + listOfCustomer);
                return Ok(await _storeBL.GetAllCustomerAsync());
            }
            catch (SqlException)
            {
                //In this case, if it was unable to connect to the database, it will give a 404 status code
                return NotFound();
            }
        }
        // GET: api/Customer/5

        // public IActionResult GetCustomerByName([FromQuery] string customerName)
        // [HttpGet("GetCustomerByName/{customerName}")]
        [HttpGet("SearchCustomerByName")]
        // public IActionResult GetCustomerByName(string customerName)
        public IActionResult GetCustomerByName([FromQuery] string customerName)
        {
            try
            {
                return Ok(_storeBL.SearchCustomerByName(customerName));
            }
            catch (SqlException)
            {
                return NotFound();
            }
        }
        [HttpGet("SearchCustomerByEmail")]
        // public IActionResult GetCustomerByName(string customerName)
        public IActionResult GetCustomerByEmail([FromQuery] string customerEmail)
        {
            try
            {
                return Ok(_storeBL.SearchCustomerByEmail(customerEmail));
            }
            catch (SqlException)
            {
                return NotFound();
            }
        }
        [HttpGet("DisplayCustomerOrders")]
        public IActionResult GetCustomerOrderById([FromQueryAttribute] int customerId)
        {
            try
            {
            List<Order> listOfOrder = new List<Order>();
            // listOfOrder =  _storeBL.GetOrderByCustomerId(customerId);
            Log.Information("Displaying Customer Orders " + customerId);
                return Ok(_storeBL.GetOrderByCustomerId(customerId));
            }
            catch (SqlException)
            {
                 Log.Information("Displaying failed " + customerId);
                return NotFound();
            }
        }

        // POST: api/Customer
        [HttpPost("AddACustomer")]
        public IActionResult Post([FromBody] Customer p_customer)
        {
            try
            {
                return Created("Successfully added", _storeBL.AddCustomer(p_customer));
            }
            catch(System.Exception)
            {
                return Conflict();
            }
            // return Ok(_storeBL.AddCustomer(p_customer));
        }

        // PUT: api/Customer/5
        [HttpPut("Update/{id}")]
        public IActionResult Put(int id, [FromBody] Customer p_customer)
        {
            p_customer.CustomerId = id;
            try
            {
                return Ok(_storeBL.UpdateCustomer(p_customer));
            }
            catch (System.Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
        // DELETE: api/Customer/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
