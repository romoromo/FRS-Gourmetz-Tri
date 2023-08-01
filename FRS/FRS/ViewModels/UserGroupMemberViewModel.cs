using FRS.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRS.ViewModels
{
    public class UserGroupMemberViewModel
    {
        public int Id { get; set; }
        public int UserGroupId { get; set; }
        public int UserId { get; set; }
        public string UserGroupName { get; set; }
    }
}
