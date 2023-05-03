using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class AllocationsViewModel
    {
        public List<AllocationViewModel> Pairs { get; }
        public string _classroom { get; }

        public int Status
        {
            get
            {
                int status = int.MaxValue;
                foreach (AllocationViewModel allocation in this.Pairs)
                {
                    if (allocation.Status < status)
                    {
                        status = allocation.Status;
                    }
                }

                return status;
            }
        }

        public AllocationsViewModel()
        {
            ;
        }
        public AllocationsViewModel(List<AllocationViewModel> p_pairs, string p_classroom)
        {
            this.Pairs = p_pairs;
            this._classroom = p_classroom; 
        }
    }
}
