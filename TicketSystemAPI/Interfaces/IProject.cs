using Core2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core2.Interfaces
{
    public interface IProject
    {
        Task<Project> CreateProjectAsync(Project project);

        Task<List<Project>> GetProjectsAsync();

        Task<Object> GetSingleProjectAsync(int? id);

        void UpdateProjectAsync(int id, Project project);

        void DeleteProjectAsync(int id);

        void AssignUsersProject(int appId, ProjUser projUser);

        void DeleteUserFromProject(int projId, string userId);
    }
}
