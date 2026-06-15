using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Medri.Web.Infrastructure
{
    public static class Alerts
    {
        public const string ALERTS_KEY = "AlertsModel";

        public static void AddInfo(Controller controller, string value)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.info, value, false, 0);
        }

        public static void AddInfo(Controller controller, string value, int millisecondsBeforeAutoDismiss)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.info, value, true, millisecondsBeforeAutoDismiss);
        }

        public static void AddSuccess(Controller controller, string value)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.success, value, false, 0);
        }

        public static void AddSuccess(Controller controller, string value, int millisecondsBeforeAutoDismiss)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.success, value, true, millisecondsBeforeAutoDismiss);
        }

        public static void AddWarning(Controller controller, string value)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.warning, value, false, 0);
        }

        public static void AddWarning(Controller controller, string value, int millisecondsBeforeAutoDismiss)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.warning, value, true, millisecondsBeforeAutoDismiss);
        }

        public static void AddError(Controller controller, string value)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.error, value, false, 0);
        }

        public static void AddError(Controller controller, string value, int millisecondsBeforeAutoDismiss)
        {
            AddAlert(controller.ViewData, AlertsDto.AlertLevelEnum.error, value, true, millisecondsBeforeAutoDismiss);
        }

        static void AddAlert(ViewDataDictionary viewData, AlertsDto.AlertLevelEnum level, string value, bool autoDismiss, int milliseconds)
        {
            var alerts = viewData[ALERTS_KEY] as AlertsDto;
            if (alerts == null)
            {
                viewData[ALERTS_KEY] = alerts = new AlertsDto();
            }

            alerts.AddAlert(level, value, autoDismiss, milliseconds);
        }
    }

    public class AlertsDto
    {
        public bool HasAlerts => List.Count > 0;
        public List<Alert> List { get; set; }

        public AlertsDto()
        {
            List = new List<Alert>();
        }

        public void AddAlert(AlertLevelEnum level, string value, bool autoDismiss, int millisecondsAutoDismiss = 0)
        {
            List.Add(new Alert()
            {
                Level = level,
                Value = value,
                AutoDismiss = autoDismiss,
                MillisecondsAutoDismiss = millisecondsAutoDismiss
            });
        }

        public class Alert
        {
            public AlertLevelEnum Level { get; set; }
            public string Value { get; set; }
            public bool AutoDismiss { get; set; }
            public int MillisecondsAutoDismiss { get; set; }
        }

        public enum AlertLevelEnum
        {
            info,
            success,
            warning,
            error,
        }
    }

    public class AlertsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            OnActionExecutingAlert(context);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            OnActionExecutedAlert(context);
            base.OnActionExecuted(context);
        }

        void OnActionExecutingAlert(ActionExecutingContext context)
        {
            var controller = (Controller)context.Controller;
            var tempDataAlerts = controller.TempData[Alerts.ALERTS_KEY] as string;

            if (tempDataAlerts != null)
            {
                controller.ViewData[Alerts.ALERTS_KEY] = JsonConvert.DeserializeObject<AlertsDto>(tempDataAlerts);
            }
        }

        void OnActionExecutedAlert(ActionExecutedContext context)
        {
            if (context.Result is RedirectResult
                || context.Result is RedirectToRouteResult
                || context.Result is RedirectToActionResult)
            {
                var controller = (Controller)context.Controller;
                var viewDataAlerts = controller.ViewData[Alerts.ALERTS_KEY];

                if (viewDataAlerts != null)
                {
                    controller.TempData[Alerts.ALERTS_KEY] = JsonConvert.SerializeObject(viewDataAlerts);
                }
            }
        }
    }
}
