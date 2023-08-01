
using System.Collections.Generic;

namespace FRS.ViewModels
{
    public class SignageCompilationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int width { get; set; }
        public int height { get; set; }

        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }

        public int CompilationId { get; set; }


        public int IsActive { get; set; }

        public int? interval { get; set; }

        public List<SignageComponentViewModel> components { get; set; }
    }
}
