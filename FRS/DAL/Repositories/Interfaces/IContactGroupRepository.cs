using DAL.Core;
using DAL.Core.DTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IContactGroupRepository : IRepository<ContactGroup>
    {
        IEnumerable<ContactGroup> All();
        Task<List<ContactGroupMember>> GetContactGroupMembersAsync(int page, int pageSize, int? userId = null);
        Task<List<ContactGroup>> GetContactGroupsLoadRelatedAsync(int page, int pageSize, int? institutionId = null, int? userId = null, List<int> departmentIds = null, bool isSimple = false);
        Task<BaseOperationResponse> CreateAsync(ContactGroup contactGroup);
        Task<TestDeleteResult> TestCanDeleteAsync(int contactGroupId);
        Task<BaseOperationResponse> DeleteAsync(int contactGroupId);
        Task<ContactGroup> GetByIdAsync(int id, bool isSimple = true);
        Task<ContactGroup> GetByCode(int departmentId, string code);
        Task<BaseOperationResponse> UpdateAsync(ContactGroup contactGroup);
        Task<List<ContactGroupMember>> GetMembers(int id);
    }
}
