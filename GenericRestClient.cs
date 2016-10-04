using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
namespace DataHelper
{
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Rest Client to consume RESTFul service.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public class GenericRestClient<T> : IGenericRestClient<T>
    {
        private string entity;
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["SmartAPI"];

        /// <summary>
        /// Constructor Generic Rest client.
        /// </summary>
        /// <param name="entity">Entity type.</param>
        public GenericRestClient(string entity)
        {
            this.entity = entity;
            _client = new RestClient(_url);
        }

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>List of entities.</returns>
        public IEnumerable<T> GetAll()
        {
            try
            {
                var request = new RestRequest("/" + this.entity, Method.GET) { RequestFormat = DataFormat.Json };
                var result = _client.Execute(request);
                dynamic response = JObject.Parse(result.Content);
                if (response != null && response.value != null)
                {
                    var json = new JavaScriptSerializer();
                    return response.value.ToObject<List<T>>();
                }

                return new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Get entity by Id.
        /// </summary>
        /// <param name="id">Id of entity.</param>
        /// <returns>Entity instance.</returns>
        public T GetById(int id)
        {
            try
            {
                var request = new RestRequest("/" + this.entity + "(" + id + ")", Method.GET) { RequestFormat = DataFormat.Json };

                var response = _client.Execute(request);
                dynamic result = JObject.Parse(response.Content);
                if (result != null)
                {
                    var json = new JavaScriptSerializer();
                    return result.ToObject<T>();
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get entities by odata filter. This'll work with all Odata filters.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IEnumerable<T> GetByFilter(string filter)
        {
            try
            {
                var request = new RestRequest("/" + this.entity + "?$filter=" + filter, Method.GET) { RequestFormat = DataFormat.Json };

                var response = _client.Execute(request);
                dynamic result = JObject.Parse(response.Content);
                if (result != null)
                {
                    var json = new JavaScriptSerializer();
                    return result.value.ToObject<List<T>>();
                }

                return new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Create new entity.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        public void Add(T entity)
        {
            try
            {
                var request = new RestRequest("/" + this.entity, Method.POST) { RequestFormat = DataFormat.Json };
                var json = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                });
                // var json = new JavaScriptSerializer().Serialize(entity);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                _client.Execute(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update existing entity.
        /// </summary>
        /// <param name="entity">Entity instance.</param>
        /// <param name="id">Entity id.</param>
        public void Update(T entity, int id)
        {
            try
            {
                var request = new RestRequest("/" + this.entity + "(" + id + ")", Method.PUT) { RequestFormat = DataFormat.Json };
                var settings = new JsonSerializerSettings
                {
                    Converters = new[] {new IsoDateTimeConverter {
                        DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"
                    }}
                };
                var json = JsonConvert.SerializeObject(entity, settings);
                request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
                _client.Execute(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get entity by expanding(odata: $expand) inner property.
        /// </summary>
        /// <param name="id">Entity Id.</param>
        /// <param name="expand">Child entity to expand.</param>
        /// <returns>Entity instance.</returns>
        public T GetByIdExpand(int id, string expand)
        {
            try
            {
                var request = new RestRequest("/" + this.entity + "(" + id + ")" + expand, Method.GET) { RequestFormat = DataFormat.Json };

                var response = _client.Execute(request);
                dynamic result = JObject.Parse(response.Content);
                if (result != null)
                {
                    var json = new JavaScriptSerializer();
                    return result.ToObject<T>();
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Detele entity.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            try
            {
                var request = new RestRequest("/" + this.entity + "(" + id + ")", Method.DELETE) { RequestFormat = DataFormat.Json };

                var response = _client.Execute(request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Custom execute method.
        /// </summary>
        /// <param name="url">Web api url.</param>
        /// <returns>List of entities.</returns>
        public IEnumerable<T> Execute(string url)
        {
            try
            {
                var request = new RestRequest("/" + url, Method.GET) { RequestFormat = DataFormat.Json };

                var response = _client.Execute(request) as RestResponse;
                dynamic result = JObject.Parse(response.Content);
                if (result != null)
                {
                    var json = new JavaScriptSerializer();

                    return result.value.ToObject<List<T>>();
                }

                return new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }
    }
}
