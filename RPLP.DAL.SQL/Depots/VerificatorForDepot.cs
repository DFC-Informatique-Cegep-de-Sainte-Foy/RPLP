using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class VerificatorForDepot
    {
        private readonly RPLPDbContext _context;

        public VerificatorForDepot(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public bool CheckUsernameTaken(string p_username)
        {
            return
                this._context.Administrators.FirstOrDefault(a => a.Username == p_username) != null ||
                this._context.Students.FirstOrDefault(s => s.Username == p_username) != null ||
                this._context.Teachers.FirstOrDefault(t => t.Username == p_username) != null;
        }

        public bool CheckEmailTaken(string p_email)
        {
            return
                this._context.Administrators.FirstOrDefault(a => a.Email == p_email) != null ||
                this._context.Students.FirstOrDefault(s => s.Email == p_email) != null ||
                this._context.Teachers.FirstOrDefault(t => t.Email == p_email) != null;
        }
    }
}
