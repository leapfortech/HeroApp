using System;
using UnityEngine;
using UnityEngine.Events;

using Leap.Core.Tools;
using Leap.UI.Dialog;

using Sirenix.OdinInspector;

public class DialogTester : MonoBehaviour
{
    [Space]
    [SerializeField, SceneObjectsOnly]
    GameObject[] objectsToAwake = null;

    [Title("Screen")]
    [SerializeField]
    float screenDelay = 3f;

    [Title("Choice")]
    [SerializeField]
    Sprite[] choiceSprites = null;

    private void Start()
    {
        for (int i = 0; i < objectsToAwake.Length; i++)
            objectsToAwake[i].SetActive(true);
    }

    public void LaunchTimer()
    {
        Invoke(nameof(StopTimer), screenDelay);
    }

    private void StopTimer()
    {
        ScreenDialog.Instance.Hide();
    }

    public void Info()
    {
        ChoiceDialog.Instance.Info("Información", "Este es el mensaje de Información");
    }

    public void Info2()
    {
        ChoiceDialog.Instance.Info("Información", "Este es el mensaje de Información 2", () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }

    public void InfoH()
    {
        ChoiceDialog.Instance.InfoH("Información H", "Este es el mensaje de Información");
    }

    public void InfoHI()
    {
        ChoiceDialog.Instance.InfoH("Información H", "Este es el mensaje de Información I", choiceSprites[0]);
    }

    public void Info2H()
    {
        ChoiceDialog.Instance.InfoH("Información H", "Este es el mensaje de Información 2", () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }

    public void Info2HI()
    {
        ChoiceDialog.Instance.InfoH("Información H", "Este es el mensaje de Información 2I", choiceSprites[0], () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }


    public void Error()
    {
        ChoiceDialog.Instance.Error("Error", "Este es el mensaje de Error");
    }

    public void Error2()
    {
        ChoiceDialog.Instance.Error("Error", "Este es el mensaje de Error 2", () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }

    public void ErrorH()
    {
        ChoiceDialog.Instance.ErrorH("Error H", "Este es el mensaje de Error");
    }

    public void ErrorHI()
    {
        ChoiceDialog.Instance.ErrorH("Error H", "Este es el mensaje de Error I", choiceSprites[0]);
    }

    public void Error2H()
    {
        ChoiceDialog.Instance.ErrorH("Error H", "Este es el mensaje de Error 2", () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }

    public void Error2HI()
    {
        ChoiceDialog.Instance.ErrorH("Error H", "Este es el mensaje de Error 2I", choiceSprites[0], () => { Debug.Log("Validado"); }, null, "Validar", "Cancelar");
    }

    public void Menu4()
    {
        ChoiceDialog.Instance.Menu("Menú", new UnityAction[] { () => { Debug.Log("Relación de dependencia"); }, () => { Debug.Log("Negocio propio"); }, () => { Debug.Log("Otros"); } },
                                           new String[] { "Relación de dependencia", "Negocio propio", "Otros", "Regresar" });
    }

    public void Menu4K()
    {
        ChoiceDialog.Instance.Menu("Menú K", new UnityAction[] { () => { Debug.Log("Tomar fotografía"); }, () => { Debug.Log("Elegir del carrete"); }, () => { Debug.Log("Quitar imagen"); } },
                                             new String[] { "Tomar fotografía", "Elegir del carrete", "Quitar imagen", "Regresar" },
                                             new Sprite[] { choiceSprites[1], choiceSprites[2], choiceSprites[3], null });
    }
}
