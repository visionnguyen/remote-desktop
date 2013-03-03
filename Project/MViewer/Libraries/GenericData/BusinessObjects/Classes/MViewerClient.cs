﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5456
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



using System.ServiceModel;
using System.ServiceModel.Channels;
using GenericDataLayer;
using Utils;
using System;
using System.Drawing;
using System.IO;

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
public partial class MViewerClient : ClientBase<IMViewerService>, IMViewerService
{
    
    public MViewerClient()
    {

    }
    
    public MViewerClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public MViewerClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public MViewerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public MViewerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {

    }

    public void UpdateFriendlyName(string senderIdentity, string newFriendlyName)
    {
        base.Channel.UpdateFriendlyName(senderIdentity, newFriendlyName);
    }

    public void UpdateContactStatus(string senderIdentity, GenericEnums.ContactStatus newStatus)
    {
        base.Channel.UpdateContactStatus(senderIdentity, newStatus);
    }

    public void SendWebcamCapture(byte[] capture, string senderIdentity)
    {
        base.Channel.SendWebcamCapture(capture, senderIdentity);
    }
    
    public void SendMicrophoneCapture(byte[] capture)
    {
        base.Channel.SendMicrophoneCapture(capture);
    }

    public bool Ping()
    {
        return base.Channel.Ping();
    }

    public void SendRoomButtonAction(string identity, GenericEnums.RoomType roomType, GenericEnums.SignalType signalType)
    {
        base.Channel.SendRoomButtonAction(identity, roomType, signalType);
    }

    public void WaitRoomButtonAction(string senderIdentity, GenericEnums.RoomType roomType, bool wait)
    {
        base.Channel.WaitRoomButtonAction(senderIdentity, roomType, wait);
    }

    public void AddContact(string identity, string friendlyName)
    {
        base.Channel.AddContact(identity, friendlyName);
    }

    public void RemoveContact(string identity)
    {
        base.Channel.RemoveContact(identity);
    }

    public void SendFile(byte[] fileBytes, string fileName)
    {
        base.Channel.SendFile(fileBytes, fileName);
    }

    public bool SendingPermission(string senderIdentity, string fileName, long fileSize)
    {
        return base.Channel.SendingPermission(senderIdentity, fileName, fileSize);
    }
}
