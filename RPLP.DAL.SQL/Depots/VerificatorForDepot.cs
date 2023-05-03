using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class VerificatorForDepot : IVerificatorForDepot
    {
        private readonly RPLPDbContext _context;

        public VerificatorForDepot(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "VerificatorForDepot - VerificatorForDepot(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public Type GetUserTypeByEmail(string p_email)
        {
            if (string.IsNullOrWhiteSpace(p_email))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "VerificatorForDepot - GetGetUserTypeByEmailTeacherByEmail - p_email passé en paramètre est vide", 0));
            }

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
            if (string.IsNullOrWhiteSpace(p_username))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "VerificatorForDepot - CheckUsernameTaken - p_username passé en paramètre est vide", 0));
            }

            return
                this._context.Administrators.Any(a => a.Username == p_username) ||
                this._context.Students.Any(s => s.Username == p_username) ||
                this._context.Teachers.Any(t => t.Username == p_username);
        }

        public bool CheckEmailTaken(string p_email)
        {
            if (string.IsNullOrWhiteSpace(p_email))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "VerificatorForDepot - CheckUsernameTaken - p_email passé en paramètre est vide", 0));
            }

            return
                this._context.Administrators.Any(a => a.Email == p_email) ||
                this._context.Students.Any(s => s.Email == p_email)  ||
                this._context.Teachers.Any(t => t.Email == p_email);
        }
    }
}
