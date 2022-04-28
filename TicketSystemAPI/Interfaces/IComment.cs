using Core2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystemAPI.Interfaces
{
    public interface IComment
    {
        Task<Comment> PostComment(int ticketId, Comment comment);

        Task<List<Comment>> GetComments(int id);

        void DeleteComment(int ticketId, int CommentId);

    }
}
