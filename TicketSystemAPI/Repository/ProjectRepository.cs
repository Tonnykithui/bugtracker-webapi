using Core2.Interfaces;
using Core2.Models;
using DataStore.EF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core2.Repository
{
    public class ProjectRepository : IProject
    {
        private readonly AppDbContext _dbContext;

        public ProjectRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AssignUsersProject(int appId, ProjUser projUser)
        {
            //check provided IDs contain corresponding matches in DB
            var projectExists = _dbContext.Projects.Find(appId);
            

            if(projectExists != null)
            {
                foreach(var projUserId in projUser.ApplicationUserIds)
                {
                    var userExists = _dbContext.Users.Find(projUserId);

                    if(userExists != null)
                    {
                        var record = new ProjUser
                        {
                            ApplicationUserId = projUserId,
                            ProjectId = appId
                        };

                        _dbContext.ProjUsers.Add(record);
                    }
                    
                }
                _dbContext.SaveChanges();
            }
            
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            if (project == null)
                throw new NullReferenceException("Provide project details");

            project.CreationDate = DateTime.Now;

            var result = await _dbContext.Projects.AddAsync(project);
            await _dbContext.SaveChangesAsync();

            return project;
        }

        public void DeleteProjectAsync(int id)
        {
            //check project with id exists
            var projectExists = _dbContext.Projects.Find(id);
            if (projectExists == null)
                throw new NullReferenceException("No project with given id exists");

            //if exists remove project
            try
            {
                _dbContext.Projects.Remove(projectExists);
            }
            catch
            {
                throw;
            }

            _dbContext.SaveChanges();
        }

        public void DeleteUserFromProject(int projId, string userId)
        {
            //check project exists
            //var userProj = new ProjUser
            //{
            //    ApplicationUserId = projUser.ApplicationUserId,
            //    ProjectId = projId
            //};

            var projectUserExists = _dbContext.ProjUsers.Find(projId, userId); 

            if(projectUserExists != null)
            {
                _dbContext.ProjUsers.Remove(projectUserExists);
                _dbContext.SaveChanges();
            }

        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            
            return await _dbContext.Projects.ToListAsync();
           
        }

        public async Task<Object> GetSingleProjectAsync(int? id)
        {
            var projectExists = await _dbContext.Projects.FindAsync(id);
            var projects = await _dbContext.Projects.ToListAsync();
            var tickets = await _dbContext.Tickets.ToListAsync();

            //GET OL USERS
            var users = await _dbContext.Users.ToListAsync();
            //GET USERS IN PROJECT
            var userInProject = await _dbContext.ProjUsers.Where(p => p.ProjectId == id).ToListAsync();
            //STORE IN HT
            Hashtable ht = new Hashtable();

            foreach(ApplicationsUser user in users)
            {
                ht.Add(user.Id, user.Email);
            }

            List<string> assignedUsers = new List<string>();

            //LOOP THROUGH USERSINPROJECT AND ASSIGN NAME OR EMAIL TO LIST OF ASSIGNED USERS IF MATCHES HT
            foreach(var user in userInProject)
            {
                foreach(DictionaryEntry de in ht)
                {
                    if(de.Key.ToString() == user.ApplicationUserId.ToString())
                    {
                        assignedUsers.Add(de.Value.ToString());
                    }
                }
            }

            if (projectExists == null)
                throw new NullReferenceException("Project not found");

            var projectTickets = from p in projects
                                 join t in tickets
                                 on p.ProjectId equals t.ProjectId
                                 select new
                                 {
                                     ProjId = p.ProjectId,
                                     TicketId = t.TicketId,
                                     TicketTitle = t.Title,
                                     TicketDescription = t.Description ,
                                     TicketOwned =  t.TicketOwner,
                                     TicketReportDate = t.ReportDate,
                                     TicketStatus = t.Status,
                                     TicketPriority = t.Priority,
                                     TicketDue = t.DueDate,
                                     TicketTime = t.EstimateTime
                                 };

            var proj = projectTickets.Where(i => i.ProjId == id);


            var py = proj as Object;

            //return py;
            return new
            {
                ProjectTickets = py,
                AssignedUsers = assignedUsers.ToList()
            };
        }

        public void UpdateProjectAsync(int id, Project project)
        {
            //check project id and supplied id match
            if (id != project.ProjectId)
                throw new NullReferenceException("Id do not match");

            //check project exists
            var projectExists =  _dbContext.Projects.Find(id);
            if (projectExists == null)
                throw new NullReferenceException("Project does not exists");

            //update
            //_dbContext.Entry(project).State = EntityState.Modified;
            projectExists.ProjectName = string.IsNullOrWhiteSpace(project.ProjectName) ?
                                         projectExists.ProjectName : project.ProjectName;
            projectExists.Description = string.IsNullOrWhiteSpace(project.Description) ?
                                         projectExists.Description : project.Description;

            if(project.AssignedUserId != null)
            {
                var recordsToDel = _dbContext.ProjUsers.Where(p => p.ProjectId == id);
                _dbContext.ProjUsers.RemoveRange(recordsToDel);

                foreach(var projuser in project.AssignedUserId)
                {
                    var projRecord = new ProjUser
                    {
                        ApplicationUserId = projuser,
                        ProjectId = id
                    };

                    _dbContext.ProjUsers.Add(projRecord);
                }
            }

            _dbContext.SaveChanges();
            
        }
    }
}
