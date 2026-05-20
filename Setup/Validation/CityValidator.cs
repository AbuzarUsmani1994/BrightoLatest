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
    public class SchemeValidator : AbstractValidator<SchemeData>
    {
        public SchemeValidator()
        {
            RuleFor(RH => RH.SchemeInfo).Must(BeUniqueCity).WithMessage("Scheme Already Exist");
        }

        private bool BeUniqueCity(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.Cities.FirstOrDefault(x => x.Name == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }
    public class CityValidator : AbstractValidator<CityData>
    {
        public CityValidator()
        {
            RuleFor(RH => RH.Name).Must(BeUniqueCity).WithMessage("City Name Already Exist");
        }

        private bool BeUniqueCity(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.Cities.FirstOrDefault(x => x.Name == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }

    public class MainCategoryValidator : AbstractValidator<MainCategories>
    {
        public MainCategoryValidator()
        {
            RuleFor(RH => RH.MainCategoryName).Must(BeUniqueCity).WithMessage("MainCategory Already Exist");
        }

        private bool BeUniqueCity(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.MainCategories.FirstOrDefault(x => x.MainCategDesc == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }
    public class zoneValidator : AbstractValidator<ZonesData>
    {
        public zoneValidator()
        {
            RuleFor(RH => RH.Name).Must(BeUniqueCity).WithMessage("Zone Already Exist");
        }

        private bool BeUniqueCity(string strName)
        {
            Boolean boolFlag = true;
            if (strName != String.Empty)
            {
                using (FOSDataModel dbContext = new FOSDataModel())
                {
                    if (dbContext.Zones.FirstOrDefault(x => x.Name == strName) == null) return true;
                    boolFlag = false;
                }
            }
            return boolFlag;
        }
    }


}