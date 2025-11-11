using System;
using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class AppointmentRegisterAction : MonoBehaviour
{
    [Header("Action")]
    [SerializeField]
    Button btnRegister = null;

    [Title("Message")]
    [SerializeField]
    String appoRegisteredTitle = "Reunión";
    [SerializeField, TextArea(2, 4)]
    String appoRegisteredMessage = "Registrado satisfactoriamente.";

    MeetingService meetingService;
    int meetingId = -1;

    private void Awake()
    {
        meetingService = GetComponent<MeetingService>();
    }

    private void Start()
    {
        btnRegister?.AddAction(RegisterAppointment);
    }

    public void SetMeetingId(int index)
    {
        this.meetingId = StateManager.Instance.MeetingFulls[index].Id;
    }

    public void Clear()
    {
        meetingId = -1;
    }

    public void RegisterAppointment()
    {
        ScreenDialog.Instance.Display();
        meetingService.RegisterAppointment(new Appointment(meetingId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyAppointment(int id)
    {
        ScreenDialog.Instance.Hide();
        ChoiceDialog.Instance.Info(appoRegisteredTitle, appoRegisteredMessage);
    }
}
