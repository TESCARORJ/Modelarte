using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Domain.Interfaces
{
    public interface IHolidaysService
    {
        Task<bool> IsHolidayAsync(DateTime date);
    }
}
