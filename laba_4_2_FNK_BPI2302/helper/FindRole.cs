using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using laba_4_2_FNK_BPI2302.model;

namespace laba_4_2_FNK_BPI2302.helper
{
    public class FindRole
    {
        int id;
        public FindRole(int id) { this.id = id; }
        public bool RolePredicate(Role role) => role.Id == id;
    }
}
