using IBMSPaymentGateway.Models;
using IBMSPaymentGateway.PWGs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;

namespace IBMSPaymentGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        string baseUrl = "https://bms.ibos.io";
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        string gatewayId = "ibos6108bfc68a2c8";
        string gatewayPass = "ibos6108bfc68a2c8@ssl";
        PaymentGatewayTransaction trans = new PaymentGatewayTransaction()
        {
            IsLive = true,
            TransId = 1,
            TotalAmount = 500,
            CustomerName = "shahed",
            CustomerEmail = "shahedimamuddin@gmail.com",
            CustomerAdd1 = "Dhaka",
            CustomerAdd2 = "Uttora",
            CustomerCity = "Dhaka",
            CustomerPostCode = "1216",
            CustomerCountry = "Bangladesh",
            CustomerPhone = "01521200542",
            IsEnableEmi = true,
            ConvertionRate = 2
        };
        [HttpPost("SslCommerz")]
        public IActionResult SslCommerz()
        {
            try
            {
                var productName = "Akij Laptop";
                var price = 90000;
                // CREATING LIST OF POST DATA
                NameValueCollection PostData = new NameValueCollection();

                PostData.Add("total_amount", price.ToString());
                PostData.Add("tran_id", "TESTASPNET1234");
                PostData.Add("success_url", baseUrl + "/Cart/CheckoutConfirmation");
                PostData.Add("fail_url", baseUrl + "/Cart/CheckoutFail");
                PostData.Add("cancel_url", baseUrl + "/Cart/CheckoutCancel");

                PostData.Add("version", "3.00");
                PostData.Add("cus_name", "ABC XY");
                PostData.Add("cus_email", "abc.xyz@mail.co");
                PostData.Add("cus_add1", "Address Line On");
                PostData.Add("cus_add2", "Address Line Tw");
                PostData.Add("cus_city", "City Nam");
                PostData.Add("cus_state", "State Nam");
                PostData.Add("cus_postcode", "Post Cod");
                PostData.Add("cus_country", "Countr");
                PostData.Add("cus_phone", "0111111111");
                PostData.Add("cus_fax", "0171111111");
                PostData.Add("ship_name", "ABC XY");
                PostData.Add("ship_add1", "Address Line On");
                PostData.Add("ship_add2", "Address Line Tw");
                PostData.Add("ship_city", "City Nam");
                PostData.Add("ship_state", "State Nam");
                PostData.Add("ship_postcode", "Post Cod");
                PostData.Add("ship_country", "Countr");
                PostData.Add("value_a", "ref00");
                PostData.Add("value_b", "ref00");
                PostData.Add("value_c", "ref00");
                PostData.Add("value_d", "ref00");
                PostData.Add("shipping_method", "NO");
                PostData.Add("num_of_item", "1");
                PostData.Add("product_name", $"{productName}");
                PostData.Add("product_profile", "general");
                PostData.Add("product_category", "Demo");

                PgwSslCommerz sslcz = new PgwSslCommerz(gatewayId, gatewayPass, trans.IsLive);

                String response = sslcz.InitiateTransaction(PostData);

                _logger.LogInformation("START >> SSLCOMMERZ >> transId > " + trans.TransId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("transId > " + trans.TransId + " >  SSLCOMMERZ > " + ex.Message);

                // Response.Redirect("~/Home/Unauthorised?err=invalid gateway parameters");
                return BadRequest(ex);
                //throw new NullReferenceException();
            }
        }
    }
}
