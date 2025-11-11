using System;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class MeetingService : MonoBehaviour
{
    [Serializable]
    public class MeetingsEvent : UnityEvent<Meeting[]> { }
    [Serializable]
    public class MeetingFullsEvent : UnityEvent<MeetingFull[]> { }

    [SerializeField]
    private MeetingsEvent onMeetingsRetreived = null;

    [SerializeField]
    private MeetingFullsEvent onMeetingFullsRetreived = null;

    [SerializeField]
    private UnityIntEvent onRegistered = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetByDates(DateTime startDateTime, DateTime endDateTime)
    {
        MeetingGetOperation meetingGetOp = new MeetingGetOperation();
        try
        {
            meetingGetOp.startDateTime = startDateTime;
            meetingGetOp.endDateTime = endDateTime;
            meetingGetOp["on-complete"] = (Action<MeetingGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onMeetingsRetreived.Invoke(op.meetings);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            meetingGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFulls()
    {
        MeetingFullsGetOperation meetingFullsGetOp = new MeetingFullsGetOperation();
        try
        {
            meetingFullsGetOp["on-complete"] = (Action<MeetingFullsGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onMeetingFullsRetreived.Invoke(op.meetingFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            meetingFullsGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    public void GetFullsByDates(DateTime startDateTime, DateTime endDateTime)
    {
        MeetingFullsByDatesGetOperation meetingFullsByDatesGetOp = new MeetingFullsByDatesGetOperation();
        try
        {
            meetingFullsByDatesGetOp.startDateTime = startDateTime;
            meetingFullsByDatesGetOp.endDateTime = endDateTime;
            meetingFullsByDatesGetOp["on-complete"] = (Action<MeetingFullsByDatesGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onMeetingFullsRetreived.Invoke(op.meetingFulls);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            meetingFullsByDatesGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void RegisterAppointment(Appointment appointment)
    {
        AppointmentRegisterOperation appointmentRegisterOp = new AppointmentRegisterOperation();
        try
        {
            appointmentRegisterOp.appointment = appointment;
            appointmentRegisterOp["on-complete"] = (Action<AppointmentRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt32(op.appointmentId));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            appointmentRegisterOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
