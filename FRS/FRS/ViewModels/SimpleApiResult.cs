using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class SimpleApiResult
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Display { get; set; }
    }

    public class SimpleApiTreeResult
    {
        public SimpleApiTreeResult()
        {

        }

        public SimpleApiTreeResult(int id, string label, string display)
        {
            this.Id = id;
            this.Label = label;
            this.Display = display;
        }

        public int Id { get; set; }
        public string Label { get; set; }
        public string Display { get; set; }

        public int? ParentId { get; set; }
        public SimpleApiTreeResult Parent { get; set; }
        public List<SimpleApiTreeResult> Children { get; set; }
    }
}
