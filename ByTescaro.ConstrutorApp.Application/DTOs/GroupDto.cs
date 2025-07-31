using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByTescaro.ConstrutorApp.Application.DTOs
{
    public class GroupDto
    {
        public string Name { get; set; }
        public string Phone { get; set; } // Este será o Group ID
        public bool IsGroup { get; set; }
    }
}
