using Vjezba.BL.Messages.Repositories;
using Vjezba.BL.User.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Vjezba.API.Messages
{
    public class AuthMessageFileUploadController : ApiController
    {
        private AuthMessagesRepository _repo = new AuthMessagesRepository();
        private AuthUserRepository _repoAuth = new AuthUserRepository();

        [Route("api/auth-message-file-upload")]
        [Authorize]
        public IHttpActionResult Post()
        {
            try
            {
                var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;
                var claim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
                int userId = Int32.Parse(claim.Value);

                var httpRequest = HttpContext.Current.Request;
                // implement file filter before save new message

                if (httpRequest.Form.AllKeys.Length < 1)
                {
                    return BadRequest();
                }
                string msgId = httpRequest.Form["messageId"];
                int messageId;
                try
                {
                    messageId = Int32.Parse(msgId);
                }
                catch
                {
                    return BadRequest();
                }
                bool result = _repo.CheckIfMessageBelongToUser(messageId, userId);
                if (!result)
                {
                    return BadRequest();
                }
                else
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        foreach (string file in httpRequest.Files)
                        {
                            HttpPostedFile postedFile = httpRequest.Files[file];
                            SaveFilesToDirectory(postedFile, messageId);
                            _repo.SaveMessageUploadedFile(messageId, userId, postedFile.FileName, postedFile.ContentType);
                        }
                    }

                    return Ok();
                }

            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
                return Unauthorized();
            }                                    
        }

        private void SaveFilesToDirectory(HttpPostedFile postedFile, int messageId)
        {
            try
            {
                // path to root folder
            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Messages/");
            // check if current user directory exist and create if not "~/UploadedFiles/Messages/3/
            var currentUserDirectory = root + messageId.ToString();
            bool existDir = Directory.Exists(currentUserDirectory);
            if (!existDir)
            {
                Directory.CreateDirectory(currentUserDirectory);
            }
            // path to new logo image and saving image
            var userLogoImgPath = Path.Combine(currentUserDirectory, postedFile.FileName);
            postedFile.SaveAs(userLogoImgPath);
            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
            }  
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
                _repoAuth.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}