using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using hg.ApiWebKit.core.http;

using Leap.Core.Tools;
using Leap.Data.Web;

using Sirenix.OdinInspector;

public class AddressService : MonoBehaviour
{
    [Serializable]
    public class AddressEvent : UnityEvent<Address> { }

    [SerializeField]
    private AddressEvent onRetreived = null;

    [SerializeField]
    private UnityIntEvent onRegistered = null;

    [SerializeField]
    private UnityIntEvent onAdded = null;

    [SerializeField]
    private UnityIntEvent onUpdated = null;

    [Title("Error")]
    [SerializeField]
    private UnityStringEvent onResponseError = null;


    // GET
    public void GetAddress()
    {
       AddressGetOperation addressGetOp = new AddressGetOperation();
        try
        {
            addressGetOp.appUserId = StateManager.Instance.AppUser.Id;
            addressGetOp["on-complete"] = (Action<AddressGetOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRetreived.Invoke(op.address);
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            addressGetOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // REGISTER
    public void RegisterAppUser(AddressInfo addressInfo)
    {
        AddressAppUserRegisterOperation addressRegisterPostOp = new AddressAppUserRegisterOperation();
        try
        {
            addressRegisterPostOp.appUserId = StateManager.Instance.AppUser.Id;
            addressRegisterPostOp.addressInfo = addressInfo;
            addressRegisterPostOp["on-complete"] = (Action<AddressAppUserRegisterOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onRegistered.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            addressRegisterPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // ADD
    public void Add(Address address)
    {
        AddressPostOperation addressPostOp = new AddressPostOperation();
        try
        {
            addressPostOp.Address = address;
            addressPostOp["on-complete"] = (Action<AddressPostOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onAdded.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            addressPostOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }

    // UPDATE

    public void UpdateAddress(Address address)
    {
        AddressPutOperation addressPutOp = new AddressPutOperation();
        try
        {
            addressPutOp.address = address;
            addressPutOp["on-complete"] = (Action<AddressPutOperation, HttpResponse>)((op, response) =>
            {
                if (response != null && !response.HasError)
                    onUpdated.Invoke(Convert.ToInt32(op.id));
                else
                    onResponseError.Invoke(response.Text.Length == 0 ? response.Error : response.Text);
            });
            addressPutOp.Send();
        }
        catch (Exception ex)
        {
            WebManager.Instance.OnSendError(ex.Message);
        }
    }
}
