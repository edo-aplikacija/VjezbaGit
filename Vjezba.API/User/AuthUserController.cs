using Vjezba.BL.User.Models;
using Vjezba.BL.User.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace Vjezba.API.User
{
    public class AuthUserController : ApiController
    {
        private AuthUserRepository _repo = new AuthUserRepository();

        // on login retrive User Data
        [Route("api/auth-user")]
        [Authorize]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                int userId = Int32.Parse(claim.Value);

                var result = _repo.GetUserDataById(userId);
                if (result == null)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
                return Unauthorized();
            }        
        }

        // edit user Data
        [Route("api/auth-user")]
        [Authorize]
        public IHttpActionResult Put(UpdateUserDataModel model)
        {
            try
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                int userId = Int32.Parse(claim.Value);

                if (model == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }
                else
                {
                    var result = _repo.UpdateUserData(model, userId);
                    if (result)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }               
            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
                return Unauthorized();
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