using Flunt.Notifications;
using System;
using System.Collections.Generic;

namespace Application.Common
{
    public class ServiceResult
    {
        public IEnumerable<Notification> Notifications { get; set; }
        public StatusResult Status { get; set; }
        public string StatusDescription => Status.ToString();

        public enum StatusResult
        {
            Ok,
            Error,
            Warning
        }

        public static ServiceResult Error(Exception ex)
        {
            return new ServiceResult
            {
                Notifications = new List<Notification> { new Notification(ex.GetType().Name, ex.GetMessageConcatenatedWithInner()) },
                Status = StatusResult.Error
            };
        }
    }

    public class ServiceTResult<T> : ServiceResult
    {
        public T Data { get; set; }
    }
}
