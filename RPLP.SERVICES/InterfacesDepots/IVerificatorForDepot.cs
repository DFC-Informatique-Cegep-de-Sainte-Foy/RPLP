using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IVerificatorForDepot
    {
        public Type GetUserTypeByEmail(string p_email);
        public bool CheckUsernameTaken(string p_username);
        public bool CheckEmailTaken(string p_email);
    }
}
