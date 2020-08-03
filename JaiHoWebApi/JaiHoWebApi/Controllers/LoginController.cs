using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JaiHoWebApi.ModelsGenerated;
using JaiHoWebApi.Utility;
using JaiHoWebApi.ViewModels;

namespace JaiHoWebApi.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        PostgreContext userDetailsDBContext = new PostgreContext();
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [Route("Login")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult UserLogin([FromBody] User user)
        {
            _logger.LogInformation("LoginController.UserLogin method called!");
            try
            {
                var usrDetails = userDetailsDBContext.Userdetails.Where(x => x.Email.Equals(user.Email) &&
                     x.Password.Equals(user.Password) &&
                     x.Isactive == true).FirstOrDefault();

                if (usrDetails == null)
                {
                    user.Id = 0;
                    user.Email = "";
                    user.Name = "";
                    user.Phone = "";
                    user.City = "";
                    user.State = "";
                    user.Country = "";
                    user.Password = "";
                    user.Confirmpassword = "";
                    user.Isactive = null;
                    user.CreatedOn = null;
                    user.LastLogin = null;
                    user.errorHandler = new ErrorHandler() { success = "N", message = "UserName or Password is Invalid." };
                    return BadRequest(user);
                }
                else
                {
                    user.Id = usrDetails.Id;
                    user.Email = usrDetails.Email;
                    user.Name = usrDetails.Name;
                    user.Phone = usrDetails.Phone;
                    user.City = usrDetails.City;
                    user.State = usrDetails.State;
                    user.Country = usrDetails.Country;
                    user.Password = "";
                    user.Confirmpassword = "";
                    user.Isactive = usrDetails.Isactive;
                    user.CreatedOn = usrDetails.CreatedOn;
                    user.LastLogin = usrDetails.LastLogin;
                    user.errorHandler = new ErrorHandler() { success = "Y", message = "Login Success!" };
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                throw ex;
            }

        }

        [Route("Register")]
        [HttpPost]
        public HttpResponseMessage RegisterUser([FromBody] User user)
        {
            _logger.LogInformation("LoginController.RegisterUser method called!");
            try
            {
                Userdetails regUser = new Userdetails();

                if (regUser.Id == 0)
                {
                    regUser.Email = user.Email;
                    regUser.Name = user.Name;
                    regUser.Phone = user.Phone;
                    regUser.City = user.City;
                    regUser.State = user.State;
                    regUser.Country = user.Country;
                    regUser.Password = user.Password;
                    regUser.Confirmpassword = user.Confirmpassword;
                    regUser.Isactive = true;
                    regUser.CreatedOn = DateTime.Now;
                    regUser.LastLogin = DateTime.Now;
                    userDetailsDBContext.Userdetails.Add(regUser);
                    userDetailsDBContext.SaveChanges();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                throw ex;
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("ChangePassword")]
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePassword changePwd)
        {
            _logger.LogInformation("LoginController.ChangePassword method called!");
            User user = new User();
            try
            {
                var usrDetails = userDetailsDBContext.Userdetails.Where(x => x.Email.Equals(changePwd.email) &&
                      x.Password.Equals(changePwd.oldpassword) &&
                      x.Isactive == true).FirstOrDefault();

                if (usrDetails == null)
                {
                    user.Id = 0;
                    user.Email = "";
                    user.Name = "";
                    user.Phone = "";
                    user.City = "";
                    user.State = "";
                    user.Country = "";
                    user.Password = "";
                    user.Confirmpassword = "";
                    user.Isactive = null;
                    user.CreatedOn = null;
                    user.LastLogin = null;
                    user.errorHandler = new ErrorHandler() { success = "N", message = "Change Password Failed." };
                    return BadRequest(user);
                }
                else
                {
                    usrDetails.Password = changePwd.newpassword;
                    usrDetails.Confirmpassword = changePwd.newpassword;
                    userDetailsDBContext.SaveChanges();

                    user.Id = usrDetails.Id;
                    user.Email = usrDetails.Email;
                    user.Name = usrDetails.Name;
                    user.Phone = usrDetails.Phone;
                    user.City = usrDetails.City;
                    user.State = usrDetails.State;
                    user.Country = usrDetails.Country;
                    user.Password = "";
                    user.Confirmpassword = "";
                    user.Isactive = usrDetails.Isactive;
                    user.CreatedOn = usrDetails.CreatedOn;
                    user.LastLogin = usrDetails.LastLogin;
                    user.errorHandler = new ErrorHandler() { success = "Y", message = "Password was changed successfully." };
                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException.ToString());
                throw ex;
            }
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public HttpResponseMessage ForgotPassword(string email)
        {
            _logger.LogInformation("LoginController.ForgotPassword method called!");
            var userDetails = userDetailsDBContext.Userdetails.Where(x => x.Email.Equals(email) &&
                      x.Isactive == true).FirstOrDefault();

            if (userDetails == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
            {
                try
                {
                    //Send Email with Reset-Password Link
                    SendMail.Email(email);
                }
                catch (Exception ex)
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
        }

    }
}
