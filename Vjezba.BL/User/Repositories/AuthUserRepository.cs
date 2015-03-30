using Vjezba.BL.User.Models;
using Vjezba.DL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Vjezba.BL.User.Repositories
{
    public class AuthUserRepository : IDisposable
    {
        private BizHubEntities ctx = new BizHubEntities();

        // GET USER
        // =============================================================================
        // LOGIN
        public Profile GetUserByEmailPassword(string email, string password)
        {
            PasswordHash passwordHash = new PasswordHash();

            var result = ctx.Profile.FirstOrDefault(r => r.Email == email);
            if (result == null)
            {
                return null;
            }
            else if (passwordHash.ValidatePassword(password, result.Password))
            {
                return result;
            }
            else
            {
                return null;
            }        
        }
        // PROFILE
        public UserDataReturnModel GetUserDataById(int userId)
        {
            var userData = ctx.Profile.FirstOrDefault(r => r.ProfileID == userId && r.Active == true && r.IsValidate == true);

            var result = ProfileModelToReturnModel(userData);

            return result;
        }
        // PREVIEW PROFILE
        public PreviewUserReturnModel GetPreviewUserDataById(int userId)
        {
            var totalPosts = ctx.Post.Count(p => p.ProfileID == userId);

            var userData = ctx.Profile.FirstOrDefault(r => r.ProfileID == userId && r.Active == true && r.IsValidate == true);

            if (userData == null)
            {
                return null;
            }
            else {
                var result = UserDataToPreviewReturnModel(userData, totalPosts);
                return result;
            }
        }
        // GET USER BY EMAIL
        public Profile GetUserDataByEmail(string email)
        {
            var userData = ctx.Profile.FirstOrDefault(r => r.Email == email);

            return userData;
        }
        // GET VALIDATED USER BY EMAIL
        public Profile GetValidatedUserDataByEmail(string email)
        {
            var userData = ctx.Profile.FirstOrDefault(r => r.Email == email && r.Active == true && r.IsValidate == true);

            return userData;
        }
        // REGISTRATION
        // =============================================================================
        public bool RegisterUser(RegisterModel model)
        {
            if (CheckIfEmailExist(model.Email))
            {
                return false;
            }
            else
            {
                Profile newUser = new Profile();
                newUser = RegisterModelToProfile(model);
                ctx.Profile.Add(newUser);
                ctx.SaveChanges();

                string validationCode = AddValidationCode(newUser.ProfileID);
                SendOnRegistrationEmail(model.Email, model.Password);
                SendValidationCodeEmail(model.Email, validationCode);
                
                return true;
            }
        }
        // VALIDATE EMAIL
        // =============================================================================
        public bool ValidateUserEmail(int userId, string token)
        {
            var codeResult = ctx.ValidationCode.FirstOrDefault(c => c.ProfileID == userId);
            if (codeResult == null)
            {
                return false;
            }
            else
            {
                if (codeResult.Code == token)
                {
                    var user = ctx.Profile.FirstOrDefault(r => r.ProfileID == userId);
                    user.Active = true;
                    user.IsValidate = true;
                    ctx.Entry(user).State = EntityState.Modified;
                    ctx.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        // UPDATE USER DATA
        // =============================================================================
        public bool UpdateUserData(UpdateUserDataModel model, int userId)
        {
            var result = ctx.Profile.FirstOrDefault(r => r.ProfileID == userId);
            if (result == null)
            {
                return false;
            }
            else
            {
                result.Name = model.Name;
                result.Category = model.Category;
                result.Description = model.Description;
                result.Country = model.Country;
                result.City = model.City;
                result.Address = model.Address;
                result.Phone = model.Phone;
                ctx.Entry(result).State = EntityState.Modified;
                ctx.SaveChanges();
                return true;
            }
        }
        // PRIVATE METHODS
        // =============================================================================
        // ADD VALIDATION CODE
        private string AddValidationCode(int userId)
        {
            ValidationCodeHash validationCodeHash = new ValidationCodeHash();
            string hashCode = validationCodeHash.CreateHash(userId.ToString());
            DateTime myDateTime = DateTime.Now;
            ValidationCode newValidationCode = new ValidationCode()
            {
                Code = hashCode,
                Created = myDateTime,
                ProfileID = userId
            };
            ctx.ValidationCode.Add(newValidationCode);
            ctx.SaveChanges();
            return newValidationCode.Code;
        }
        // CHECK IF EMAIL EXIST
        private bool CheckIfEmailExist(string email)
        {
            bool exist = ctx.Profile.Any(user => user.Email.Equals(email));
            return exist;
        }
        // REGISTER MODEL TO PROFILE MODEL
        private Profile RegisterModelToProfile(RegisterModel model)
        {
            PasswordHash passwordHash = new PasswordHash();
            string passwordToHash = passwordHash.CreateHash(model.Password);
            ProfileLogoImageBasePath mainImageFolder = new ProfileLogoImageBasePath();
            string pathToDefaultImage = mainImageFolder.pathToMainImageFolder + "default/profile.png";
            DateTime myDateTime = DateTime.Now;
            Profile profile = new Profile()
            {
                Name = model.Name,
                Email = model.Email,
                Password = passwordToHash,
                Login = "local",
                Active = false,
                IsValidate = false,
                Created = myDateTime,
                ProfilePicture = pathToDefaultImage,
            };
            return profile;
        }         
        // PROFILE MODEL TO RETURN USER MODEL
        private UserDataReturnModel ProfileModelToReturnModel(Profile model)
        {
            UserDataReturnModel result = new UserDataReturnModel()
            {
                ProfileID = model.ProfileID,
                Name = model.Name,
                Description = model.Description,
                Country = model.Country,
                City = model.City,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                IsValidate = model.IsValidate,
                ProfilePicture = model.ProfilePicture,
                Category = model.Category
            };

            return result;
        }
        // USER DATA TO PREVIEW RETURN MODEL
        private PreviewUserReturnModel UserDataToPreviewReturnModel(Profile model, int totalPosts)
        {
            PreviewUserReturnModel result = new PreviewUserReturnModel()
            {
                ProfileID = model.ProfileID,
                Email = model.Email,
                Name = model.Name,
                Category = model.Category,
                Country = model.Country,
                City = model.City,
                Address = model.Address,
                Phone = model.Phone,
                Description = model.Description,
                ProfilePicture = model.ProfilePicture,
                TotalPosts = totalPosts
            };

            return result;
        } 
        // SEND ON REGISTRATION EMAIL
        private void SendOnRegistrationEmail(string userEmail, string password)
        {
            SendEmailConf emailConf = new SendEmailConf();
            MailMessage msg = new MailMessage();

            msg.Subject = "Uspješna registracija";
           
            string newLine = ("<br/>");
            string greeatingsMsg = "Čestitamo! Uspešno ste se registrovali na www.bizhub.ba" + newLine + newLine;
            string passwordMsg = "Vaša lozinka:" + password + newLine;
            string messageToSend = "Poštovanje," + newLine + newLine + greeatingsMsg + passwordMsg;
            msg.Body = messageToSend;

            msg.From = new MailAddress("bizhub.mail.service@gmail.com");
            msg.To.Add(userEmail);
            msg.IsBodyHtml = true;
            try
            {
                Object state = msg;
                emailConf.smtp.Send(msg);
            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
            }          
        }
        // SEND VALIDATION CODE EMAIL
        private void SendValidationCodeEmail(string userEmail, string token)
        {
            SendEmailConf emailConf = new SendEmailConf();
            MailMessage msg = new MailMessage();

            msg.Subject = "Validacija email-a";

            string validationLink = "http://localhost:49824/#/validacija-emaila/" + userEmail + "/" + token;

            string newLine = ("<br/>");
            string greeatingsMsg = "Da bi ste aktivirali vaš račun na www.bizhub.ba potrebno je kliknete na link koji je naveden dolje. Hvala" + newLine + newLine;
            string validationMsg = "Validacijski link:" + validationLink + newLine;
            string messageToSend = "Poštovanje," + newLine + newLine + greeatingsMsg + validationMsg;
            msg.Body = messageToSend;

            msg.From = new MailAddress("bizhub.mail.service@gmail.com");
            msg.To.Add(userEmail);
            msg.IsBodyHtml = true;
            try
            {
                Object state = msg;
                emailConf.smtp.Send(msg);
            }
            catch (Exception ex)
            {
                // implement log
                string error = ex.Message;
            }
        }

        public void Dispose()
        {
            ctx.Dispose();
        }
    }
}
