using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Page;
using Leap.Core.Tools;

using Sirenix.OdinInspector;

public class DisplayMeetingAction : MonoBehaviour
{
    [Title("List")]
    [SerializeField]
    ListScroller lstMeeting;
    [SerializeField]
    Text txtEmpty;

    [Space]
    [Title("Details")]
    [SerializeField]
    Text txtSubject = null;
    [SerializeField]
    FieldValue fldType = null;
    [SerializeField]
    FieldValue fldStartDateTime = null;
    [SerializeField]
    FieldValue fldEndDateTime = null;
    [SerializeField]
    Text txtDescription = null;

    [Title("Event")]
    [SerializeField]
    UnityIntEvent onMeetingSelected = null;

    [Title("Page")]
    [SerializeField]
    Page pagMeetingDetails;


    public int SelMeetingIdx { get; set; } = 0;

    public void Clear()
    {
        txtSubject.Clear();
        fldType.Clear();
        fldStartDateTime.Clear();
        fldEndDateTime.Clear();
        txtDescription.Clear();
        lstMeeting.Clear();
    }

    public Page PagMeetingDetailBack
    {
        get => pagMeetingDetails.HeaderPage;
        set => pagMeetingDetails.HeaderPage = value;
    }


    public void DisplayMeetingFulls()
    {
        if (StateManager.Instance.MeetingFulls == null)
            return;
        
        if (StateManager.Instance.MeetingFulls.Count == 0)
        {
            txtEmpty.gameObject.SetActive(true);
            return;
        }

        txtEmpty.gameObject.SetActive(false);

        lstMeeting.ClearValues();

        for (int i = 0; i < StateManager.Instance.MeetingFulls.Count; i++)
        {
            ListScrollerValue scrollerValue = new ListScrollerValue(4, true);
            scrollerValue.SetText(0, StateManager.Instance.MeetingFulls[i].Subject);
            scrollerValue.SetText(1, StateManager.Instance.MeetingFulls[i].MeetingType);
            scrollerValue.SetText(2, StateManager.Instance.MeetingFulls[i].StartDateTime.ToString("dd/MM/yyyy HH:mm"));
            scrollerValue.SetText(3, StateManager.Instance.MeetingFulls[i].EndDateTime.ToString("dd/MM/yyyy HH:mm"));

            lstMeeting.AddValue(scrollerValue);
        }

        lstMeeting.ApplyValues();
    }

    public void DisplayMeetingDetails()
    {
        txtSubject.TextValue = StateManager.Instance.MeetingFulls[SelMeetingIdx].Subject;
        fldType.TextValue = StateManager.Instance.MeetingFulls[SelMeetingIdx].MeetingType;
        fldStartDateTime.TextValue = StateManager.Instance.MeetingFulls[SelMeetingIdx].StartDateTime.ToString("dd 'de' MMMM 'de' yyyy '-' HH:mm 'horas'", StateManager.Instance.CultureInfo);
        fldEndDateTime.TextValue = StateManager.Instance.MeetingFulls[SelMeetingIdx].EndDateTime.ToString("dd 'de' MMMM 'de' yyyy '-' HH:mm 'horas'", StateManager.Instance.CultureInfo); ;
        txtDescription.TextValue = StateManager.Instance.MeetingFulls[SelMeetingIdx].Description;

        onMeetingSelected.Invoke(SelMeetingIdx);

        PageManager.Instance.ChangePage(pagMeetingDetails);
    }
}
