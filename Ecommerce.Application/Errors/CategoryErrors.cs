using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Errors;
public static class CategoryErrors
{
    public static readonly Error NotFound =
    new("Category.CategoryNotFound", "can't find Category with given id", StatusCodes.Status404NotFound);

    public static readonly Error duplicated =
    new("Category.Categoryduplicated", "there are already a Category with same name ", StatusCodes.Status409Conflict);
}
