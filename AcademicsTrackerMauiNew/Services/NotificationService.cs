using AcademicsTrackerMauiNew.Models;
using Plugin.LocalNotification;

namespace AcademicsTrackerMauiNew.Services
{
    /// <summary>
    /// Schedule and cancel notifications services related to courses and assessments.
    /// </summary>
    public class NotificationService
    {
        // Schedule a course notification request
        public static void ScheduleCourseNotification(Course course)
        {
            // If start notification set to true:
            if (course.NotifyStart)
            {
                var startReq = new NotificationRequest
                {
                    NotificationId = 1000 + course.Id * 10,
                    Title = $"Course Start: {course.Title}",
                    Subtitle = "Reminder",
                    Description = $"Your course {course.Title} starts today.",
                    Schedule = new NotificationRequestSchedule { NotifyTime = course.StartDate }
                };
                LocalNotificationCenter.Current.Show(startReq);
            }

            // If end notification set to true:
            if (course.NotifyEnd)
            {
                var endReq = new NotificationRequest
                {
                    NotificationId = 1000 + course.Id * 10 + 1,
                    Title = $"Course End: {course.Title}",
                    Subtitle = "Due Soon",
                    Description = $"Your course {course.Title} ends today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = course.EndDate }
                };
                LocalNotificationCenter.Current.Show(endReq);
            }
        }

        // Schedule an Assessment notification request
        public static void ScheduleAssessmentNotification(Assessment assessment)
        {
            if (assessment.NotifyStart)
            {
                var startReq = new NotificationRequest
                {
                    NotificationId = 2000 + assessment.Id * 10,
                    Title = $"Assessment Start: {assessment.Title}",
                    Subtitle = "Get Ready",
                    Description = $"Your {assessment.Type} assessment starts today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = assessment.StartDate }
                };
                LocalNotificationCenter.Current.Show(startReq);
            }

            if (assessment.NotifyEnd)
            {
                var endReq = new NotificationRequest
                {
                    NotificationId = 2000 + assessment.Id * 10 + 1,
                    Title = $"Assessment Due: {assessment.Title}",
                    Subtitle = "Time's Up",
                    Description = $"Your {assessment.Type} assessment is due today!",
                    Schedule = new NotificationRequestSchedule { NotifyTime = assessment.EndDate }
                };
                LocalNotificationCenter.Current.Show(endReq);
            }
        }

        // Handle notification cancellations
        public static void CancelCourseNotifications(int courseId)
        {
            LocalNotificationCenter.Current.Cancel(1000 + courseId * 10);
            LocalNotificationCenter.Current.Cancel(1000 + courseId * 10 + 1);
        }

        public static void CancelAssessmentNotifications(int assessmentId)
        {
            LocalNotificationCenter.Current.Cancel(2000 + assessmentId * 10);
            LocalNotificationCenter.Current.Cancel(2000 + assessmentId * 10 + 1);
        }
    }
}