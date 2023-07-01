#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dave3.Model
{
    public class ControlObject
    {
       
        public string Name { get; set; }
        public string? ControlString { get; set; }
        public int? ControlInt { get; set; }
        public float? ControlFloat { get; set; }
    }
}
