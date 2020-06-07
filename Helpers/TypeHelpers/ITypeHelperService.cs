using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ord.WebApi.Helpers.TypeHelpers
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
