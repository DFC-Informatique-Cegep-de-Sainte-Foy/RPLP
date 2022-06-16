using RPLP.ENTITES;
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

        public VerificatorForDepot()
        {
            this._context = new RPLPDbContext();
        }


        public VerificatorForDepot(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public Type GetUserTypeByEmail(string p_email)
        {
            if (this._context.Administrators.Any(a => a.Email == p_email))
            {
                return typeof(Administrator); 
            }

            else if (this._context.Students.Any(s => s.Email == p_email))
            {
                return typeof(Student);
            }
                
            else if (this._context.Teachers.Any(t => t.Email == p_email))
            {
                return typeof(Teacher);
            }

            return null;
                
        }

        public bool CheckUsernameTaken(string p_username)
        {
            Console.Out.WriteLine($"CheckUsernameTaken(string p_username) - p_username : {p_username}");

            return
                this._context.Administrators.Any(a => a.Username == p_username) ||
                this._context.Students.Any(s => s.Username == p_username) ||
                this._context.Teachers.Any(t => t.Username == p_username);
        }

        public bool CheckEmailTaken(string p_email)
        {
            return
                this._context.Administrators.Any(a => a.Email == p_email) ||
                this._context.Students.Any(s => s.Email == p_email)  ||
                this._context.Teachers.Any(t => t.Email == p_email);
        }
    }
}
