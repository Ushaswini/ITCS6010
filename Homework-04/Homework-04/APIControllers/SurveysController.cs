using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Homework_04.Models;
using Homework_04.DTOs;
using PushBots.NET;
using PushBots.NET.Models;
using PushBots.NET.Enums;
using Hangfire;
using Newtonsoft.Json.Linq;

namespace Homework_04.Controllers
{
    [Authorize]
    [RoutePrefix("api/Surveys")]
    public class SurveysController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private readonly PushBotsClient _pushBotsClient;
        private const string AppId = "5a0c68d49b823ae75c8b4568";
        private const string Secret = "2ea297eee9933ab16e23bbc28d1cf2c1";

        public SurveysController()
        {
            _pushBotsClient = new PushBotsClient(AppId, Secret);
        }

        protected PushBotsClient PushBots_Client
        {
            get
            {
                return _pushBotsClient;
            }
        }

        // GET: api/Surveys
        public IList<SurveyDTO> GetSurveys()
        {
            return db.Surveys.Include(s => s.StudyGroup).Select(s => new SurveyDTO {
                        SurveyId = s.SurveyId,
                        SurveyCreatedTime = s.SurveyCreatedTime,
                        QuestionText = s.QuestionText,
                        StudyGroupId = s.StudyGroupId,
                        StudyGroupName = s.StudyGroup.StudyName

            }).ToList();
        }

        public IList<SurveyDTO> GetSurveysForStudyGroup(string studyGroupId)
        {
            var surveys = db.Surveys.Where(s => s.StudyGroupId == studyGroupId).Include(s => s.StudyGroup).Select(s => new SurveyDTO
            {
                SurveyId = s.SurveyId,
                SurveyCreatedTime = s.SurveyCreatedTime,
                QuestionText = s.QuestionText,
                StudyGroupId = s.StudyGroupId,
                StudyGroupName = s.StudyGroup.StudyName

            });

            return surveys.ToList();
        }

        // GET: api/Surveys/5
        [ResponseType(typeof(Survey))]
        public IHttpActionResult GetSurvey(string id)
        {
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        // PUT: api/Surveys/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSurvey(string id, Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != survey.SurveyId)
            {
                return BadRequest();
            }

            db.Entry(survey).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Surveys
        [Route("Post")]
        [ResponseType(typeof(Survey))]
        public IHttpActionResult PostSurvey(Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Surveys.Add(survey);

            try
            {
                db.SaveChanges();
                switch (survey.FrequencyOfNotifications)
                {
                    case Frequency.Daily:
                        {
                            String[] times = survey.Time1.Split(':');
                            String cornExpression = times[1] + " " + times[0] + " * * *";
                            RecurringJob.AddOrUpdate(survey.SurveyId, () => PushNotificationsAsync(), cornExpression,TimeZoneInfo.Local);
                            break;
                        }
                    case Frequency.Hourly:
                        PushNotificationsAsync();
                        RecurringJob.AddOrUpdate(survey.SurveyId, () => PushNotificationsAsync(), Cron.Hourly, TimeZoneInfo.Local);
                        break;
                    case Frequency.TwiceDaily:
                        {
                            String[] times = survey.Time1.Split(':');
                            String cornExpression = times[1] + " " + times[0] + " * * *";
                            String[] times2 = survey.Time2.Split(':');
                            String cornExpression2 = times2[1] + " " + times2[0] + " * * *";
                            RecurringJob.AddOrUpdate(survey.SurveyId + "First", () => PushNotificationsAsync(), cornExpression, TimeZoneInfo.Local);
                            RecurringJob.AddOrUpdate(survey.SurveyId + "Second", () => PushNotificationsAsync(), cornExpression2, TimeZoneInfo.Local);
                            break;
                        }
                }
                //RecurringJob.AddOrUpdate(survey.SurveyId,() => PushNotificationsAsync(), Cron.Minutely);
                //PushNotificationsAsync();
                //push notification to users who opted in
            }
            catch (DbUpdateException)
            {
                if (SurveyExists(survey.SurveyId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(survey);
        }


        public void PushNotificationsAsync()
        {
            var payload = new JObject();
            payload.Add("ntitle", "CareMe");
            payload.Add("message", "Co-ordinator has a question for you!!");
            payload.Add("inboxStyle", "true");
            var pushMessage = new BatchPush()
            {
                Message = "Survey Received!!",
                Badge = "+1",
                Platforms = new[] { Platform.Android, Platform.iOS },
                Payload = payload
            };

            

            var result =  PushBots_Client.Push(pushMessage);

        }
        // DELETE: api/Surveys/5
        [ResponseType(typeof(Survey))]
        public IHttpActionResult DeleteSurvey(string id)
        {
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return NotFound();
            }

            db.Surveys.Remove(survey);
            db.SaveChanges();

            switch (survey.FrequencyOfNotifications)
            {
                case Frequency.Daily:
                    {
                        RecurringJob.RemoveIfExists(id);
                        break;
                    }
                    
                case Frequency.Hourly:
                    {
                        RecurringJob.RemoveIfExists(id);
                        break;
                    }
                case Frequency.TwiceDaily:
                    {
                        RecurringJob.RemoveIfExists(id + "First");
                        RecurringJob.RemoveIfExists(id + "Second");
                        break;
                    }
                    
            }
            

            return Ok(survey);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SurveyExists(string id)
        {
            return db.Surveys.Count(e => e.SurveyId == id) > 0;
        }
    }
}