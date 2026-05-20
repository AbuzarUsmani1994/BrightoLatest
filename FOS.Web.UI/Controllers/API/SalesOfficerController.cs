using FOS.DataLayer;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class SalesOfficerController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        [Route("api/salesofficer/{name}/{password}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Login(string name, string password)
        {
            string Status = "";
            string Message = "";
            try
            {
                string Token;
                var SO = db.SaleOfficers.Where(s => s.UserName.ToLower().Equals(name.ToLower()) && s.Password.ToLower().Equals(password.ToLower())).FirstOrDefault();


                if (SO != null)
                {
                    Status = "1";
                    Message = "UserName And Password Matched";

                    //Token Generate And Save In Token Table...
                    Token = FOS.Web.UI.Common.Token.TokenAttribute.GenerateToken(name, password);
                    Token tokenObj = new Token();

                    tokenObj.SalesOfficerID = SO.ID;
                    tokenObj.TokenName = Token;
                    tokenObj.TokenAssignDate = DateTime.Now;
                    db.Tokens.Add(tokenObj);
                    db.SaveChanges();
                    //END

                    return Ok(new
                    {
                        SalesOfficer = new List<object>() { new {
                        SO.ID,
                        SO.Name,
                        SO.UserName,
                        SO.Password,
                        SO.RegionalHeadID,
                        SO.Phone1,
                        SO.Phone2,
                        SO.IsActive,
                        Token,
                        Status = Status,
                        Message = Message,
                        RoleID = 66,
                        Role = "SaleOfficer"
                     }}
                    });
                }
                else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
                {
                    var user = db.Users.Where(p => p.UserName.ToLower().Equals(name.ToLower()) && p.Password.ToLower().Equals(password.ToLower())).FirstOrDefault();
                    if (user != null)
                    {


                        var RHeads = db.Users.Where(s => s.ID == user.ID).FirstOrDefault().RegionalHeads.Select(r => r.ID).ToList();
                        if (user != null && RHeads.Count > 0)
                        {
                            Status = "1";
                            Message = "UserName And Password Matched";

                            //Token Generate And Save In Token Table...
                            Token = FOS.Web.UI.Common.Token.TokenAttribute.GenerateToken(name, password);
                            Token tokenObj = new Token();

                            tokenObj.SalesOfficerID = user.ID;
                            tokenObj.TokenName = Token;
                            tokenObj.TokenAssignDate = DateTime.Now;
                            db.Tokens.Add(tokenObj);
                            db.SaveChanges();
                            //END

                            return Ok(new
                            {
                                SalesOfficer = new List<object>() {
                                new {
                                    user.ID,
                                        Name = user.UserName,
                                    user.UserName,
                                    user.Password,
                                    RegionalHeadID = RHeads[0],
                                    Phone1 = user.PhoneNo,
                                    Phone2 = user.PhoneNo,
                                    user.IsActive,
                                    Token,
                                    Status = Status,
                                    Message = Message,
                                    RoleID = 65,
                                    Role = "RegionalHead"
                                }
                            }
                            });
                        }
                        else
                        {
                            Status = "0";
                            Message = "User Name/Password Not Matched";
                            return Ok(new
                            {
                                SalesOfficer = new List<object>() {
                                    new {
                                        Status,
                                        Message
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        Status = "0";
                        Message = "User Name/Password Not Matched";

                        return Ok(new
                        {
                            SalesOfficer = new List<object>() {
                                    new {
                                        Status,
                                        Message
                                    }
                                }
                        });
                    }
                }
                else
                {
                    Status = "0";
                    Message = "User Name/Password Not Matched";
                    return Ok(new
                    {
                        SalesOfficer = new List<object>() { new {
                            Status,
                            Message
                     }}
                    });
                }
            }
            catch (Exception exp)
            {
                Status = "0";
                Message = "Server Is Not Responding";

                Log.Instance.Error(exp, "Sales Officer Login API Failed");

                return Ok(new
                {
                    SalesOfficer = new List<object>() { new {
                            Status,
                            Message
                     }}
                });
            }
        }


        [Route("api/salesofficer/{name}/{password}")]
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [System.Web.Http.HttpPost]
        public bool LogOut(int id)
        {
            try
            {

                db.Tokens.RemoveRange(db.Tokens.Where(t => t.SalesOfficerID == id));
                db.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Token Destroy API Failed");
                return false;
            }

        }



        [HttpGet]
        public IHttpActionResult GetProductInfo(int soId)
        {
            try
            {
                return Ok(new
                {
                    ProductInfo = new
                    {
                        Url = "http://painters.kmlg.com:8082/images/prodinfo/mapleleafwallcoatmob.pdf"
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetProductInfo API Failed");
                return Ok(new
                {
                    ProductInfo = new { }
                });
            }
        }

    }

    public class Data
    {

        public int ID { get; private set; }
        public string Name { get; private set; }
        public string UserName { get; private set; }
        public int CityID { get; private set; }
        public int RegionalHeadID { get; private set; }
        public string Phone1 { get; private set; }
        public string Phone2 { get; private set; }
        public bool IsActive { get; private set; }

        public Data(int id, string name, string UserName, int? cityId, int? regionalHeadId, string phone1, string phone2, bool IsActive)
        {
            this.ID = id;
            this.Name = name;
            this.UserName = UserName;
            this.CityID = (int)cityId;
            this.RegionalHeadID = (int)regionalHeadId;
            this.Phone1 = phone1;
            this.Phone2 = phone2;
            this.IsActive = IsActive;
        }
    }
}