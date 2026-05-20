using FluentValidation;
using FOS.DataLayer;
using FOS.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Setup.Validation
{
    public class DealerValidator : AbstractValidator<DealerData>
    {
        public DealerValidator()
        {
            //RuleFor(RH => RH.Name).NotEmpty().WithMessage("* Required");
            RuleFor(RH => RH.Phone1).Must(BeUniquePhone1).WithMessage("Phone1 Already Exist");
            RuleFor(RH => RH.Phone2).Must(BeUniquePhone2).WithMessage("Phone2 Already Exist");
        }

        private bool BeUniquePhone1(string strPhone1)
        {
            Boolean boolFlag = true;
            if (!String.IsNullOrEmpty(strPhone1))
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    int DealerID = dbContext.Dealers.Where(x => x.Phone1 == strPhone1).Select(x =>x.ID).FirstOrDefault();
                    if (dbContext.Dealers.FirstOrDefault(x => x.Phone1 == strPhone1 && x.ID != DealerID) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }

        private bool BeUniquePhone2(string strPhone2)
        {
            Boolean boolFlag = true;
            if (!String.IsNullOrEmpty(strPhone2))
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    int DealerID = dbContext.Dealers.Where(x => x.Phone2 == strPhone2).Select(x => x.ID).FirstOrDefault();
                    if (dbContext.Dealers.FirstOrDefault(x => x.Phone2 == strPhone2 && x.ID != DealerID) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }
}