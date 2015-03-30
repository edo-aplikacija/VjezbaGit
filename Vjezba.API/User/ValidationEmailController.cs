using Vjezba.BL.User.Models;
using Vjezba.BL.User.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Vjezba.API.User
{
    public class ValidationEmailController : ApiController
    {
        private AuthUserRepository _repo = new AuthUserRepository();

        [Route("api/validation-email")]
        public IHttpActionResult Post(string email, string token)
        {

            if (email == null || email == "" || token == null || token == "")
            {
                string message = "Ups! Navedeni podaci su nisu ispravni.";
                return BadRequest(message);
            }
            else
            {
                var user = _repo.GetUserDataByEmail(email);
                if (user == null)
                {
                    string message = "Ups! Navedeni podaci su nisu ispravni.";
                    return BadRequest(message);
                }
                else
                {
                    if (user.Active == true && user.IsValidate == true)
                    {
                        return Ok();
                    }
                    else
                    {
                        var result = _repo.ValidateUserEmail(user.ProfileID, token);
                        if (result == true)
                        {
                            return Ok();
                        }
                        else
                        {
                            string message = "Ups! Navedeni podaci su nisu ispravni.";
                            return BadRequest(message);
                        }
                    }
                }
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