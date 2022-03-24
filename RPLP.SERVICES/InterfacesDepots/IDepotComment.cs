using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotComment
    {
        public List<Comment> GetComments();
        public Comment GetCommentById(int p_id);
        public void UpsertComment(Comment p_comment);
        public void DeleteComment(int p_commentId);
    }
}
