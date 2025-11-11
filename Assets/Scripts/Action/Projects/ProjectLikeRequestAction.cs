using UnityEngine;

using Leap.UI.Elements;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;
public class ProjectLikeRequestAction : MonoBehaviour
{
    [Title("Action")]
    [SerializeField]
    Button btnProjectLike = null;
    
    int projectId = -1;

    ProjectService projectService;

    private void Awake()
    {
        projectService = GetComponent<ProjectService>();
    }

    private void Start()
    {
        btnProjectLike?.AddAction(RequestProjectLike);
    }

    public void SetProjectId(int projectId)
    {
        this.projectId = projectId;
    }

    private void RequestProjectLike()
    {
        ScreenDialog.Instance.Display();
        projectService.RegisterProjectLike(new ProjectLike(projectId, StateManager.Instance.AppUser.Id));
    }

    public void ApplyProjectLike(int id)
    {
        StateManager.Instance.ProjectLikeIds.Add(projectId);
        ScreenDialog.Instance.Hide();
        ChoiceDialog.Instance.Info("Favoritos", "El proyecto fue agregado a tus favoritos.");
    }
}