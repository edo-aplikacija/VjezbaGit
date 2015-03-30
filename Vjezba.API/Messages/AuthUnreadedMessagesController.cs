using Vjezba.BL.Messages.Repositories;
using Vjezba.BL.User.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Vjezba.API.Messages
{
    public class AuthUnreadedMessagesController : ApiController
    {
        private AuthMessagesRepository _repo = new AuthMessagesRepository();

        [Route("api/auth-unreaded-messages")]
        [Authorize]
        public IHttpActionResult Get()
        {
            try
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                int userId = Int32.Parse(claim.Value);
                var unreadedMessages = _repo.GetUserUnreadedMessages(userId);
                return Ok(unreadedMessages);

            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
                return Unauthorized();
            }
        }
    }
}