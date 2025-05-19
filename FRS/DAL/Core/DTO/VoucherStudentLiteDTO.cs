using Sieve.Attributes;

namespace DAL.Core.DTO
{
    public class VoucherStudentLiteDTO
    {
        public int StudentVoucherId { get; set; }
        [Sieve(CanFilter = false, CanSort = true)]
        public int VoucherId { get; set; }
        [Sieve(CanFilter = false, CanSort = true)]
        public int StudentId { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Code { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public string VoucherTypeName { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string StudentName { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Status { get; set; }
    }
}
