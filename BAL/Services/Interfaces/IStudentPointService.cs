using BAL.DTO;
using BAL.DTO.MealOrder;
using DAL.Core;
using DAL.Filters;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BAL.Services.Interfaces
{
    public interface IStudentPointService
    {
        Task<BaseOperationResponse> StudentPointTransactionAsync(StudentPointTransactionDTO dto);
        Task<List<StudentPointTransactionDTO>> GetPointTransactionByIdAsync(int id);
    }
}
