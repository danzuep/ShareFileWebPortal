using System;
using System.Collections.Generic;

namespace Data.ShareFile.Models
{
    public partial class Application
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
    }
}
