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
    public class RegionValidator : AbstractValidator<RegionData>
    {
        public RegionValidator()
        {
            RuleFor(RH => RH.Name).Must(BeUniqueRegion).WithMessage("Region Name Already Exist");
        }

        private bool BeUniqueRegion(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.Regions.FirstOrDefault(x => x.Name == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }



    public class ActivityPurposeValidator : AbstractValidator<PurposeOfActivityData>
    {
        public ActivityPurposeValidator()
        {
            RuleFor(RH => RH.Name).Must(BeUniqueRegion).WithMessage("ActivityPurpose Name Already Exist");
        }

        private bool BeUniqueRegion(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.ActivityPurposes.FirstOrDefault(x => x.PurposeName == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }





}