using IBMSPaymentGateway.Models;
using Microsoft.AspNetCore.Http;
using Nancy.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IBMSPaymentGateway.PWGs
{
    public class PgwSslCommerz
    {
        protected List<String> key_list;
        protected String generated_hash;
        protected string error;

        protected string Store_ID;
        protected string Store_Pass;
        protected bool Store_Test_Mode;

        protected string SSLCz_URL = "https://securepay.sslcommerz.com/";
        protected string Submit_URL = "gwprocess/v4/api.php";
        protected string Validation_URL = "validator/api/validationserverAPI.php";
        protected string Checking_URL = "validator/api/merchantTransIDvalidationAPI.php";

        public PgwSslCommerz(string Store_ID, string Store_Pass, bool Store_Test_Mode = false)
        {
            if (Store_ID != "" && Store_Pass != "")
            {
                this.Store_ID = Store_ID;
                this.Store_Pass = Store_Pass;
                this.SetSSLCzTestMode(Store_Test_Mode);
            }
            else
            {
                throw new Exception("Please provide Store ID and Password to initialize SSLCommerz");
            }
        }

        public string InitiateTransaction(NameValueCollection PostData, bool GetGateWayList = false)
        {
            PostData.Add("store_id", this.Store_ID);
            PostData.Add("store_passwd", this.Store_Pass);
            string response = this.SendPost(PostData);
            try
            {
                SSLCommerzInitResponse resp = new JavaScriptSerializer().Deserialize<SSLCommerzInitResponse>(response);
                if (resp.status == "SUCCESS")
                {
                    if (GetGateWayList)
                    {
                        // We will work on it!
                    }
                    else
                    {
                        return resp.GatewayPageURL.ToString();
                    }
                }
                else
                {
                    throw new Exception("Unable to get data from SSLCommerz. Please contact your manager!");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }

            return response;
        }

        public bool OrderValidate(string MerchantTrxID, string MerchantTrxAmount, string MerchantTrxCurrency, HttpRequest req)
        {
            bool hash_verified = this.ipn_hash_verify(req);
            if (hash_verified)
            {

                string json = string.Empty;

                string EncodedValID = System.Web.HttpUtility.UrlEncode(req.Form["val_id"]);
                string EncodedStoreID = System.Web.HttpUtility.UrlEncode(this.Store_ID);
                string EncodedStorePassword = System.Web.HttpUtility.UrlEncode(this.Store_Pass);

                string validate_url = this.SSLCz_URL + this.Validation_URL + "?val_id=" + EncodedValID + "&store_id=" + EncodedStoreID + "&store_passwd=" + EncodedStorePassword + "&v=1&format=json";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(validate_url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(resStream))
                {
                    json = reader.ReadToEnd();
                }
                if (json != "")
                {
                    SSLCommerzValidatorResponse resp = new JavaScriptSerializer().Deserialize<SSLCommerzValidatorResponse>(json);

                    if (resp.status == "VALID" || resp.status == "VALIDATED")
                    {
                        if (MerchantTrxCurrency == "BDT")
                        {
                            if (MerchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(MerchantTrxAmount) - Convert.ToDecimal(resp.amount)) < 1) && MerchantTrxCurrency == "BDT")
                            {
                                return true;
                            }
                            else
                            {
                                this.error = "Amount not matching";
                                return false;
                            }
                        }
                        else
                        {
                            if (MerchantTrxID == resp.tran_id && (Math.Abs(Convert.ToDecimal(MerchantTrxAmount) - Convert.ToDecimal(resp.currency_amount)) < 1) && MerchantTrxCurrency == resp.currency_type)
                            {
                                return true;
                            }
                            else
                            {
                                this.error = "Currency Amount not matching";
                                return false;
                            }

                        }
                    }
                    else
                    {
                        this.error = "This transaction is either expired or fails";
                        return false;
                    }
                }
                else
                {
                    this.error = "Unable to get Transaction JSON status";
                    return false;

                }
            }
            else
            {
                this.error = "Unable to verify hash";
                return false;
            }
        }

        protected void SetSSLCzTestMode(bool mode)
        {
            this.Store_Test_Mode = mode;
            if (mode)
            {
                this.Store_ID = "testbox";
                this.Store_Pass = "qwerty";
                this.SSLCz_URL = "https://sandbox.sslcommerz.com/";
            }
        }

        protected string SendPost(NameValueCollection PostData)
        {
            //Console.WriteLine(this.SSLCz_URL + this.Submit_URL);
            string response = PgwSslCommerz.Post(this.SSLCz_URL + this.Submit_URL, PostData);
            return response;
        }

        public static string Post(string uri, NameValueCollection PostData)
        {
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, PostData);
            }
            return System.Text.Encoding.UTF8.GetString(response);
        }

        /// <summary>
        /// SSLCommerz IPN Hash Verify method
        /// </summary>
        /// <param name="req"></param>
        /// <param name="pass"></param>
        /// <returns>Boolean - True or False</returns>
        public Boolean ipn_hash_verify(HttpRequest req)
        {

            // Check For verify_sign and verify_key parameters
            if (req.Form["verify_sign"] != "" && req.Form["verify_key"] != "")
            {
                // Get the verify key
                String verify_key = req.Form["verify_key"];
                if (verify_key != "")
                {

                    // Split key string by comma to make a list array
                    key_list = verify_key.Split(',').ToList<String>();

                    // Initiate a key value pair list array
                    List<KeyValuePair<String, String>> data_array = new List<KeyValuePair<string, string>>();

                    // Store key and value of post in a list
                    foreach (String k in key_list)
                    {
                        data_array.Add(new KeyValuePair<string, string>(k, req.Form[k]));
                    }

                    // Store Hashed Password in list
                    String hashed_pass = this.MD5(this.Store_Pass);
                    data_array.Add(new KeyValuePair<string, string>("store_passwd", hashed_pass));

                    // Sort Array
                    data_array.Sort(
                        delegate (KeyValuePair<string, string> pair1,
                        KeyValuePair<string, string> pair2)
                        {
                            return pair1.Key.CompareTo(pair2.Key);
                        }
                    );


                    // Concat and make String from array
                    String hash_string = "";
                    foreach (var kv in data_array)
                    {
                        hash_string += kv.Key + "=" + kv.Value + "&";
                    }
                    // Trim & from end of this string
                    hash_string = hash_string.TrimEnd('&');

                    // Make hash by hash_string and store
                    generated_hash = this.MD5(hash_string);

                    // Check if generated hash and verify_sign match or not
                    if (generated_hash == req.Form["verify_sign"])
                    {
                        return true; // Matched
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Make PHP like MD5 Hashing
        /// </summary>
        /// <param name="s"></param>
        /// <returns>md5 Hashed String</returns>
        public String MD5(String s)
        {
            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(s);
            byte[] hashedBytes = System.Security.Cryptography.MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hashedString;
        }
    }
}

