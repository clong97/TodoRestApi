using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using TodoRestApi.Dto;
using TodoRestApi.Models;
using TodoRestApi.Utils;

namespace TodoRestApi.Controllers
{
    [BasicAuthentication]
    [RoutePrefix("api/team")]
    public class TeamController : ApiController
    {
        private TodoDbEntities db = new TodoDbEntities();

        [ResponseType(typeof(Team))]
        [HttpGet]
        [Route("getMyTeams")]
        public IQueryable<Team> GetMyTeams()
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            IQueryable<int> idList = db.TeamMembers.Where(tm => tm.User.Username == username && tm.Status == Constant.STATUS_ACTIVE).Select(tm => tm.TeamId);
            return db.Teams.Where(t => idList.Contains(t.Id));
        }

        [ResponseType(typeof(Team))]
        [HttpGet]
        [Route("getMyTeam/{id}")]
        public IHttpActionResult GetMyTeam(int id)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            string username = Thread.CurrentPrincipal.Identity.Name;
            TeamMember member = team.TeamMembers.SingleOrDefault(tm => tm.User.Username == username);
            if (member != null)
            {
                return Ok(team);
            }

            return Unauthorized();
        }

        [ResponseType(typeof(Team))]
        [HttpPut]
        [Route("updateMyTeam/{id}")]
        public IHttpActionResult UpdateMyTeam(int id, [FromBody]TeamDto model)
        {
            if (ModelState.IsValid)
            {
                Team team = db.Teams.Find(id);
                TeamMember owner = team.TeamMembers.SingleOrDefault(tm => tm.Role == Constant.ROLE_OWNER);

                if (IsOwner(owner))
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(model.Name))
                        {
                            team.Name = model.Name;
                        }

                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TeamExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Ok(team);
                }
                return Unauthorized();
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("createTeam")]
        public IHttpActionResult CreateTeam([FromBody]TeamDto model)
        {
            if (ModelState.IsValid)
            {
                string username = Thread.CurrentPrincipal.Identity.Name;
                User user = db.Users.SingleOrDefault(u => u.Username == username);

                TeamMember member = new TeamMember();
                member.Status = Constant.STATUS_ACTIVE;
                member.JoinedDate = DateTime.Now;
                member.LeaveDate = null;
                member.Role = Constant.ROLE_OWNER;
                member.User = user;

                Team team = new Team();
                team.Name = model.Name;
                team.Status = Constant.STATUS_ACTIVE;
                team.TeamMembers.Add(member);

                db.Teams.Add(team);
                db.SaveChanges();

                return Ok(team);
            }
            return BadRequest(ModelState);
        }

        [ResponseType(typeof(Team))]
        [HttpPut]
        [Route("deactivateTeam/{id}")]
        public IHttpActionResult DeactivateTeam(int id)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            TeamMember owner = team.TeamMembers.SingleOrDefault(tm => tm.Role == Constant.ROLE_OWNER);
            if (IsOwner(owner))
            {
                team.Status = Constant.STATUS_DEACTIVATED;

                foreach (TeamMember member in team.TeamMembers)
                {
                    member.Status = Constant.STATUS_DEACTIVATED;
                }

                foreach (Task task in team.Tasks)
                {
                    if (task.Status != Constant.STATUS_COMPLETED)
                    {
                        task.Status = Constant.STATUS_CANCELLED;
                    }
                }

                db.SaveChanges();

                return Ok(team);
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("getMyTeam/{id}/members")]
        public IHttpActionResult GetMyTeamMembers(int id)
        {
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }

            string username = Thread.CurrentPrincipal.Identity.Name;
            TeamMember member = team.TeamMembers.SingleOrDefault(tm => tm.User.Username == username);
            if (member != null)
            {
                return Ok(new { team, team.TeamMembers });
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("inviteMember")]
        public IHttpActionResult InviteMember([FromBody]InviteDto model)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.SingleOrDefault(u => u.Username == model.Username);
                Team team = db.Teams.Find(model.TeamId);

                if (user != null && team != null)
                {
                    if (!team.TeamMembers.Any(tm => tm.User == user))
                    {
                        TeamMember member = new TeamMember();
                        member.Status = Constant.STATUS_INVITED;
                        member.Role = Constant.ROLE_MEMBER;
                        member.User = user;
                        member.Team = team;

                        db.TeamMembers.Add(member);
                        db.SaveChanges();

                        return Ok("User invited.");
                    }
                    return Ok("User already invited.");
                }
                return Ok("User or Team does not exist.");
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("acceptInvitation/{id}")]
        public IHttpActionResult AcceptInvitation(int id)
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            User user = db.Users.SingleOrDefault(u => u.Username == username);
            Team team = db.Teams.Find(id);

            if (team != null)
            {
                TeamMember member = team.TeamMembers.SingleOrDefault(tm => tm.User == user && tm.Status == Constant.STATUS_INVITED);
                if (member != null)
                {
                    member.Status = Constant.STATUS_ACTIVE;
                    member.JoinedDate = DateTime.Now;

                    db.SaveChanges();

                    return Ok("Invitation accepted.");
                }
                return Ok("Invitation does not exist.");
            }
            return Ok("Team does not exist.");
        }

        [HttpPut]
        [Route("rejectInvitation/{id}")]
        public IHttpActionResult RejectInvitation(int id)
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            User user = db.Users.SingleOrDefault(u => u.Username == username);
            Team team = db.Teams.Find(id);

            if (team != null)
            {
                TeamMember member = team.TeamMembers.SingleOrDefault(tm => tm.User == user && tm.Status == Constant.STATUS_INVITED);
                if (member != null)
                {
                    member.Status = Constant.STATUS_REJECTED;

                    db.SaveChanges();

                    return Ok("Invitation rejected.");
                }
                return Ok("Invitation does not exist.");
            }
            return Ok("Team does not exist.");
        }

        [HttpPut]
        [Route("leaveTeam/{id}")]
        public IHttpActionResult LeaveTeam(int id)
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            User user = db.Users.SingleOrDefault(u => u.Username == username);
            Team team = db.Teams.Find(id);

            if (team != null)
            {
                TeamMember member = team.TeamMembers.SingleOrDefault(tm => tm.User == user && tm.Status == Constant.STATUS_ACTIVE);
                if (member != null)
                {
                    member.Status = Constant.STATUS_DEACTIVATED;
                    member.LeaveDate = DateTime.Now;

                    db.SaveChanges();

                    return Ok("Left the team.");
                }
                return Ok("Invalid action. You are either not in the team or already left.");
            }
            return Ok("Team does not exist.");
        }

        [ResponseType(typeof(Team))]
        [HttpGet]
        [Route("getMyInvitations")]
        public IQueryable<Team> GetMyInvitations()
        {
            string username = Thread.CurrentPrincipal.Identity.Name;
            IQueryable<int> idList = db.TeamMembers.Where(tm => tm.User.Username == username && tm.Status == Constant.STATUS_INVITED).Select(tm => tm.TeamId);
            return db.Teams.Where(t => idList.Contains(t.Id));
        }

        private bool TeamExists(int id)
        {
            return db.Teams.Count(e => e.Id == id) > 0;
        }

        private bool IsOwner(TeamMember member)
        {
            string username = Thread.CurrentPrincipal.Identity.Name;

            return member.User.Username == username;
        }
    }
}