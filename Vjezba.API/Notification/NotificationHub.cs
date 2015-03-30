using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Claims;
using Vjezba.BL.Messages.Repositories;
using Vjezba.BL.User.Repositories;

namespace Vjezba.API.Notification
{
    [HubName("notification")]
    [Authorize]
    public class NotificationHub : Hub
    {
        private AuthMessageNotificationRepository _repoMsg = new AuthMessageNotificationRepository();

        private AuthUserRepository _repoUser = new AuthUserRepository();

        public void MessageInitNotification()
        {
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (identity != null)
            {
                var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);               
                if (claim != null)
                {
                    var userId = Int32.Parse(claim.Value);

                    var result = _repoMsg.GetNewUnreadedMessages(userId);

                    this.Clients.Caller.onMessageInit(result);             
                }               
            }
        }

        public void NewMessageNotificationByUserEmail(string userEmail)
        {
            var userResult = _repoUser.GetValidatedUserDataByEmail(userEmail);
            if (userResult != null && userEmail != null)
            {
                var result = _repoMsg.GetLatestNewMessage(userResult.ProfileID);

                this.Clients.User(userResult.Email).onNewMessage(result);
            }          
        }

        public void NewMessageNotificationByUserId(int userId)
        {
            var userResult = _repoUser.GetUserDataById(userId);
            if (userResult != null)
            {
                var result = _repoMsg.GetLatestNewMessage(userResult.ProfileID);

                this.Clients.User(userResult.Email).onNewMessage(result);
            }                                   
        }
    }
}