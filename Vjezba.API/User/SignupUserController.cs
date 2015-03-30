using Vjezba.BL.User.Models;
using Vjezba.BL.User.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Vjezba.API.User
{
    public class SignupUserController : ApiController
    {
        private AuthUserRepository _repo = new AuthUserRepository();

        [Route("api/signup-user")]
        public IHttpActionResult Post(RegisterModel credentials)
        {
            if (credentials == null)
            {
                string message = "Ups! Trebali bi popuniti sva polja. Pokušajte ponovo!";
                return BadRequest(message);
            }
            else if (!ModelState.IsValid)           
            {
                string message = "Ups! Polja nisu validna. Pokušajte ponovo!";
                return BadRequest(message);
            }
            else
            {
                if (_repo.RegisterUser(credentials))
                {
                    return Ok();
                }
                else
                {
                    string message = "Ups! Korisnik već postoji sa datim emailom.";
                    return BadRequest(message);
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