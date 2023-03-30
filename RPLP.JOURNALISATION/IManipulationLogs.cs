using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.JOURNALISATION
{
    public interface IManipulationLogs
    {
        void Journal(Log log);
        void ClearLogs();
    }
}
