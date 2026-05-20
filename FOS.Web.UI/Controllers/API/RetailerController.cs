using FOS.DataLayer;
using FOS.Setup;
using FOS.DataLayer;
using FOS.Web.UI.Common;
using FOS.Shared;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace FOS.Web.UI.Controllers.API
{
    public class RetailerController : ApiController
    {


        public class RetailerModel
        {
            public int RetailerID { get; set; }
            public string ShopName { get; set; }
            public string PersonName { get; set; }
            public string ContactNo { get; set; }
            public int DealerID { get; set; }
            public int SalesOfficerID { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public decimal Lattitude { get; set; }
            public decimal Longitude { get; set; }
            public string LocationName { get; set; }
            public string Address { get; set; }
            public string Token { get; set; }
            public string Phone2 { get; set; }
            public bool IsVerified { get; set; }
            public string BankName { get; set; }
            public string BankName2 { get; set; }
            public string AccountNo { get; set; }
            public string AccountNo2 { get; set; }
        }

        public class RetailerInfo
        {
            public string Name { get; set; }
            public string ShopName { get; set; }
            public int ID { get; set; }
            public string CityName { get; set; }
            public bool IsVerified { get; set; }
        }


        FOSDataModel db = new FOSDataModel();

        [System.Web.Http.HttpPost]
        public IHttpActionResult AddRetailerOLD(RetailerModel rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //ADD New Retailer 
                    retailerObj.ID = db.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    retailerObj.Name = rm.PersonName;
                    retailerObj.SaleOfficerID = rm.SalesOfficerID;
                   // retailerObj.DealerID = rm.DealerID;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.Location = rm.Lattitude + "," + rm.Longitude;
                    retailerObj.Latitude = rm.Lattitude;
                    retailerObj.Longitude = rm.Longitude;
                    retailerObj.LocationName = rm.LocationName;
                    retailerObj.LocationMargin = null;
                    retailerObj.Phone1 = rm.ContactNo;
                    retailerObj.Phone2 = null;
                    retailerObj.IsActive = true;
                    retailerObj.RetailerType = "Regular";
                    retailerObj.Status = false;
                    retailerObj.Address = rm.Address;
                    retailerObj.LastUpdate = DateTime.Now;
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    db.Retailers.Add(retailerObj);
                    //END

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Add New Retailer";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer added successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in add retailer API"
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Add Retailer API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Add Retailer API Failed"
                    }
                });
            }
        }







        [System.Web.Http.HttpPost]
        public IHttpActionResult AddRetailer(RetailerModel rm)
        {
            RetailersForApproval retailerObj = new RetailersForApproval();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //ADD New Retailer 
                    retailerObj.ID = 0; // IT WILL BE CALCULATED AFTER APPROVAL

                    retailerObj.Name = rm.PersonName;
                    retailerObj.SaleOfficerID = rm.SalesOfficerID;
                    retailerObj.DealerID = rm.DealerID;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.Type = "WALLCOAT";
                    retailerObj.Phone1 = rm.ContactNo;
                    retailerObj.Phone2 = rm.Phone2;
                    retailerObj.RetailerType = "Regular";
                    retailerObj.Address = rm.Address;

                    retailerObj.AccountNo = rm.AccountNo;
                    retailerObj.AccountNo2 = rm.AccountNo2;
                    retailerObj.BankName = rm.BankName;
                    retailerObj.BankName2 = rm.BankName2;

                    retailerObj.CreatedBy = rm.SalesOfficerID;
                    retailerObj.IsVerified = rm.IsVerified;

                    retailerObj.Source = (int)RetSourceEnum.Mobile;
                    retailerObj.Action = (int)RetActionEnum.Add;
                    retailerObj.CreatedOn = DateTime.Now;
                    retailerObj.IsDeleted = false;

                    db.RetailersForApprovals.Add(retailerObj);
                    //END

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Add New RetailerForApproval";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer added successfully for approval"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in add retailer for approval API"
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Add Retailer for approval API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Add Retailer for approval API Failed"
                    }
                });
            }
        }

        [Route("api/GetRetailers/{salesOfficerId}")]
        public IHttpActionResult GetRetailers(int salesOfficerId)
        {
            try
            {
                var so = db.SaleOfficers.Where(s => s.ID == salesOfficerId).FirstOrDefault();

                if (so != null)
                {
                    return Ok(new
                    {
                        Retailers = so.Retailers.Where(r => r.SaleOfficerID == so.ID 
                        && r.Location == null && r.IsActive && r.Status && !r.IsDeleted
                        && (r.Source != (int)RetSourceEnum.Mobile || r.Action == (int)RetActionEnum.UpdateApproved)).Select(d => new RetailerInfo
                        {
                            ID = d.ID,
                            ShopName = d.ShopName,
                            CityName = db.Cities.Where(c => c.ID == d.CityID).Select(c => c.Name).FirstOrDefault(),
                            IsVerified = d.IsVerified ?? false
                        })
                    });
                }

                else

                    return NotFound();
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get Retailer List API Failed");
                throw ex;
            }
        }

        [HttpGet]
        public IHttpActionResult GetSONewShopsAndRemap(int soId)
        {
            try
            {
                return Ok(new
                {
                    Retailers = db.Retailers.Where(r => r.SaleOfficerID == soId 
                    && r.IsActive && r.Status && !r.IsDeleted
                    && r.Location == null && (r.Action == (int)RetActionEnum.ResetLocApproved || r.Action == (int)RetActionEnum.AddApproved)).Select(d => new RetailerInfo
                    {
                        ID = d.ID,
                        ShopName = d.ShopName,
                        CityName = db.Cities.Where(c => c.ID == d.CityID).Select(c => c.Name).FirstOrDefault(),
                        IsVerified = d.IsVerified ?? false
                    })
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetSOResetShops API Failed");
                return Ok(new
                {
                    Retailers = new { }
                });
            }
        }
        [HttpGet]
        public IHttpActionResult GetSOResetShops(int soId)
        {
            try
            {
                return Ok(new
                {
                    Retailers = db.Retailers.Where(r => r.SaleOfficerID == soId 
                    && r.IsActive && r.Status && !r.IsDeleted
                    && r.Location != null).Select(d => new RetailerInfo
                    {
                        ID = d.ID,
                        ShopName = d.ShopName,
                        CityName = db.Cities.Where(c => c.ID == d.CityID).Select(c => c.Name).FirstOrDefault(),
                        IsVerified = d.IsVerified ?? false
                    })
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetSOResetShops API Failed");
                return Ok(new
                {
                    Retailers = new { }
                });
            }
        }

        public IHttpActionResult GetSORetailers(int soId)
        {
            try
            {
                return Ok(new
                {
                    Retailers = db.Retailers.Where(d => d.SaleOfficerID == soId
                    && d.IsActive && d.Status && !d.IsDeleted).Select(d => new
                    {
                        d.ID,
                        d.ShopName
                    }).OrderBy(d => d.ShopName)
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Get SO Retailer List API Failed");
                return Ok(new
                {
                    Retailers = new { }
                });
            }
        }

        [HttpGet]
        public IHttpActionResult GetRetailersRelevantInfo(int retailerId)
        {
            try
            {
                return Ok(new
                {
                    RelevantInfo = new
                    {
                        OutstandingAmount = 12000,
                        LastPaymentReceived = 1000,
                        LastDispatch = "20 Ton, 25-Feb-2018",
                        OverdueAmount = 25000
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetRetailersRelevantInfo API Failed");
                return Ok(new
                {
                    RelevantInfo = new { }
                });
            }
        }

        public class URModel
        {
            public int SalesOfficerID { get; set; }
            public int RetailerID { get; set; }
            public decimal Lattitude { get; set; }
            public decimal Longitude { get; set; }
            public string LocationName { get; set; }
            public string Token { get; set; }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateRetailerLocationOLD(URModel rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //Find Retailer
                    retailerObj = db.Retailers.Where(u => u.ID == rm.RetailerID).FirstOrDefault();

                    if (rm.Longitude > 0 && rm.Lattitude > 0)
                    {
                        retailerObj.SaleOfficerID = rm.SalesOfficerID;
                        retailerObj.Location = rm.Lattitude + "," + rm.Longitude;
                        retailerObj.Latitude = rm.Lattitude;
                        retailerObj.Longitude = rm.Longitude;
                        retailerObj.LocationName = rm.LocationName;

                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = rm.Token;
                        tokenDetail.Action = "Update Location Of Retailer";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END
                    }
                    else
                    {
                        retailerObj.Location = null;
                        retailerObj.Latitude = null;
                        retailerObj.Longitude = null;
                        retailerObj.LocationName = null;

                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = rm.Token;
                        tokenDetail.Action = "Reset Location Of Retailer";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END

                    }
                    //END
                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer updated successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in update retailer API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Retailer Location API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Retailer Location API Failed"
                    }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateRetailerLocation(URModel rm)
        {
            RetailersForApproval retailerObj = new RetailersForApproval();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    if (rm.Longitude > 0 && rm.Lattitude > 0)
                    {
                        Retailer retObj = db.Retailers.Where(u => u.ID == rm.RetailerID).FirstOrDefault();
                        //Find Retailer
                        retObj.ID = rm.RetailerID;

                        retObj.SaleOfficerID = rm.SalesOfficerID;
                        retObj.Location = rm.Lattitude + "," + rm.Longitude;
                        retObj.Latitude = rm.Lattitude;
                        retObj.Longitude = rm.Longitude;
                        retObj.LocationName = rm.LocationName;

                        retObj.Action = (int)RetActionEnum.UpdateLoc;
                        retObj.UpdateSource = (int)RetSourceEnum.Mobile;

                        retObj.LastUpdate = DateTime.Now;
                        retObj.CreatedOn = DateTime.Now;

                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = rm.Token;
                        tokenDetail.Action = "Update Location Of Retailer";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END
                    }
                    else
                    {
                        retailerObj.ID = rm.RetailerID;
                        retailerObj.IsDeleted = false;
                        //retailerObj.Location = null;
                        //retailerObj.Latitude = null;
                        //retailerObj.Longitude = null;
                        //retailerObj.LocationName = null;

                        retailerObj.Action = (int)RetActionEnum.ResetLoc;
                        retailerObj.Source = (int)RetSourceEnum.Mobile;

                        retailerObj.UpdatedBy = rm.SalesOfficerID;
                        retailerObj.UpdatedOn = DateTime.Now;
                        retailerObj.CreatedOn = DateTime.Now;

                        db.RetailersForApprovals.Add(retailerObj);


                        // Add Token Detail ...
                        TokenDetail tokenDetail = new TokenDetail();
                        tokenDetail.TokenName = rm.Token;
                        tokenDetail.Action = "Reset Location Of Retailer for Approval";
                        tokenDetail.ProcessedDateTime = DateTime.Now;
                        db.TokenDetails.Add(tokenDetail);
                        //END
                    }


                    //END
                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer location updated successfully for approval"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in update retailer location API for approval"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Retailer Location for approval API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Retailer Location for approval API Failed"
                    }
                });
            }
        }

        [HttpGet]
        public IHttpActionResult GetRetailerById(int retailerId)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                retailerObj = db.Retailers.Where(u => u.ID == retailerId).FirstOrDefault();

                if (retailerObj != null)
                {
                    return Ok(new
                    {
                        RetailerInfo = new RetailerModel
                        {
                            RetailerID = retailerObj.ID,
                            SalesOfficerID = retailerObj.SaleOfficerID,
                            Lattitude = retailerObj.Latitude.HasValue ? retailerObj.Latitude.Value : 0,
                            Longitude = retailerObj.Longitude.HasValue ? retailerObj.Longitude.Value : 0,
                            LocationName = retailerObj.LocationName,
                            Address = retailerObj.Address,
                            AreaID = retailerObj.AreaID.HasValue ? retailerObj.AreaID.Value : 0,
                            CityID = retailerObj.CityID.HasValue ? retailerObj.CityID.Value : 0,
                            ZoneID = retailerObj.ZoneID.HasValue ? retailerObj.ZoneID.Value : 0,
                            ContactNo = retailerObj.Phone1,
                            Phone2 = retailerObj.Phone2,
                           // DealerID = retailerObj.DealerID.HasValue ? retailerObj.DealerID.Value : 0,
                            PersonName = retailerObj.Name,
                            ShopName = retailerObj.ShopName,
                            BankName = retailerObj.BankName,    
                            AccountNo = retailerObj.AccountNo,
                            BankName2 = retailerObj.BankName2,
                            AccountNo2 = retailerObj.AccountNo2,
                            IsVerified = retailerObj.IsVerified ?? false
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        RetailerInfo = new { }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetRetailerById API Failed");
                return Ok(new
                {
                    RetailerInfo = new { }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateRetailerDetailsOLD(RetailerModel rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //Find Retailer
                    retailerObj = db.Retailers.Where(u => u.ID == rm.RetailerID).FirstOrDefault();

                    retailerObj.Name = rm.PersonName;
                    //retailerObj.DealerID = rm.DealerID;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.Phone1 = rm.ContactNo;
                    retailerObj.Address = rm.Address;
                    retailerObj.LastUpdate = DateTime.Now;
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Update Of Retailer details";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer details updated successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in Update Retailer Details API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "UpdateRetailerDetails API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Retailer Details API Failed"
                    }
                });
            }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateRetailerDetails(RetailerModel rm)
        {
            RetailersForApproval retailerObj = new RetailersForApproval();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //Find Retailer
                    retailerObj.ID = rm.RetailerID; //

                    retailerObj.Name = rm.PersonName;
                    retailerObj.DealerID = rm.DealerID;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.SaleOfficerID = rm.SalesOfficerID;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.Phone1 = rm.ContactNo;
                    retailerObj.Phone2 = rm.Phone2;
                    retailerObj.Address = rm.Address;

                    retailerObj.AccountNo = rm.AccountNo;
                    retailerObj.AccountNo2 = rm.AccountNo2;
                    retailerObj.BankName = rm.BankName;
                    retailerObj.BankName2 = rm.BankName2;

                    retailerObj.IsVerified = rm.IsVerified;

                    retailerObj.Source = (int)RetSourceEnum.Mobile;
                    retailerObj.Action = (int)RetActionEnum.Update;

                    retailerObj.UpdatedBy = rm.SalesOfficerID;
                    retailerObj.UpdatedOn = DateTime.Now;

                    //retailerObj.CreatedOn = DateTime.Now;
                    retailerObj.IsDeleted = false;

                    db.RetailersForApprovals.Add(retailerObj);
                    //END

                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Update shop RetailerForApproval";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END
                    

                    // update retailers ISVERIFIED FLAG
                    var retailer = db.Retailers.Where(p => p.ID == rm.RetailerID).FirstOrDefault();
                    if (retailer != null)
                    {
                        retailer.IsVerified = true;
                    }

                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer updated successfully for approval"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in update retailer for approval API"
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Retailer for approval API Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Retailer for approval API Failed"
                    }
                });
            }
        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateRetailerVerified(URModel rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //Find Retailer
                    retailerObj = db.Retailers.Where(u => u.ID == rm.RetailerID).FirstOrDefault();
                    retailerObj.IsVerified = true;
                    retailerObj.LastUpdate = DateTime.Now;


                    // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Update IsVerified Of Retailer";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    //END
                    db.SaveChanges();

                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "200",
                            message = "Retailer verified updated successfully"
                        }
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = new CheckInLatLongResp
                        {
                            code = "400",
                            message = "Authentication failed in retailer verified API"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Update Retailer Verified API UpdateRetailerVerified Failed");
                return Ok(new
                {
                    status = new CheckInLatLongResp
                    {
                        code = "400",
                        message = "Update Retailer Verified API UpdateRetailerVerified Failed"
                    }
                });
            }
        }

        public IHttpActionResult GetBanks()
        {
            try
            {
                return Ok(new
                {
                    Banks = ManageRetailer.GetBanks()
                });

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "GetBanks API Failed");
                return Ok(new
                {
                    Banks = new { }
                });
            }
        }
    }
}