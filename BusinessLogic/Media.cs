using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAPPTest
{
    public class Media
    {
        public Guid MediaId { get; set; }
        public Guid MediaFolderId { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int Sequence { get; set; }
        public bool IsScreenSaver { get; set; }
        public bool IsDeleted { get; set; }
    }
}
