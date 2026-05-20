using FOS.Setup;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FOS.Web.UI.Controllers
{
    public class DealerAPIController : ApiController
    {

        [HttpGet]
        public List<DealerData> Get()
        {
            return ManageDealer.GetDealerList();
        }

    }
}