using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using TodoRestApi.Dto;
using TodoRestApi.Models;
using TodoRestApi.Utils;

namespace TodoRestApi.Controllers
{
    [BasicAuthentication]
    [RoutePrefix("api/task")]
    public class TaskController : ApiController
    {
        private TodoDbEntities db = new TodoDbEntities();

        [ResponseType(typeof(Task))]
        [HttpGet]
        [Route("getAllTask")]
        public IQueryable<Task> GetAllTasks(string sort = "id")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            IQueryable<int> idList = db.TeamMembers.Where(tm => tm.User.Username == username && tm.Status == Constant.STATUS_ACTIVE).Select(tm => tm.TeamId);

            return db.Tasks.Where(t => t.User1.Username == username || t.User.Username == username || idList.Contains((int)t.TeamId)).ApplySort(sort);
        }

        [ResponseType(typeof(Task))]
        [HttpGet]
        [Route("getMyTasks")]
        public IQueryable<Task> GetMyTasks(string sort = "id")
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            return db.Tasks.Where(t => (t.User == null && t.User1.Username == username) || (t.User.Username == username)).ApplySort(sort);
        }

        [ResponseType(typeof(Task))]
        [HttpGet]
        [Route("getMyTask/{id}")]
        public IHttpActionResult GetMyTask(int id)
        {
            Task task = db.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            string username = Thread.CurrentPrincipal.Identity.Name;
            if (task.User.Username == username || (task.User1.Username == username && task.User == null))
            {
                return Ok(task);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("createTask")]
        public IHttpActionResult CreateTask([FromBody]CreateTaskDto model)
        {
            if (ModelState.IsValid)
            {
                string username = Thread.CurrentPrincipal.Identity.Name;
                User user = db.Users.SingleOrDefault(u => u.Username == username);

                Task task = new Task();
                task.Name = model.Name;
                task.Description = model.Description;
                task.Priority = model.Priority;
                task.Status = Constant.STATUS_NEW;
                task.Progress = 0;
                task.User1 = user;
                task.CreatedDate = DateTime.Now;

                DateTime startDt = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                task.StartDate = startDt;

                DateTime dueDt = DateTime.ParseExact(model.DueDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                task.DueDate = dueDt;

                Team team = db.Teams.Find(model.TeamId);
                task.Team = team ?? null;

                User assignee = db.Users.Find(model.AssigneeId);
                task.User = assignee ?? null;

                db.Tasks.Add(task);
                db.SaveChanges();

                return Ok(task);
            }
            return BadRequest(ModelState);
        }

        [ResponseType(typeof(Task))]
        [HttpPut]
        [Route("updateMyTask/{id}")]
        public IHttpActionResult UpdateMyTask(int id, [FromBody]UpdateTaskDto model)
        {
            if (ModelState.IsValid)
            {
                Task task = db.Tasks.Find(id);

                if (task != null)
                {
                    string username = Thread.CurrentPrincipal.Identity.Name;
                    IQueryable<int> idList = db.Tasks.Where(t => (t.User == null && t.User1.Username == username) || (t.User.Username == username)).Select(t => t.Id);

                    if (idList.Contains(task.Id))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                task.Name = model.Name;
                            }

                            if (!string.IsNullOrEmpty(model.Description))
                            {
                                task.Description = model.Description;
                            }

                            if (!string.IsNullOrEmpty(model.Priority))
                            {
                                task.Priority = model.Priority;
                            }

                            if (!string.IsNullOrEmpty(model.Status))
                            {
                                task.Status = model.Status;
                            }

                            if (model.Progress != null)
                            {
                                task.Progress = model.Progress ?? task.Progress;
                            }

                            if (model.AssigneeId != null)
                            {
                                User assignee = db.Users.Find(model.AssigneeId);
                                task.User = assignee ?? task.User;
                            }

                            db.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!TaskExists(id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return Ok(task);
                    }
                    return Unauthorized();
                }
                return NotFound();
            }
            return BadRequest(ModelState);
        }

        // TODO: Deactivate Task

        private bool TaskExists(int id)
        {
            return db.Tasks.Count(e => e.Id == id) > 0;
        }
    }
}