using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrikeDefender.Domain.BaseModels
{
    public class BaseModel
    {

        public DateTime Createdon { get; private set; } = DateTime.UtcNow;
        public bool Deleted { get; private set; }
        public string? UpdatedByid { get; private set; }
        public DateTime? Updatedon { get; private set; }


        protected void Touch(string updatedById)
        {
            UpdatedByid = updatedById;
            Updatedon = DateTime.UtcNow;
        }

        internal void SoftDelete(string updatedById)
        {
            if (Deleted) return;

            Deleted = true;
            Touch(updatedById);
        }

        protected void Restore(string updatedById)
        {
            if (!Deleted) return;

            Deleted = false;
            Touch(updatedById);
        }
    }
}

