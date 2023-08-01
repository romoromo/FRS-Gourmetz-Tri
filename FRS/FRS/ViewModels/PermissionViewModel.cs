using System;
using System.Collections.Generic;
using System.Linq;

namespace FRS.ViewModels
{
    public class PermissionViewModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
    }

    public class PermissionTreeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }

        public PermissionTreeViewModel Parent { get; set; }
        public List<PermissionTreeViewModel> Children { get; set; }
    }
}
