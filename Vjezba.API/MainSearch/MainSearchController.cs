using Vjezba.BL.MainSearch.Models;
using Vjezba.BL.MainSearch.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Vjezba.API.MainSearch
{
    public class MainSearchController : ApiController
    {
        private MainSearchRepository _repo = new MainSearchRepository();
        // string searchName, [FromBody]string[] country, string searchCity, int page

        [Route("api/main-search")]
        [HttpGet]
        public IHttpActionResult Get([FromUri]MainSearchModel mainSearchModel)
        {
            if (mainSearchModel.searchName == null)
            {
                mainSearchModel.searchName = "";
            }
            if (mainSearchModel.country == null)
            {
                string[] newCountry = new string[1] { "" };
                mainSearchModel.country = newCountry;
            }
            for (int i = 0; i < mainSearchModel.country.Length; i++)
            {
                if (mainSearchModel.country[0] == null)
                {
                    mainSearchModel.country[0] = "";
                }
            }
            if (mainSearchModel.searchCity == null)
            {
                mainSearchModel.searchCity = "";
            }
            if (mainSearchModel.category == null)
            {
                string[] newCategory = new string[1] { "" };
                mainSearchModel.category = newCategory;
            }
            for (int i = 0; i < mainSearchModel.category.Length; i++)
            {
                if (mainSearchModel.category[0] == null)
                {
                    mainSearchModel.category[0] = "";
                }
            }


            var result = _repo.AdvancedSearchUsers(mainSearchModel.searchName, mainSearchModel.country, mainSearchModel.searchCity, mainSearchModel.category, mainSearchModel.page);
            if (result == null)
            {
                return Ok();
            }
            else
            {
                return Ok(result);
            }
            

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}