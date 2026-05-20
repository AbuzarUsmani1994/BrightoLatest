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
    public class AreaValidator : AbstractValidator<AreaData>
    {
        public AreaValidator()
        {
        }
    }

    public class SubCatValidator : AbstractValidator<SubCategories>
    {
        public SubCatValidator()
        {
        }
    }

    public class ItemValidator : AbstractValidator<Items>
    {
        public ItemValidator()
        {
        }
    }
    public class SubCategoryAValidator : AbstractValidator<SubCategoryA>
    {
        public SubCategoryAValidator()
        {
        }
    }

    public class RoleAccessValidator : AbstractValidator<AreaData>
    {
        public RoleAccessValidator()
        {
        }
    }
    public class SOREGIONSValidator : AbstractValidator<AreaData>
    {
        public SOREGIONSValidator()
        {
        }
    }

}