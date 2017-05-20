using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using ESQLWebSericeCSharpAPI.classes.exceptions;
using ESQLWebSericeCSharpAPI.classes.models.json;
using ESQLWebSericeCSharpAPI.classes.models.methods;
using System.Net;

namespace ESQLWebSericeCSharpAPI
{
    /// <summary>
    /// Class that represent ESQL Web Service.
    /// </summary>
    public class ESQLWebService
    {
        private string exceptionPrefix = "{0}"; // Error on ESQL Web Service with message:

        private string serverAddress;
        private string username;
        private string password;
        private string token;
        private HttpClient service;

        public ESQLWebService()
        {

        }

        /// <summary>
        /// Class constructor in which will check if ESQL Web Service exist on given address, 
        /// if doesn't exist exception ExceptionServiceNotAvaliable will be thrown.
        /// </summary>
        /// <param name="serverAddress">Address of the ESQL Web Service.</param>
        /// <param name="username">Non-empty value that represents username.</param>
        /// <param name="password">Non-empty value that represents password.</param>
        public ESQLWebService(string serverAddress, string username, string password)
        {
            this.ServerAddress = serverAddress;
            this.username = username;
            this.password = password;

            this.InitService();

            if (!this.Ping())
                throw new ExceptionServiceNotAvaliable("Service isn't available at the moement.");
        }

        /// <summary>
        /// Class constructor in which will check if ESQL Web Service exist on given address, 
        /// if doesn't exist exception ExceptionServiceNotAvaliable will be thrown.
        /// </summary>
        /// <param name="serverAddress">Address of the ESQL Web Service.</param>
        /// <param name="token">Token for connection.</param>
        public ESQLWebService(string serverAddress, string token)
        {
            this.ServerAddress = serverAddress;
            this.token = token;

            this.InitService();

            if (!this.Ping())
                throw new ExceptionServiceNotAvaliable("Service isn't available at the moement.");
        }

        /// <summary>
        /// Class constructor in which will check if ESQL Web Service exist on given address, 
        /// if doesn't exist exception ExceptionServiceNotAvaliable will be thrown.
        /// </summary>
        /// <param name="serverAddress">Address of the ESQL Web Service.</param>
        public ESQLWebService(string serverAddress)
        {
            this.ServerAddress = serverAddress;

            this.InitService();

            /*if (!this.Ping())
                throw new ExceptionServiceNotAvaliable("Service isn't available at the moement.");*/
        }

        /// <summary>
        /// Method for connecting on ESQL Web Service with propper username and password. 
        /// Authorization will be done via BasicAuth method. 
        /// If user is successfully authorized token will be received and stored to local proprety, 
        /// otherwise WrongCredentialsException will be thrown.
        /// </summary>
        /// <param name="username">Non-empty value that represents username.</param>
        /// <param name="password">Non-empty value that represents password.</param>
        public bool Connect(string username, string password)
        {
            this.username = username;
            this.password = password;

            string basicAuthData = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", this.username, this.password)));
            this.service.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthData);

            string methodName = WSMethods.Connect;
            HttpResponseMessage response = this.service.PostAsync(methodName, null).Result;

            if (response.IsSuccessStatusCode)
            {
                dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                if (content.token != null)
                {
                    this.token = (string)content.token;
                    this.service.DefaultRequestHeaders.Authorization = null; // remove from header (we will use token now on)
                    return true;
                }
                else
                {
                    this.HandleErrorResponse(response, WSMethods.Connect);
                }
            }
            else
            {
                this.HandleErrorResponse(response, WSMethods.Connect);
            }

            return false;
        }

        /// <summary>
        /// Disconect from server using existing token and clear all local data.
        /// </summary>
        /// <returns>True if successfully restarted, false otherwise.</returns>
        public bool Disconect()
        {
            string methodName = WSMethods.Disconnect;
            var postContent = new FormUrlEncodedContent(this.PrepareTokenForPost());

            HttpResponseMessage response = this.service.PostAsync(methodName, postContent).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        #region Query execution methods

        /// <summary>
        /// Method for execution of query with returning JSON as representation of output.
        /// </summary>
        /// <param name="query">Query for execution.</param>
        /// <returns>String in JSON format, or throws ExceptionDuringQueryExection on failure.</returns>
        public string ExecuteSelectQueryAndReturnJSON(string query)
        {
            string output = (string)this.ExecuteQuery(query);

            return output;
        }

        /// <summary>
        /// Method for performing update on DB (create, update and delete).
        /// </summary>
        /// <param name="query">Query for execution.</param>
        /// <returns>Return true if successfully executed, false otherwise. </returns>
        public bool ExecuteUpdateQuery(string query)
        {
            bool output = true;

            this.ExecuteQuery(query);

            return true;
        }

        /// <summary>
        /// Method for execution of query with returning DataTable object as representation of output. 
        /// </summary>
        /// <param name="query">Query for execution.</param>
        /// <returns>DataTable object, or throws ExceptionDuringQueryExection on failure.</returns>
        public System.Data.DataTable ExecuteSelectQueryAndReturnDatatable(string query)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable();

            string output = (string)this.ExecuteQuery(query);
            Select dt = JsonConvert.DeserializeObject<Select>(output);

            // insert columns and column types
            for (int i = 0; i < dt.Columns.Length; i++)
            {
                dataTable.Columns.Add(new System.Data.DataColumn(dt.Columns[i], Type.GetType(dt.ColumnTypes[i])));
            }
            // insert data
            for (int i = 0; i < dt.Data[0].Length; i++)
            {
                System.Data.DataRow tmpRow = dataTable.NewRow();
                for (int j = 0; j < dt.Data.Length; j++)
                {
                    string tmpCurrVal = dt.Data[j][i];

                    switch (dt.ColumnTypes[j])
                    {
                        case "System.Boolean":
                            tmpRow[j] = tmpCurrVal.Equals("1") ? true : false;
                            break;
                        case "System.Double":
                            tmpRow[j] = Double.Parse(tmpCurrVal);
                            break;
                        case "System.DateTime":
                            try
                            {
                                tmpRow[j] = DateTime.Parse(tmpCurrVal);
                            }
                            catch (Exception e)
                            {
                                tmpRow[j] = DateTime.MinValue;
                            }
                            break;
                        default:
                            // string
                            tmpRow[j] = tmpCurrVal;
                            break;
                    }
                }
                dataTable.Rows.InsertAt(tmpRow, i);
            }

            return dataTable;
        }

        /// <summary>
        /// Method which directly comunicates with WS.
        /// </summary>
        /// <param name="query">Query for execution.</param>
        /// <returns>String output from WS, throws ExceptionDuringQueryExection on failure.</returns>
        private dynamic ExecuteQuery(string query)
        {
            string output = "";
            string methodName = WSMethods.ExecuteQuery;

            List<KeyValuePair<string, string>> postData = this.PrepareTokenForPost();
            postData.Add(new KeyValuePair<string, string>("query", query));

            var postContent = new FormUrlEncodedContent(postData);

            HttpResponseMessage response = this.service.PostAsync(methodName, postContent).Result;

            if (response.IsSuccessStatusCode)
            {
                dynamic content = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                output = content.result;
            }
            else
            {
                this.HandleErrorResponse(response, methodName);
            }

            return output;
        }

        #endregion

        /// <summary>
        /// Method for reseting ESQL JSON Service, for reset provide admin username and admin password.
        /// </summary>
        /// <param name="adminUser">Username with admin credentials.</param>
        /// <param name="adminPassword">Password for admin user.</param>
        /// <returns>True if successfully reset, false when user&password are bad and exception of type ExceptionDuringReset will be thrown when service can't be reseted.</returns>
        public bool Reset(string adminUser, string adminPassword)
        {
            string basicAuthData = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", adminUser, adminPassword)));
            this.service.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthData);

            string methodName = WSMethods.Reset;
            HttpResponseMessage response = this.service.PostAsync(methodName, null).Result;

            this.service.DefaultRequestHeaders.Authorization = null;

            if (response.IsSuccessStatusCode)
                return true;
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return false;
                else
                    throw new ExceptionDuringReset(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        public bool RefreshToken()
        {
            string methodName = WSMethods.RefreshToken;
            var postContent = new FormUrlEncodedContent(this.PrepareTokenForPost());
            HttpResponseMessage response = this.service.PostAsync(methodName, postContent).Result;
            if (response.IsSuccessStatusCode)
            {
                // this.token = ...

                return true;
            }
            else
            {
                HandleErrorResponse(response, methodName);
                return false;
            }
        }

        /// <summary>
        /// Method that checks if ESQL Server is online.
        /// </summary>
        /// <returns>True if online, false otherwise.</returns>
        public bool Ping()
        {
            string methodName = WSMethods.Ping;

            HttpResponseMessage response = this.service.PostAsync(methodName, null).Result;

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Exception throw method, this method throw propper exception based on response code and method name.
        /// </summary>
        /// <param name="response">Response object with status code.</param>
        /// <param name="methodName">Name of current executed method.</param>
        private void HandleErrorResponse(HttpResponseMessage response, string methodName)
        {
            Exception exc;
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    if (methodName == WSMethods.RefreshToken)
                        exc = new ExceptionDuringTokenRenewal(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    else if (methodName == WSMethods.Connect)
                        exc = new ExceptionDuringConnect(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    else if (methodName == WSMethods.ExecuteQuery)
                        exc = new ExceptionDuringQueryExection(response.Content.ReadAsStringAsync().Result);
                    else if (methodName == WSMethods.Disconnect)
                        exc = new ExceptionDuringDisconnect(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    else
                        exc = new Exception(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    break;
                case HttpStatusCode.Unauthorized:
                    exc = new ExceptionWrongCredentials(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    break;
                case HttpStatusCode.RequestTimeout:
                    exc = new ExceptionServiceTimeout(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    exc = new ExceptionServiceNotAvaliable(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    break;
                default:
                    exc = new Exception(string.Format(this.exceptionPrefix, response.Content.ReadAsStringAsync().Result));
                    break;
            }

            throw exc;
        }

        /// <summary>
        /// Prepare auth token.
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> PrepareTokenForPost()
        {
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("token", this.token));

            return postData;
        }

        #region Getters and setters


        public string ServerAddress
        {
            get
            {
                return this.serverAddress;
            }
            set
            {
                if (value.IndexOf("http") == 0)
                {
                    this.serverAddress = value;
                }
                else
                {
                    this.serverAddress = "http://" + value;
                }
            }
        }
        public string Token { get { return this.token; } }

        #endregion

        private void InitService()
        {
            this.service = new HttpClient();
            this.service.BaseAddress = new Uri(this.serverAddress);
        }
    }
}