using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2.Models;
using DataStore.EF;
using Microsoft.EntityFrameworkCore;
using TicketSystemAPI.Interfaces;

namespace TicketSystemAPI.Repository
{
    public class CommentRepository : IComment
    {
        private readonly AppDbContext _dbContext;

        public CommentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void DeleteComment(int ticketId, int CommentId)
        {
            var Ticketcomments = _dbContext.Comments.Where(c => c.TicketId == ticketId).ToList();

            if (Ticketcomments != null)
            {
                foreach (var comment in Ticketcomments)
                {
                    if (comment.CommentId == CommentId)
                    {
                        _dbContext.Comments.Remove(comment);
                    }
                }
            }

            _dbContext.SaveChanges();

        }

        public async Task<List<Comment>> GetComments(int id)
        {
            var comments = await _dbContext.Comments.Where(c => c.TicketId == id).ToListAsync();
            return comments;
        }

        public async Task<Comment> PostComment(int ticketId, Comment comment)
        {
            comment.TicketId = ticketId;
            comment.SubmitTime = DateTime.Now;
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }
    }
}
