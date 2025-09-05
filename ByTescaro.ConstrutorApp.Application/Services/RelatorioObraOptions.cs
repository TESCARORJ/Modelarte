using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.Services
{
    public sealed class RelatorioObraOptions
    {
        public ProgressoStrategy ProgressoStrategy { get; set; } = ProgressoStrategy.Media;
    }
}
